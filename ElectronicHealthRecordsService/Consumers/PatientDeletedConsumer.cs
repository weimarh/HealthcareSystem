using ElectronicHealthRecordsService.Repositories;
using MassTransit;
using PatientContracts;

namespace ElectronicHealthRecordsService.Consumers
{
    public class PatientDeletedConsumer : IConsumer<PatientDeleted>
    {
        private readonly IPatientRepository _patientRepository;

        public PatientDeletedConsumer(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        }

        public async Task Consume(ConsumeContext<PatientDeleted> context)
        {
            var message = context.Message;

            var patient = await _patientRepository.GetPatientByIdAsync(message.PatientId);

            if (patient == null)
                return;

            await _patientRepository.DeletePatientAsync(message.PatientId);
        }
    }
}
