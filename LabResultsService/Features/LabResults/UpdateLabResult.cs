using Carter;
using ErrorOr;
using FluentValidation;
using LabResultsService.Contracts.LabResults;
using LabResultsService.Database;
using LabResultsService.Entities;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabResultsService.Features.LabResults
{
    public static class UpdateLabResult
    {
        public record Command(
            Guid LabResultId,
            string Value,
            DateTime ReportedDate
        ) : IRequest<ErrorOr<Unit>>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.LabResultId).NotEmpty();
                RuleFor(x => x.Value).MaximumLength(40).NotEmpty();
                RuleFor(x => x.ReportedDate).NotEmpty();
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
                var validationResult = _validator.Validate(command);
                if (!validationResult.IsValid)
                    return Error.Validation("Validation error", "Invalid input data");

                var labResult = await _context.LabResults
                    .FirstOrDefaultAsync(x => x.LabResultId == command.LabResultId, cancellationToken);

                if (labResult == null)
                    return Error.NotFound("Lab result not found", "The lab result with the given Id was not found");

                var updatedLabResult = LabResult.UpdateLabResult(
                    labResult,
                    command.Value,
                    command.ReportedDate);

                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }

    public class UpdateLabResultEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("api/labresults/{id}", async (string id,[FromBody] UpdateLabResultRequest request, [FromServices] ISender sender) =>
            {
                if (id != request.LabResultId.ToString())
                    return Results.BadRequest("Lab result not found");

                var command = request.Adapt<UpdateLabResult.Command>();

                var result = await sender.Send(command);

                return result.Match(
                    labResultId => Results.Ok(labResultId), 
                    errors => Results.BadRequest(errors));
            });
        }
    }
}
