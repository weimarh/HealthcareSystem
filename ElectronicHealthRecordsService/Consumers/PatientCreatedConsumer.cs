using ElectronicHealthRecordsService.Entities;
using ElectronicHealthRecordsService.Repositories;
using MassTransit;
using PatientContracts;

namespace ElectronicHealthRecordsService.Consumers
{
    public class PatientCreatedConsumer : IConsumer<PatientCreated>
    {
        private readonly IPatientRepository _patientRepository;

        public PatientCreatedConsumer(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        }

        public async Task Consume(ConsumeContext<PatientCreated> context)
        {
            var message = context.Message;

            var patient = await _patientRepository.GetPatientByIdAsync(message.PatientId);

            if (patient != null)
                return;

            patient = new Patient
            {
                Id = message.PatientId,
                Complement = message.Complement ?? string.Empty,
                FirstName = message.FirstName,
                LastName = message.LastName,
                Gender = message.Gender,
            };

            await _patientRepository.CreatePatientAsync(patient);
        }
    }
}
