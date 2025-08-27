using ElectronicHealthRecordsService.Entities;
using MongoDB.Driver;
using SharpCompress.Common;
using System.Linq.Expressions;

namespace ElectronicHealthRecordsService.Repositories
{
    public class MedicalHistoryRepository : IRepository<MedicalHistory>
    {
        private readonly string collectionName = "medicalHistories";
        private readonly IMongoCollection<MedicalHistory> dbCollection;
        private readonly FilterDefinitionBuilder<MedicalHistory> filterBuilder = Builders<MedicalHistory>.Filter;

        public MedicalHistoryRepository(IMongoDatabase database)
        {
            dbCollection = database.GetCollection<MedicalHistory>(collectionName);
        }

        public async Task<IReadOnlyCollection<MedicalHistory>> GetAllAsync() => 
            await dbCollection.Find(filterBuilder.Empty).ToListAsync();

        public async Task<IReadOnlyCollection<MedicalHistory>> GetAllAsync(Expression<Func<MedicalHistory, bool>> filter) => 
            await dbCollection.Find(filter).ToListAsync();

        public async Task<MedicalHistory> GetAsync(Guid id) =>
            await dbCollection.Find(filterBuilder.Eq(entity => entity.Id, id)).FirstOrDefaultAsync();

        public async Task<MedicalHistory> GetAsync(Expression<Func<MedicalHistory, bool>> filter) =>
            await dbCollection.Find(filter).FirstOrDefaultAsync();

        public async Task CreateAsync(MedicalHistory entity) =>
            await (entity == null
                ? throw new ArgumentNullException(nameof(entity))
                : dbCollection.InsertOneAsync(entity));

        public async Task UpdateAsync(MedicalHistory entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            FilterDefinition<MedicalHistory> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity);
        }

        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<MedicalHistory> filter = filterBuilder.Eq(entity => entity.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }
    }
}
