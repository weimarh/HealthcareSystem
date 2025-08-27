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

namespace LabResultsService.Features.LabTests
{
    public static class CreateLabTest
    {
        public record Command(
            string TestName,
            string ReferenceRange,
            int SpecimenType
        ) : IRequest<ErrorOr<Guid>>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.TestName).NotEmpty().MaximumLength(50);
                RuleFor(x => x.ReferenceRange).MaximumLength(50);
                RuleFor(x => x.SpecimenType).NotEmpty();
            }
        }

        internal sealed class Handler : IRequestHandler<Command, ErrorOr<Guid>>
        {
            private readonly ApplicationDbContext _context;
            private readonly IValidator<Command> _validator;

            public Handler(ApplicationDbContext context, IValidator<Command> validator)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            }

            public async Task<ErrorOr<Guid>> Handle(Command command, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(command);

                if (!validationResult.IsValid)
                    return Error.Validation("Validation error", "Invalid input data");

                if (command.SpecimenType < 0 || command.SpecimenType > 4)
                    return Error.NotFound("Specimen error", "The specified specimen is out of range.");

                var labTest = new LabTest
                (
                    Guid.NewGuid(),
                    command.TestName,
                    (SpecimenType)command.SpecimenType,
                    command.ReferenceRange
                );

                _context.LabTests.Add( labTest );

                await _context.SaveChangesAsync();

                return labTest.LabTestId;
            }
        }
    }

    public class CreateLabTestEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/labtests", async ([FromBody]CreateLabTestRequest request, [FromServices] ISender sender) =>
            {
                var command = request.Adapt<CreateLabTest.Command>();

                var result = await sender.Send(command);

                return result.Match(
                    labTestId => Results.Ok(labTestId),
                    errors => Results.BadRequest(errors));
            });
        }
    }
}
