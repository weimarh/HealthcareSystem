namespace LabManagementService.Contracts.LabResults
{
    public class LabResultResponse
    {
        public Guid LabResultId { get; set; }
        public Guid LabOrderId { get; set; }
        public int PatientId { get; set; }
        public string Complement { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string LabTestName { get; set; } = string.Empty;
        public DateTime LabStartDate { get; set; }
        public string Value { get; set; } = string.Empty;
        public DateTime ReportedDate { get; set; }
    }
}
