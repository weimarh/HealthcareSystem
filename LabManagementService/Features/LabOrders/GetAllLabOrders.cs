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

            public Handler(IRepository<LabOrder> context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<ErrorOr<IReadOnlyList<LabOrderResponse>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var labOrders = await _context.GetAllAsync();

                var labOrderResponses = labOrders
                    .Select(x => new LabOrderResponse
                    {
                        LabOrderId = x.LabOrderId,
                        PatientId = x.PatientId,
                        OrderDate = x.OrderDate,
                        Status = x.Status,
                        OrderedBy = x.OrderedBy,
                    })
                    .ToList();

                return labOrderResponses;
            }
        }
    }
}
