namespace ElectronicHealthRecordsService.Entities
{
    public class MedicalHistory
    {
        public Guid Id { get; set; }
        public int PatientId { get; set; }
        public string PastIllnesses { get; set; } = string.Empty;
        public string Surgeries { get; set; } = string.Empty;
        public string Hospitalizations { get; set; } = string.Empty;
        public string Allergies { get; set; } = string.Empty;
        public string CurrentMedications { get; set; } = string.Empty;
        public string SubstanceAbuseHistory { get; set; } = string.Empty;
        public string FamilyMedicalHistory { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string Lifestyle { get; set; } = string.Empty;
    }
}
