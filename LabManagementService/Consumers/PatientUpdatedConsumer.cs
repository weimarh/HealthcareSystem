using LabManagementService.Entities;
using LabManagementService.Repositories;
using MassTransit;
using PatientContracts;

namespace LabManagementService.Consumers
{
    public class PatientUpdatedConsumer : IConsumer<PatientUpdated>
    {
        private readonly IPatientRepository _patientRepository;

        public PatientUpdatedConsumer(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        }

        public async Task Consume(ConsumeContext<PatientUpdated> context)
        {
            var message = context.Message;

            var patient = await _patientRepository.GetAsync(message.PatientId);

            if (patient == null)
            {
                patient = new Patient
                {
                    Id = message.PatientId,
                    Complement = message.Complement ?? string.Empty,
                    FirstName = message.FirstName,
                    LastName = message.LastName,
                    Gender = message.Gender,
                };

                await _patientRepository.CreateAsync(patient);
            }
            else
            {
                patient.Complement = message.Complement ?? string.Empty;
                patient.FirstName = message.FirstName;
                patient.LastName = message.LastName;
                patient.Gender = message.Gender;
            }

            _patientRepository.UpdateAsync(patient);

            await _patientRepository.SaveChangesAsync();
        }
    }
}
