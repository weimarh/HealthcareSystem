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

            public Handler(IRepository<LabOrder> context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<ErrorOr<LabOrderResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var labOrder = await _context.GetAsync(request.Id);

                if (labOrder == null)
                    return Error.NotFound("Lab order not found", "The lab order with the given Id number was not found.");

                var labOrderResponse = new LabOrderResponse
                {
                    LabOrderId = labOrder.LabOrderId,
                    PatientId = labOrder.PatientId,
                    OrderDate = labOrder.OrderDate,
                    Status = labOrder.Status,
                    OrderedBy = labOrder.OrderedBy,
                };

                return labOrderResponse;
            }
        }
    }
}
