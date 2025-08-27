using Microsoft.AspNetCore.Mvc;
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

        public PatientsController(IPatientRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
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

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var existingPatient = await _repository.GetAsync(id);

            if (existingPatient == null)
                return NotFound();

            await _repository.RemoveAsync(id);

            return NoContent();
        }
    }
}
