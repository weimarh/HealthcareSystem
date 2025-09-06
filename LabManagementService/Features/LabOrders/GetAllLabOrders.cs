using ErrorOr;
using LabManagementService.Contracts.LabOrders;
using LabManagementService.Entities;
using LabManagementService.Repositories;
using MediatR;

namespace LabManagementService.Features.LabOrders
{
    public static class GetAllLabOrders
    {
        public record Query() : IRequest<ErrorOr<IReadOnlyList<LabOrderResponse>>>;

        internal sealed class Handler : IRequestHandler<Query, ErrorOr<IReadOnlyList<LabOrderResponse>>>
        {
            private readonly IRepository<LabOrder> _context;
            private readonly IPatientRepository _patientRepository;

            public Handler(IRepository<LabOrder> context, IPatientRepository patientRepository)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            }

            public async Task<ErrorOr<IReadOnlyList<LabOrderResponse>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var labOrders = await _context.GetAllAsync();
                var patients = await _patientRepository.GetAllAsync();
                Patient? patient;

                var labOrderResponses = labOrders.Select(labOrderResponse =>
                {
                    if (patients == null)
                    {
                        var response = new LabOrderResponse
                        {
                            LabOrderId = labOrderResponse.LabOrderId,
                            PatientId = labOrderResponse.PatientId,
                            Complement = "",
                            FirstName = "",
                            LastName = "",
                            Gender = "",
                            OrderDate = labOrderResponse.OrderDate,
                            Status = labOrderResponse.Status,
                            OrderedBy = labOrderResponse.OrderedBy,
                        };
                        return response;
                    }

                    patient = patients.FirstOrDefault(patient => patient.Id == labOrderResponse.PatientId);

                    if (patient == null)
                    {
                        var response = new LabOrderResponse
                        {
                            LabOrderId = labOrderResponse.LabOrderId,
                            PatientId = labOrderResponse.PatientId,
                            Complement = "",
                            FirstName = "",
                            LastName = "",
                            Gender = "",
                            OrderDate = labOrderResponse.OrderDate,
                            Status = labOrderResponse.Status,
                            OrderedBy = labOrderResponse.OrderedBy,
                        };
                        return response;
                    }

                    return new LabOrderResponse
                    {
                        LabOrderId = labOrderResponse.LabOrderId,
                        PatientId = labOrderResponse.PatientId,
                        Complement = patient.Complement,
                        FirstName = patient.FirstName,
                        LastName = patient.LastName,
                        Gender = patient.Gender,
                        OrderDate = labOrderResponse.OrderDate,
                        Status = labOrderResponse.Status,
                        OrderedBy = labOrderResponse.OrderedBy,
                    };
                });

                //var labOrderResponses = labOrders
                //    .Select(x => new LabOrderResponse
                //    {
                //        LabOrderId = x.LabOrderId,
                //        PatientId = x.PatientId,
                //        OrderDate = x.OrderDate,
                //        Status = x.Status,
                //        OrderedBy = x.OrderedBy,
                //    })
                //    .ToList();

                return labOrderResponses.ToList();
            }
        }
    }
}
