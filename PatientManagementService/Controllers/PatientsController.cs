using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PatientContracts;
using PatientManagementService.Entities;
using PatientManagementService.Enums;
using PatientManagementService.Repositories;

namespace PatientManagementService.Controllers
{
    [ApiController]
    [Route("api/patients")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientRepository _repository;
        private readonly IPublishEndpoint _publishEndpoint;

        public PatientsController(IPatientRepository repository, IPublishEndpoint publishEndpoint)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        [HttpGet]
        public async Task<IEnumerable<PatientDto>> GetAllPatients()
        {
            var patients = (await _repository.GetAllAsync()).Select(patient => patient.AsDto());
            return patients;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatientById(int id)
        {
            var patient = await _repository.GetAsync(id);

            if (patient == null)
                return NotFound();

            return patient.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<PatientDto>> CreatePatient(CreatePatientDto createPatientDto)
        {
            var patient = new Patient
            {
                Id = createPatientDto.Id,
                Complement = createPatientDto.Complement,
                FirstName = createPatientDto.FirstName,
                LastName = createPatientDto.LastName,
                Gender = (Gender)createPatientDto.Gender,
                HomeAddress = createPatientDto.HomeAddress,
                EmergencyContactName = createPatientDto.EmergencyContactName,
                EmergencyContactPhone = createPatientDto.EmergencyContactPhone,
            };

            await _repository.CreateAsync(patient);

            await _publishEndpoint.Publish(new PatientCreated(patient.Id,
                patient.Complement,
                patient.FirstName,
                patient.LastName,
                patient.Gender.ToString()));
          
            return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, patient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, UpdatePatientDto updatePatientDto)
        {
            var existingPatient = await _repository.GetAsync(id);

            if (existingPatient == null)
                return NotFound();

            existingPatient.Complement = updatePatientDto.Complement;
            existingPatient.FirstName = updatePatientDto.FirstName;
            existingPatient.LastName = updatePatientDto.LastName;
            existingPatient.Gender = (Gender)updatePatientDto.Gender;
            existingPatient.HomeAddress = updatePatientDto.HomeAddress;
            existingPatient.EmergencyContactName = updatePatientDto.EmergencyContactName;
            existingPatient.EmergencyContactPhone = updatePatientDto.EmergencyContactPhone;

            await _repository.UpdateAsync(existingPatient);

            await _publishEndpoint.Publish(new PatientUpdated(
                existingPatient.Id,
                existingPatient.Complement,
                existingPatient.FirstName,
                existingPatient.LastName,
                existingPatient.Gender.ToString()));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var existingPatient = await _repository.GetAsync(id);

            if (existingPatient == null)
                return NotFound();

            await _repository.RemoveAsync(id);

            await _publishEndpoint.Publish(new PatientDeleted(existingPatient.Id));

            return NoContent();
        }
    }
}
