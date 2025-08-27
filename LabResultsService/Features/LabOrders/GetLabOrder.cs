using Carter;
using ErrorOr;
using LabResultsService.Contracts.LabOrders;
using LabResultsService.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabResultsService.Features.LabOrders
{
    public static class GetLabOrder
    {
        public record Query(Guid Id) : IRequest<ErrorOr<LabOrderResponse>>;

        internal sealed class Handler : IRequestHandler<Query, ErrorOr<LabOrderResponse>>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<ErrorOr<LabOrderResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var labOrderResponse = await _context.LabOrders
                    .Where(x => x.LabOrderId == request.Id)
                    .Select(x => new LabOrderResponse
                    {
                        LabOrderId = x.LabOrderId,
                        PatientId = x.PatientId,
                        OrderDate = x.OrderDate,
                        Status = x.Status,
                        OrderedBy = x.OrderedBy,
                    }).FirstOrDefaultAsync(cancellationToken);

                if (labOrderResponse == null)
                    return Error.NotFound("Lab order not found", "The lab order with the given Id number was not found.");

                return labOrderResponse;
            }
        }
    }

    public class GetLabOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/laborders/{id}", async (string id, [FromServices]ISender sender) =>
            {
                var query = new GetLabOrder.Query(Guid.Parse(id));
                var result = await sender.Send(query);

                return result.Match(
                    laborder => Results.Ok(laborder), 
                    error => Results.NotFound(error));
            });
        }
    }
}
