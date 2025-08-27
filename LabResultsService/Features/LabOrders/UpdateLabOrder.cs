using Carter;
using ErrorOr;
using FluentValidation;
using LabResultsService.Contracts.LabOrders;
using LabResultsService.Database;
using LabResultsService.Entities;
using LabResultsService.Enums;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabResultsService.Features.LabOrders
{
    public static class UpdateLabOrder
    {
        public record Command(
            Guid LabOrderId,
            int PatientId,
            Guid LabTestId,
            DateTime OrderDate,
            int Status,
            string OrderedBy
        ) : IRequest<ErrorOr<Unit>>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.LabOrderId).NotEmpty();
                RuleFor(x => x.PatientId).NotEmpty();
                RuleFor(x => x.LabTestId).NotEmpty();
                RuleFor(x => x.OrderDate).NotEmpty();
                RuleFor(x => x.Status).NotEmpty();
                RuleFor(x => x.OrderedBy).NotEmpty();
            }
        }

        internal sealed class Handler : IRequestHandler<Command, ErrorOr<Unit>>
        {
            private readonly ApplicationDbContext _context;
            private readonly IValidator<Command> _validator;

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

                var labOrder = _context.LabOrders.Find(command.LabOrderId);

                if (labOrder == null)
                    return Error.NotFound("Lab order not found", "The lab order with the given Id was not found");

                var labTest = await _context.LabTests
                    .FirstOrDefaultAsync(labTest => labTest.LabTestId == command.LabTestId, cancellationToken);

                if (labTest == null)
                    return Error.NotFound("Lab Test not found", "The specified Lab Test does not exist.");

                LabOrder updatedLabOrder = LabOrder.UpdateLabOrder(
                    labOrder,
                    labTest,
                    command.LabTestId,
                    command.OrderDate,
                    (Status)command.Status,
                    command.OrderedBy);

                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }

    public class UpdateLabOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("api/laborders/{id}",
                async (string id, [FromBody]UpdateLabOrderRequest request, [FromServices] ISender sender) =>
                {
                    if (id != request.LabOrderId.ToString())
                        return Results.BadRequest("Identification number in the URL does not match the request body.");

                    var command = request.Adapt<UpdateLabOrder.Command>();

                    var result = await sender.Send(command);

                    return result.Match(
                        labOrderId => Results.Ok(labOrderId), 
                        errors => Results.BadRequest(errors));
                });
        }
    }
}
