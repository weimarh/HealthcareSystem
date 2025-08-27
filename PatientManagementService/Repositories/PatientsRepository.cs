using MongoDB.Driver;
using PatientManagementService.Entities;

namespace PatientManagementService.Repositories
{
    public class PatientsRepository : IPatientRepository
    {
        private const string collectionName = "patients";
        private readonly IMongoCollection<Patient> dbCollection;
        private readonly FilterDefinitionBuilder<Patient> filterBuilder = Builders<Patient>.Filter;

        public PatientsRepository(IMongoDatabase database)
        {
            dbCollection = database.GetCollection<Patient>(collectionName);
        }

        public async Task<IReadOnlyCollection<Patient>> GetAllAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<Patient> GetAsync(int id)
        {
            FilterDefinition<Patient> filter = filterBuilder.Eq(patient => patient.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient));

            await dbCollection.InsertOneAsync(patient);
        }

        public async Task UpdateAsync(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient));

            FilterDefinition<Patient> filter = filterBuilder.Eq(patient => patient.Id, patient.Id);
            await dbCollection.ReplaceOneAsync(filter, patient);
        }

        public async Task RemoveAsync(int id)
        {
            FilterDefinition<Patient> filter = filterBuilder.Eq(patient => patient.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }
    }
}
