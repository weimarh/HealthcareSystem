namespace LabManagementService.Contracts.LabTests
{
    public class UpdateLabTestRequest
    {
        public Guid LabTestId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string ReferenceRange { get; set; } = string.Empty;
        public int SpecimenType { get; set; }
    }
}
