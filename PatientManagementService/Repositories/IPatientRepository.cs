using PatientManagementService.Entities;

namespace PatientManagementService.Repositories
{
    public interface IPatientRepository
    {
        Task CreateAsync(Patient patient);
        Task<IReadOnlyCollection<Patient>> GetAllAsync();
        Task<Patient> GetAsync(int id);
        Task RemoveAsync(int id);
        Task UpdateAsync(Patient patient);
    }
}
