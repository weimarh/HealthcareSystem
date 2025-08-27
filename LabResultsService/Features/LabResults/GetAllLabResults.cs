using Carter;
using ErrorOr;
using LabResultsService.Contracts.LabResults;
using LabResultsService.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabResultsService.Features.LabResults
{
    public static class GetAllLabResults
    {
        public record Query() : IRequest<ErrorOr<IReadOnlyList<LabResultResponse>>>;

        internal sealed class Handler : IRequestHandler<Query, ErrorOr<IReadOnlyList<LabResultResponse>>>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<ErrorOr<IReadOnlyList<LabResultResponse>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var labResults = await _context.LabResults.ToListAsync(cancellationToken);
                var labOrders = await _context.LabOrders.ToListAsync(cancellationToken);
                var labTests = await _context.LabTests.ToListAsync(cancellationToken);

                var labResultsResponse = labResults.Select(x =>
                {
                    var labOrder = labOrders.FirstOrDefault(order => order.LabOrderId == x.LabOrderId);
                    var labTest = labTests.FirstOrDefault(test => test.LabTestId == labOrder?.LabTestId);

                    return new LabResultResponse
                    {
                        LabResultId = x.LabResultId,
                        LabOrderId = x.LabOrderId,
                        PatientId = labOrder?.PatientId ?? 0, // Default to 0 if null
                        LabTestName = labTest?.TestName ?? "Unknown", // Default to "Unknown" if null
                        LabStartDate = labOrder?.OrderDate ?? DateTime.MinValue, // Default to DateTime.MinValue if null
                        Value = x.Value,
                        ReportedDate = x.ReportedDate
                    };
                }).ToList();

                return labResultsResponse;
            }
        }
    }

    public class GetAllLabResultsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/labresults", async ([FromServices] ISender sender) =>
            {
                var query = new GetAllLabResults.Query();
                var result = await sender.Send(query);

                return result.Match(
                    labResults => Results.Ok(labResults),
                    errors => Results.BadRequest(errors));
            });
        }
    }
}
