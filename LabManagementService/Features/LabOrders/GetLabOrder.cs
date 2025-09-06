using ErrorOr;
using LabManagementService.Contracts.LabOrders;
using LabManagementService.Entities;
using LabManagementService.Repositories;
using MediatR;

namespace LabManagementService.Features.LabOrders
{
    public static class GetLabOrder
    {
        public record Query(Guid Id) : IRequest<ErrorOr<LabOrderResponse>>;

        internal sealed class Handler : IRequestHandler<Query, ErrorOr<LabOrderResponse>>
        {
            private readonly IRepository<LabOrder> _context;
            private readonly IPatientRepository _patientRepository;

            public Handler(IRepository<LabOrder> context, IPatientRepository patientRepository)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            }

            public async Task<ErrorOr<LabOrderResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var labOrder = await _context.GetAsync(request.Id);

                if (labOrder == null)
                    return Error.NotFound("Lab order not found", "The lab order with the given Id number was not found.");

                var patient = await _patientRepository.GetAsync(labOrder.PatientId);

                LabOrderResponse labOrderResponse = new LabOrderResponse();

                if (patient == null)
                {
                    labOrderResponse.LabOrderId = labOrder.LabOrderId;
                    labOrderResponse.PatientId = labOrder.PatientId;
                    labOrderResponse.Complement = "";
                    labOrderResponse.FirstName = "";
                    labOrderResponse.LastName = "";
                    labOrderResponse.Gender = "";
                    labOrderResponse.OrderDate = labOrder.OrderDate;
                    labOrderResponse.Status = labOrder.Status;
                    labOrderResponse.OrderedBy = labOrder.OrderedBy;
                }
                else
                {
                    labOrderResponse.LabOrderId = labOrder.LabOrderId;
                    labOrderResponse.PatientId = labOrder.PatientId;
                    labOrderResponse.Complement = patient.Complement;
                    labOrderResponse.FirstName = patient.FirstName;
                    labOrderResponse.LastName = patient.LastName;
                    labOrderResponse.Gender = patient.Gender;
                    labOrderResponse.OrderDate = labOrder.OrderDate;
                    labOrderResponse.Status = labOrder.Status;
                    labOrderResponse.OrderedBy = labOrder.OrderedBy;
                }

                return labOrderResponse;
            }
        }
    }
}
