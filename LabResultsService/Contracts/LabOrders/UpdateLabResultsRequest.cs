using LabResultsService.Entities;

namespace LabResultsService.Contracts.LabOrders
{
    public class UpdateLabResultsRequest
    {
        public Guid LabOrderId { get; set; }
        public Guid? LabResultId { get; set; }
        public LabResult? LabResult { get; set; }
    }
}
