using System.ComponentModel.DataAnnotations;

namespace ElectronicHealthRecordsService
{
    public record MedicalAppointmentDto(
        Guid Id,
        int PatientId,
        DateTime ConsultationDate,
        string Symptoms,
        string Diagnosis,
        string Treatment);

    public record CreateMedicalAppointmentDto(
        int PatientId,
        DateTime ConsultationDate,
        string Symptoms,
        string Diagnosis,
        string Treatment);

    public record UpdateMedicalAppointmentDto(
        DateTime ConsultationDate,
        string Symptoms,
        string Diagnosis,
        string Treatment);

    public record MedicalHistoryDto(
        Guid Id,
        int PatientId,
        string PastIllnesses,
        string Surgeries,
        string Hospitalizations,
        string Allergies,
        string CurrentMedications,
        string SubstanceAbuseHistory,
        string FamilyMedicalHistory,
        string Occupation,
        string Lifestyle);

    public record CreateMedicalHistoryDto(
        [Required]int PatientId,
        string PastIllnesses,
        string Surgeries,
        string Hospitalizations,
        string Allergies,
        string CurrentMedications,
        string SubstanceAbuseHistory,
        string FamilyMedicalHistory,
        [Required]string Occupation,
        [Required]string Lifestyle);

    public record UpdateMedicalHistoryDto(
        string PastIllnesses,
        string Surgeries,
        string Hospitalizations,
        string Allergies,
        string CurrentMedications,
        string SubstanceAbuseHistory,
        string FamilyMedicalHistory,
        [Required]string Occupation,
        [Required]string Lifestyle);
}
