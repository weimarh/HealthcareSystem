namespace LabManagementService.Contracts.LabResults
{
    public class LabResultResponse
    {
        public Guid LabResultId { get; set; }
        public Guid LabOrderId { get; set; }
        public int PatientId { get; set; }
        public string LabTestName { get; set; } = string.Empty;
        public DateTime LabStartDate { get; set; }
        public string Value { get; set; } = string.Empty;
        public DateTime ReportedDate { get; set; }
    }
}
