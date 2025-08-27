namespace LabResultsService.Contracts.Specimens
{
    public class CreateSpecimenRequest
    {
        public int PatientId { get; set; }
        public int SpecimenType { get; set; }
    }
}
