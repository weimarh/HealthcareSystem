namespace LabResultsService.Contracts.LabResults
{
    public class UpdateLabResultRequest
    {
        public Guid LabResultId { get; set; }
        public string Value { get; set; } = string.Empty;
        public DateTime ReportedDate { get; set; }
    }
}
