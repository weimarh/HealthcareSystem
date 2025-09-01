using ElectronicHealthRecordsService.Entities;

namespace ElectronicHealthRecordsService
{
    public static class Extensions
    {
        public static MedicalAppointmentDto AsDto(this MedicalAppointment medicalAppointment, string complement, string firstName, string lastName, string gender)
        {
            return new MedicalAppointmentDto(
                medicalAppointment.Id,
                medicalAppointment.PatientId,
                complement,
                firstName,
                lastName,
                gender,
                medicalAppointment.ConsultationDate,
                medicalAppointment.Symptoms,
                medicalAppointment.Diagnosis,
                medicalAppointment.Treatment);
        }

        public static MedicalHistoryDto AsDto(this MedicalHistory medicalHistory, string complement, string firstName, string lastName, string gender)
        {
            return new MedicalHistoryDto(
                medicalHistory.Id,
                medicalHistory.PatientId,
                complement,
                firstName,
                lastName,
                gender,
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
