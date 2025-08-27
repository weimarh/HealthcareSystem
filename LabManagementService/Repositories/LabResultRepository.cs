using LabManagementService.Database;
using LabManagementService.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabManagementService.Repositories
{
    public class LabResultRepository : IRepository<LabResult>
    {
        private readonly ApplicationDbContext _context;

        public LabResultRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IReadOnlyCollection<LabResult>> GetAllAsync() =>
            await _context.LabResults.ToListAsync();

        public async Task<LabResult?> GetAsync(Guid id) =>
            await _context.LabResults.SingleOrDefaultAsync(x => x.LabResultId == id);

        public async Task CreateAsync(LabResult entity) =>
            await _context.LabResults.AddAsync(entity);

        public void UpdateAsync(LabResult entity) =>
            _context.LabResults.Update(entity);

        public void DeleteAsync(LabResult entity) =>
            _context.LabResults.Remove(entity);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
