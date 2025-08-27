using PatientManagementService.Enums;

namespace PatientManagementService.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string? Complement { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public string HomeAddress { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
    }
}
