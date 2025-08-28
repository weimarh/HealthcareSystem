namespace PatientContracts
{
    public record PatientCreated(
        int PatientId,
        string? Complement,
        string FirstName,
        string LastName,
        string Gender);

    public record PatientUpdated(
        int PatientId,
        string? Complement,
        string FirstName,
        string LastName,
        string Gender);

    public record PatientDeleted(int  PatientId);
}
