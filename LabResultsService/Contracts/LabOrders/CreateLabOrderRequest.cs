namespace LabResultsService.Contracts.LabOrders
{
    public class CreateLabOrderRequest
    {
        public int PatientId { get; set; }
        public Guid LabTestId { get; set; }
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public string OrderedBy { get; set; } = string.Empty;
    }
}
