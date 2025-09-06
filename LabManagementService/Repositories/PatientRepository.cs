using LabManagementService.Database;
using LabManagementService.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabManagementService.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IReadOnlyCollection<Patient>> GetAllAsync() =>
            await _context.Patients.ToListAsync();

        public async Task<Patient?> GetAsync(int id) =>
            await _context.Patients.SingleOrDefaultAsync(x => x.Id == id);

        public async Task CreateAsync(Patient entity) =>
            await _context.Patients.AddAsync(entity);

        public void UpdateAsync(Patient entity) =>
            _context.Patients.Update(entity);

        public async Task DeleteAsync(int id)
        {
            var patient = await _context.Patients.SingleOrDefaultAsync(x => x.Id == id);

            if (patient == null)
                throw new ArgumentNullException($"Patient {id}");

            _context.Patients.Remove(patient);
        }

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
