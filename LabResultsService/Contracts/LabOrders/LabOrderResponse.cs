using LabResultsService.Enums;

namespace LabResultsService.Contracts.LabOrders
{
    public class LabOrderResponse
    {
        public Guid LabOrderId { get; set; }
        public int PatientId { get; set; }
        public DateTime OrderDate { get; set; }
        public Status Status { get; set; }
        public string OrderedBy { get; set; } = string.Empty;
    }
}
