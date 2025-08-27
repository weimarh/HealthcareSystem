using LabResultsService.Enums;

namespace LabResultsService.Contracts.Specimens
{
    public class SpecimenResponse
    {
        public Guid SpecimenId { get; set; }
        public int PatientId { get; set; }
        public SpecimenType SpecimenType { get; set; }
    }
}
