namespace LabManagementService.Entities
{
    public class LabResult
    {
        public LabResult() { }

        public LabResult(Guid labResultId, Guid labOrderId, LabOrder labOrder, string value, DateTime reportedDate)
        {
            LabResultId = labResultId;
            LabOrderId = labOrderId;
            LabOrder = labOrder;
            Value = value;
            ReportedDate = reportedDate;
        }

        public Guid LabResultId { get; set; }
        public Guid LabOrderId { get; set; }
        public LabOrder? LabOrder { get; set; }
        public string Value { get; set; } = string.Empty;
        public DateTime ReportedDate { get; set; }

        public static LabResult UpdateLabResult(LabResult labResult, string value, DateTime reportedDate)
        {
            labResult.Value = value;
            labResult.ReportedDate = reportedDate;

            return labResult;
        }
    }
}
