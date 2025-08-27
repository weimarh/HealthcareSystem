using Carter;
using ErrorOr;
using FluentValidation;
using LabResultsService.Contracts.LabResults;
using LabResultsService.Database;
using LabResultsService.Entities;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LabResultsService.Features.LabResults
{
    public static class CreateLabResult
    {
        public record Command(
            Guid LabOrderId,
            string Value,
            DateTime ReportedDate
        ) : IRequest<ErrorOr<Guid>>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.LabOrderId).NotEmpty();
                RuleFor(x => x.Value).MaximumLength(40) .NotEmpty();
                RuleFor(x => x.ReportedDate).NotEmpty();
            }
        }

        internal sealed class Handler : IRequestHandler<Command, ErrorOr<Guid>>
        {
            private readonly IValidator<Command> _validator;
            private readonly ApplicationDbContext _context;
            public Handler(IValidator<Command> validator, ApplicationDbContext context)
            {
                _validator = validator ?? throw new ArgumentNullException(nameof(validator));
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<ErrorOr<Guid>> Handle(Command command, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(command);

                if (!validationResult.IsValid)
                    return Error.Validation("Validation error", "Invalid input data");

                var labOrder = _context.LabOrders.FirstOrDefault(x => x.LabOrderId == command.LabOrderId);

                if (labOrder != null)
                    return Error.NotFound("Lab order not found", "The specified Lab order does not exist.");

                var labResult = new LabResult
                {
                    LabResultId = Guid.NewGuid(),
                    LabOrderId = command.LabOrderId,
                    LabOrder = labOrder,
                    Value = command.Value,
                    ReportedDate = command.ReportedDate,
                };

                _context.LabResults.Add(labResult);

                await _context.SaveChangesAsync();
                
                return labResult.LabResultId;
            }
        }
    }

    public class CreateLabResultEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/labresults", async ([FromBody]CreateLabResultRequest request, [FromServices]ISender sender) =>
            {
                var command = request.Adapt<CreateLabResult.Command>();

                var result = await sender.Send(command);

                return result.Match(
                    labResultId => Results.Ok(labResultId),
                    errors => Results.BadRequest(errors));
            });
        }
    }
}
