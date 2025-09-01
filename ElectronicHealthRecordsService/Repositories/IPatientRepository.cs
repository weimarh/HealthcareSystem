using ElectronicHealthRecordsService.Entities;

namespace ElectronicHealthRecordsService.Repositories
{
    public interface IPatientRepository
    {
        Task<IReadOnlyCollection<Patient>> GetAllPatientsAsync();
        Task<Patient> GetPatientByIdAsync(int id);
        Task CreatePatientAsync(Patient patient);
        Task UpdatePatientAsync(Patient patient);
        Task DeletePatientAsync(int id);
    }
}
