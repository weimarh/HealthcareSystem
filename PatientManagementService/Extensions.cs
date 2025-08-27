using PatientManagementService.Entities;

namespace PatientManagementService
{
    public static class Extensions
    {
        public static PatientDto AsDto(this Patient patient)
        {
            return new PatientDto(
                patient.Id,
                patient.Complement ?? string.Empty,
                patient.FirstName,
                patient.LastName,
                patient.Gender.ToString(),
                patient.HomeAddress,
                patient.EmergencyContactName,
                patient.EmergencyContactPhone);
        }
    }
}
