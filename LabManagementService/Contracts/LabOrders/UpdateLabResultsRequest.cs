using LabManagementService.Entities;

namespace LabManagementService.Contracts.LabOrders
{
    public class UpdateLabResultsRequest
    {
        public Guid LabOrderId { get; set; }
        public Guid LabResultId { get; set; }
    }
}
