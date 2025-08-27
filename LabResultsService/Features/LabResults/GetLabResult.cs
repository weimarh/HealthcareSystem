using Carter;
using ErrorOr;
using LabResultsService.Contracts.LabResults;
using LabResultsService.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabResultsService.Features.LabResults
{
    public static class GetLabResult
    {
        public record Query(Guid Id) : IRequest<ErrorOr<LabResultResponse>>;

        internal sealed class Handler : IRequestHandler<Query, ErrorOr<LabResultResponse>>
        {
            public readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<ErrorOr<LabResultResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var labResult = await _context.LabResults.FirstOrDefaultAsync(x => x.LabResultId == request.Id);

                if (labResult == null)
                    return Error.NotFound("Lab result not found", "The lab result with the given Id number was not found.");

                var labOrder = await _context.LabOrders.FirstOrDefaultAsync(x => x.LabOrderId == labResult.LabOrderId);

                if (labOrder == null)
                    return Error.NotFound("Lab order not found", "The lab order with the given Id number was not found.");

                var labTest = await _context.LabTests.FirstOrDefaultAsync(x => x.LabTestId == labOrder.LabTestId);

                if (labTest == null)
                    return Error.NotFound("Lab test not found", "The lab test with the given Id number was not found.");

                var labResultResponse = new LabResultResponse
                {
                    LabResultId = labResult.LabResultId,
                    LabOrderId = labResult.LabOrderId,
                    PatientId = labOrder.PatientId,
                    LabTestName = labTest.TestName,
                    LabStartDate = labOrder.OrderDate,
                    Value = labResult.Value,
                    ReportedDate = labResult.ReportedDate,

                };

                return labResultResponse;
            }
        }
    }

    public class GetLabResultEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/labresults/{id}", async (string id, [FromServices] ISender sender) =>
            {
                var query = new GetLabResult.Query(Guid.Parse(id));

                var result = await sender.Send(query);

                return result.Match(
                    labResult => Results.Ok(labResult),
                    errors => Results.BadRequest(errors));
            });
        }
    }
}
