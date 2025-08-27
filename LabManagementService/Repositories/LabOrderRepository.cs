using LabManagementService.Database;
using LabManagementService.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabManagementService.Repositories
{
    public class LabOrderRepository : IRepository<LabOrder>
    {
        private readonly ApplicationDbContext _context;

        public LabOrderRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IReadOnlyCollection<LabOrder>> GetAllAsync() =>
            await _context.LabOrders.ToListAsync();

        public async Task<LabOrder?> GetAsync(Guid id) =>
            await _context.LabOrders.SingleOrDefaultAsync(x => x.LabOrderId == id);

        public async Task CreateAsync(LabOrder entity) =>
            await _context.LabOrders.AddAsync(entity);

        public void UpdateAsync(LabOrder entity) =>
            _context.LabOrders.Update(entity);

        public void DeleteAsync(LabOrder entity) =>
            _context.LabOrders.Remove(entity);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
