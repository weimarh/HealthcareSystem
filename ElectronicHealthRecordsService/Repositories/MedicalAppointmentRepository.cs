using ElectronicHealthRecordsService.Entities;
using MongoDB.Driver;
using SharpCompress.Common;
using System.Linq.Expressions;

namespace ElectronicHealthRecordsService.Repositories
{
    public class MedicalAppointmentRepository : IRepository<MedicalAppointment>
    {
        public readonly string collectionName = "medicalAppointments";
        private readonly IMongoCollection<MedicalAppointment> dbCollection;
        private readonly FilterDefinitionBuilder<MedicalAppointment> filterBuilder = Builders<MedicalAppointment>.Filter;

        public MedicalAppointmentRepository(IMongoDatabase database)
        {
            dbCollection = database.GetCollection<MedicalAppointment>(collectionName);
        }

        public async Task<IReadOnlyCollection<MedicalAppointment>> GetAllAsync() =>
            await dbCollection.Find(filterBuilder.Empty).ToListAsync();

        public async Task<IReadOnlyCollection<MedicalAppointment>> GetAllAsync(Expression<Func<MedicalAppointment, bool>> filter) =>
            await dbCollection.Find(filter).ToListAsync();

        public async Task<MedicalAppointment> GetAsync(Guid id) =>
            await dbCollection.Find(filterBuilder.Eq(entity => entity.Id, id)).FirstOrDefaultAsync();

        public async Task<MedicalAppointment> GetAsync(Expression<Func<MedicalAppointment, bool>> filter) =>
            await dbCollection.Find(filter).FirstOrDefaultAsync();

        public async Task CreateAsync(MedicalAppointment entity) =>
            await (entity == null
                ? throw new ArgumentNullException(nameof(entity))
                : dbCollection.InsertOneAsync(entity));

        public async Task UpdateAsync(MedicalAppointment entity)
        {
            if (entity == null) 
                throw new ArgumentNullException(nameof(entity));

            FilterDefinition<MedicalAppointment> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity);
        }

        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<MedicalAppointment> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }
    }
}
