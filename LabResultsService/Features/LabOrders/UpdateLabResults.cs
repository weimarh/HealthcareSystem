using Carter;
using ErrorOr;
using FluentValidation;
using LabResultsService.Contracts.LabOrders;
using LabResultsService.Database;
using LabResultsService.Entities;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LabResultsService.Features.LabOrders
{
    public static class UpdateLabResults
    {
        public record Command(
            Guid LabOrderId,
            Guid LabResultId,
            LabResult LabResult
        ) : IRequest<ErrorOr<Unit>>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.LabOrderId).NotEmpty();
                RuleFor(x => x.LabResultId).NotEmpty();
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
                var validatorResult = _validator.Validate(command);
                if (!validatorResult.IsValid)
                    return Error.Validation("Validation error", "Invalid input data");

                var labOrder = _context.LabOrders.Find(command.LabOrderId);

                if (labOrder == null)
                    return Error.NotFound("Lab order not found", "The lab order with the given Id was not found");

                var labResult = _context.LabResults.Find(command.LabOrderId);

                if (labResult == null) 
                    return Error.NotFound("Lab result not found", "The lab result with the given Id was not found");

                LabOrder UpdateLabResult = LabOrder.UpdateLabResults(
                    labOrder,
                    labResult,
                    command.LabResultId);

                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }

    public class UpdateLabResultsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("api/laborders/labrequests/{id}",
                async (string id, [FromBody]UpdateLabResultsRequest request, [FromServices] ISender sender) =>
                {
                    if (id != request.LabOrderId.ToString())
                        return Results.BadRequest("Identification number in the URL does not match the request body.");

                    var command = request.Adapt<UpdateLabResults.Command>();

                    var result = await sender.Send(command);

                    return result.Match(
                        labOrderId => Results.Ok(labOrderId),
                        errors => Results.BadRequest(errors));
                });
        }
    }
}
