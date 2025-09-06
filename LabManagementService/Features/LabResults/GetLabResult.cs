using ErrorOr;
using LabManagementService.Contracts.LabResults;
using LabManagementService.Entities;
using LabManagementService.Repositories;
using MediatR;

namespace LabManagementService.Features.LabResults
{
    public static class GetLabResult
    {
        public record Query(Guid Id) : IRequest<ErrorOr<LabResultResponse>>;

        internal sealed class Handler : IRequestHandler<Query, ErrorOr<LabResultResponse>>
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

            public async Task<ErrorOr<LabResultResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var labResult = await _context.GetAsync(request.Id);

                if (labResult == null)
                    return Error.NotFound("Lab result not found", "The lab result with the given Id number was not found.");

                var labOrder = await _labOrderContext.GetAsync(labResult.LabOrderId);

                if (labOrder == null)
                    return Error.NotFound("Lab order not found", "The lab order with the given Id number was not found.");

                var labTest = await _labTestContext.GetAsync(labOrder.LabTestId);

                if (labTest == null)
                    return Error.NotFound("Lab test not found", "The lab test with the given Id number was not found.");

                var patient = await _patientRepository.GetAsync(labOrder.PatientId);

                var labResultResponse = new LabResultResponse();

                if (patient == null)
                {
                    labResultResponse.LabResultId = labResult.LabOrderId;
                    labResultResponse.LabOrderId = labResult.LabOrderId;
                    labResultResponse.PatientId = labOrder.PatientId;
                    labResultResponse.Complement = "";
                    labResultResponse.FirstName = "";
                    labResultResponse.LastName = "";
                    labResultResponse.Gender = "";
                    labResultResponse.LabTestName = labTest.TestName;
                    labResultResponse.LabStartDate = labOrder.OrderDate;
                    labResultResponse.Value = labResult.Value;
                    labResultResponse.ReportedDate = labResult.ReportedDate;
                }
                else
                {
                    labResultResponse.LabResultId = labResult.LabOrderId;
                    labResultResponse.LabOrderId = labResult.LabOrderId;
                    labResultResponse.PatientId = labOrder.PatientId;
                    labResultResponse.Complement = patient.Complement;
                    labResultResponse.FirstName = patient.FirstName;
                    labResultResponse.LastName = patient.LastName;
                    labResultResponse.Gender = patient.Gender;
                    labResultResponse.LabTestName = labTest.TestName;
                    labResultResponse.LabStartDate = labOrder.OrderDate;
                    labResultResponse.Value = labResult.Value;
                    labResultResponse.ReportedDate = labResult.ReportedDate;
                }

                return labResultResponse;
            }
        }
    }
}
