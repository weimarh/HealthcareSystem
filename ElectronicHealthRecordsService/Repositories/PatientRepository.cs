using ElectronicHealthRecordsService.Entities;
using MongoDB.Driver;

namespace ElectronicHealthRecordsService.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly string collectionName = "patients";
        private readonly IMongoCollection<Patient> dbCollection;
        private readonly FilterDefinitionBuilder<Patient> filterBuilder = Builders<Patient>.Filter;

        public PatientRepository(IMongoDatabase database)
        {
            dbCollection = database.GetCollection<Patient>(collectionName);
        }

        public async Task<IReadOnlyCollection<Patient>> GetAllPatientsAsync() =>
            await dbCollection.Find(filterBuilder.Empty).ToListAsync();

        public async Task<Patient> GetPatientByIdAsync(int id) =>
            await dbCollection.Find(filterBuilder.Eq(patient => patient.Id, id)).FirstOrDefaultAsync();

        public async Task CreatePatientAsync(Patient patient) =>
            await (patient == null
            ? throw new ArgumentNullException(nameof(patient))
            : dbCollection.InsertOneAsync(patient));

        public async Task UpdatePatientAsync(Patient patient)
        {
            if (patient == null) 
                throw new ArgumentNullException(nameof(patient));

            FilterDefinition<Patient> filter = filterBuilder.Eq(existingPatient => existingPatient.Id, patient.Id);
            await dbCollection.ReplaceOneAsync(filter, patient);
        }

        public async Task DeletePatientAsync(int id)
        {
            FilterDefinition<Patient> filter = filterBuilder.Eq(existingPatient => existingPatient.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }
    }
}
