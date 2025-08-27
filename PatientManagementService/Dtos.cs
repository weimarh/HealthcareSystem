using System.ComponentModel.DataAnnotations;

namespace PatientManagementService
{
    public record PatientDto(
        int Id,
        string? Complement,
        string FirstName,
        string LastName,
        string Gender,
        string HomeAddress,
        string EmergencyContactName,
        string EmergencyContactPhone);

    public record CreatePatientDto(
        [Required] int Id,
        string? Complement,
        [Required][MaxLength(20)][MinLength(2)] string FirstName,
        [Required][MaxLength(20)][MinLength(2)] string LastName,
        [Required][RegularExpression(@"^[0-9]$", ErrorMessage = "Please enter a positive integer.")] int Gender,
        [Required][MaxLength(50)][MinLength(5)] string HomeAddress,
        [Required][MaxLength(50)][MinLength(5)] string EmergencyContactName,
        [Required][RegularExpression(@"^[1-9]\d{7}$", ErrorMessage = "Please enter a positive integer with exactly 8 digits.")] string EmergencyContactPhone);

    public record UpdatePatientDto(
        string? Complement,
        [Required][MaxLength(20)][MinLength(2)] string FirstName,
        [Required][MaxLength(20)][MinLength(2)] string LastName,
        [Required][RegularExpression(@"^[0-9]$", ErrorMessage = "Please enter a positive integer.")] int Gender,
        [Required][MaxLength(50)][MinLength(5)] string HomeAddress,
        [Required][MaxLength(50)][MinLength(5)] string EmergencyContactName,
        [Required][RegularExpression(@"^[1-9]\d{7}$", ErrorMessage = "Please enter a positive integer with exactly 8 digits.")] string EmergencyContactPhone);
}
