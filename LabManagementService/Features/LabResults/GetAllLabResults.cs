using ErrorOr;
using LabManagementService.Contracts.LabResults;
using LabManagementService.Entities;
using LabManagementService.Repositories;
using MediatR;

namespace LabManagementService.Features.LabResults
{
    public static class GetAllLabResults
    {
        public record Query() : IRequest<ErrorOr<IReadOnlyList<LabResultResponse>>>;

        internal sealed class Handler : IRequestHandler<Query, ErrorOr<IReadOnlyList<LabResultResponse>>>
        {
            private readonly IRepository<LabResult> _context;
            private readonly IRepository<LabOrder> _labOrderContext;
            private readonly IRepository<LabTest> _labTestContext;

            public Handler(IRepository<LabResult> context, IRepository<LabOrder> labOrderContext, IRepository<LabTest> labTestContext)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _labOrderContext = labOrderContext ?? throw new ArgumentNullException(nameof(labOrderContext));
                _labTestContext = labTestContext ?? throw new ArgumentNullException(nameof(labTestContext));
            }

            public async Task<ErrorOr<IReadOnlyList<LabResultResponse>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var labResults = await _context.GetAllAsync();
                var labOrders = await _labOrderContext.GetAllAsync();
                var labTests = await _labTestContext.GetAllAsync();

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
}
