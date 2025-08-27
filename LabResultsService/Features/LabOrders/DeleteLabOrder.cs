using Azure.Core;
using Carter;
using ErrorOr;
using FluentValidation;
using LabResultsService.Contracts.LabOrders;
using LabResultsService.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabResultsService.Features.LabOrders
{
    public static class DeleteLabOrder
    {
        public record Command(Guid Id) : IRequest<ErrorOr<Unit>>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }

        internal sealed class Handler : IRequestHandler<Command, ErrorOr<Unit>>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<ErrorOr<Unit>> Handle(Command command, CancellationToken cancellationToken)
            {
                var labOrder = await _context.LabOrders.FirstOrDefaultAsync(x => x.LabOrderId == command.Id);

                if (labOrder == null)
                    return Error.NotFound("Lab order not found", "The lab order with the given Id was not found");

                _context.LabOrders.Remove(labOrder);

                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }

    public class DeleteLabOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/laborders/{id}",
                async (string id, [FromBody]DeleteLabOrderRequest request, [FromServices]ISender sender) =>
                {
                    if (id != request.Id.ToString())
                        return Results.BadRequest("Identification number in the URL does not match the request body.");

                    var command = new DeleteLabOrder.Command(request.Id);

                    var result = await sender.Send(command);

                    return result.Match(
                        id => Results.Ok(id),
                        error => Results.NotFound(error));
                });
        }
    }
}
