using LabManagementService.Entities;

namespace LabManagementService.Repositories
{
    public interface IPatientRepository
    {
        Task<IReadOnlyCollection<Patient>> GetAllAsync();
        Task<Patient?> GetAsync(int id);
        Task CreateAsync(Patient entity);
        void UpdateAsync(Patient entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}
