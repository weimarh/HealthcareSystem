using LabManagementService.Database;
using LabManagementService.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabManagementService.Repositories
{
    public class LabTestRepository : IRepository<LabTest>
    {
        private readonly ApplicationDbContext _context;

        public LabTestRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IReadOnlyCollection<LabTest>> GetAllAsync() =>
            await _context.LabTests.ToListAsync();

        public async Task<LabTest?> GetAsync(Guid id) =>
            await _context.LabTests.SingleOrDefaultAsync(x => x.LabTestId == id);

        public async Task CreateAsync(LabTest entity) =>
            await _context.LabTests.AddAsync(entity);

        public void UpdateAsync(LabTest entity) =>
            _context.LabTests.Update(entity);

        public void DeleteAsync(LabTest entity) =>
            _context.LabTests.Remove(entity);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
