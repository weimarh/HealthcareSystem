namespace LabManagementService.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string Complement { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
    }
}
