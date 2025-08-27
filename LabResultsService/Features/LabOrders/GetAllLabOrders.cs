using Carter;
using ErrorOr;
using LabResultsService.Contracts.LabOrders;
using LabResultsService.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabResultsService.Features.LabOrders
{
    public static class GetAllLabOrders
    {
        public record Query() : IRequest<ErrorOr<IReadOnlyList<LabOrderResponse>>>;

        internal sealed class Handler : IRequestHandler<Query, ErrorOr<IReadOnlyList<LabOrderResponse>>>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<ErrorOr<IReadOnlyList<LabOrderResponse>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var labOrders = await _context.LabOrders
                    .Select(x => new LabOrderResponse
                    {
                        LabOrderId = x.LabOrderId,
                        PatientId = x.PatientId,
                        OrderDate = x.OrderDate,
                        Status = x.Status,
                        OrderedBy = x.OrderedBy,
                    }).ToListAsync(cancellationToken);

                return labOrders;
            }
        }
    }

    public class GetAllLabOrdersEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/laborders", async ([FromServices] ISender sender) =>
            {
                var query = new GetAllLabOrders.Query();
                var result = await sender.Send(query);

                return result.Match(
                    laborders => Results.Ok(laborders),
                    errors => Results.Problem(string.Join("; ", errors.Select(e => e.Description))));
            });
        }
    }
}
