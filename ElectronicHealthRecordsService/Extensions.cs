using ElectronicHealthRecordsService.Entities;

namespace ElectronicHealthRecordsService
{
    public static class Extensions
    {
        public static MedicalAppointmentDto AsDto(this MedicalAppointment medicalAppointment)
        {
            return new MedicalAppointmentDto(
                medicalAppointment.Id,
                medicalAppointment.PatientId,
                medicalAppointment.ConsultationDate,
                medicalAppointment.Symptoms,
                medicalAppointment.Diagnosis,
                medicalAppointment.Treatment);
        }

        public static MedicalHistoryDto AsDto(this MedicalHistory medicalHistory)
        {
            return new MedicalHistoryDto(
                medicalHistory.Id,
                medicalHistory.PatientId,
                medicalHistory.PastIllnesses,
                medicalHistory.Surgeries,
                medicalHistory.Hospitalizations,
                medicalHistory.Allergies,
                medicalHistory.CurrentMedications,
                medicalHistory.SubstanceAbuseHistory,
                medicalHistory.FamilyMedicalHistory,
                medicalHistory.Occupation,
                medicalHistory.Lifestyle);
        }
    }
}
