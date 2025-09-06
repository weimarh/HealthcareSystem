using LabManagementService.Entities;
using LabManagementService.Repositories;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PatientContracts;

namespace LabManagementService.Consumers
{
    public class PatientCreatedConsumer : IConsumer<PatientCreated>
    {
        private readonly IPatientRepository _patientRepository;
        ILogger<PatientCreatedConsumer> _logger;

        public PatientCreatedConsumer(IPatientRepository patientRepository, ILogger<PatientCreatedConsumer> logger)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PatientCreated> context)
        {
            var message = context.Message;

            var patient = await _patientRepository.GetAsync(message.PatientId);

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

            await _patientRepository.CreateAsync(patient);
            
            try
            {
                await _patientRepository.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                // Handle specific SQL errors
                if (sqlEx.Number == 2627) // Primary key violation
                {
                    _logger.LogWarning("Duplicate patient ID: {PatientId}", context.Message.PatientId);
                    // Consider updating existing record instead
                }
                else if (sqlEx.Number == 547) // Foreign key violation
                {
                    _logger.LogError("Foreign key violation: {Message}", sqlEx.Message);
                }
                throw;
            }
            
        }
    }
}
