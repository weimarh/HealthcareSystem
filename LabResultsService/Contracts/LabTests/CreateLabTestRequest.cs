namespace LabResultsService.Contracts.LabTests
{
    public class CreateLabTestRequest
    {
        public string TestName { get; set; } = string.Empty;
        public string ReferenceRange { get; set; } = string.Empty;
        public int SpecimenType { get; set; }
    }
}
