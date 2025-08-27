using LabManagementService.Enums;

namespace LabManagementService.Contracts.LabTests
{
    public class LabTestResponse
    {
        public Guid LabTestId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string ReferenceRange { get; set; } = string.Empty;
        public SpecimenType SpecimenType { get; set; }
    }
}
