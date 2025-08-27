using Carter;
using ErrorOr;
using FluentValidation;
using LabResultsService.Contracts.LabTests;
using LabResultsService.Database;
using LabResultsService.Entities;
using LabResultsService.Enums;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabResultsService.Features.LabTests
{
    public static class UpdateLabTest
    {
        public record Command(
            Guid LabTestId,
            string TestName,
            string ReferenceRange,
            int SpecimenType
        ) : IRequest<ErrorOr<Unit>>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.LabTestId).NotEmpty();
                RuleFor(x => x.TestName).MaximumLength(50).NotEmpty();
                RuleFor(x => x.ReferenceRange).MaximumLength(50).NotEmpty();
                RuleFor(x => x.SpecimenType).NotEmpty();
            }
        }

        internal sealed class Handler : IRequestHandler<Command, ErrorOr<Unit>>
        {
            private readonly IValidator<Command> _validator;
            private readonly ApplicationDbContext _context;

            public Handler(IValidator<Command> validator, ApplicationDbContext context)
            {
                _validator = validator ?? throw new ArgumentNullException(nameof(validator));
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<ErrorOr<Unit>> Handle(Command command, CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(command);

                if (!validationResult.IsValid)
                    return Error.Validation("Validation error", "Invalid input data");

                var labTest = await _context.LabTests.FirstOrDefaultAsync(x => x.LabTestId == command.LabTestId);

                if (labTest == null)
                    return Error.NotFound("Lab test not found", "The lab test with the given Id was not found");

                LabTest updatedLabTest = LabTest.UpdateLabTest(
                    labTest,
                    command.TestName,
                    (SpecimenType)command.SpecimenType,
                    command.ReferenceRange);

                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }

    public class UpdateLabTestOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("api/laborders/{id}", async (string id, [FromBody]UpdateLabTestRequest request, [FromServices] ISender sender) =>
            {
                if (id != request.LabTestId.ToString())
                    return Results.BadRequest("Lab test not found");

                var command = request.Adapt<UpdateLabTest.Command>();

                var result = await sender.Send(command);

                return result.Match(
                    labOrderId => Results.Ok(labOrderId),
                        errors => Results.BadRequest(errors));
            });
        }
    }
}
