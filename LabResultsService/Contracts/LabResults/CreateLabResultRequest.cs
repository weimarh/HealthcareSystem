namespace LabResultsService.Contracts.LabResults
{
    public class CreateLabResultRequest
    {
        public Guid LabOrderId { get; set; }
        public string Value { get; set; } = string.Empty;
        public DateTime ReportedDate { get; set; }
    }
}
