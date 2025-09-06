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
            private readonly IPatientRepository _patientRepository;

            public Handler(IRepository<LabResult> context, IRepository<LabOrder> labOrderContext, IRepository<LabTest> labTestContext, IPatientRepository patientRepository)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _labOrderContext = labOrderContext ?? throw new ArgumentNullException(nameof(labOrderContext));
                _labTestContext = labTestContext ?? throw new ArgumentNullException(nameof(labTestContext));
                _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            }

            public async Task<ErrorOr<IReadOnlyList<LabResultResponse>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var labResults = await _context.GetAllAsync();
                var labOrders = await _labOrderContext.GetAllAsync();
                var labTests = await _labTestContext.GetAllAsync();
                var patients = await _patientRepository.GetAllAsync();

                var labResultsResponse = labResults.Select(labResultResponse =>
                {
                    var labOrder = labOrders.FirstOrDefault(order => order.LabOrderId == labResultResponse.LabOrderId);
                    var labTest = labTests.FirstOrDefault(test => test.LabTestId == labOrder?.LabTestId);

                    Patient? patient = patients?.FirstOrDefault(x => x.Id == labOrder?.PatientId);

                    return new LabResultResponse
                    {
                        LabResultId = labResultResponse.LabResultId,
                        LabOrderId = labResultResponse.LabOrderId,
                        PatientId = labOrder?.PatientId ?? 0,
                        Complement = patient?.Complement ?? "",
                        FirstName = patient?.FirstName ?? "",
                        LastName = patient?.LastName ?? "",
                        Gender = patient?.Gender ?? "",
                        LabTestName = labTest?.TestName ?? "Unknown",
                        LabStartDate = labOrder?.OrderDate ?? DateTime.MinValue,
                        Value = labResultResponse.Value,
                        ReportedDate = labResultResponse.ReportedDate,
                    };
                }).ToList();

                return labResultsResponse;
            }
        }
    }
}
