namespace LabResultsService.Contracts.Specimens
{
    public class UpdateSpecimenRequest
    {
        public Guid SpecimenId { get; set; }
        public int PatientId { get; set; }
        public int SpecimenType { get; set; }
    }
}
