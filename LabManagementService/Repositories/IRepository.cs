namespace LabManagementService.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IReadOnlyCollection<T>> GetAllAsync();
        Task<T?> GetAsync(Guid id);
        Task CreateAsync(T entity);
        void UpdateAsync(T entity);
        void DeleteAsync(T entity);
        Task SaveChangesAsync();
    }
}
