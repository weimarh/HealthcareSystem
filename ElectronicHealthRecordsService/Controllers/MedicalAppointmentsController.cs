using ElectronicHealthRecordsService.Entities;
using ElectronicHealthRecordsService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicHealthRecordsService.Controllers
{
    [ApiController]
    [Route("api/medicalappointments")]
    public class MedicalAppointmentsController : ControllerBase
    {
        private readonly IRepository<MedicalAppointment> _repository;
        private readonly IPatientRepository _petientRepository;
        public MedicalAppointmentsController(IRepository<MedicalAppointment> repository, IPatientRepository petientRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _petientRepository = petientRepository ?? throw new ArgumentNullException(nameof(petientRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalAppointmentDto>>> GetAllMedicalAppointments()
        {
            var medicalAppointments = await _repository.GetAllAsync();
            var patients = await _petientRepository.GetAllPatientsAsync();

            var patientLookup = patients?.ToDictionary(p => p.Id) ?? new Dictionary<int, Patient>();

            var medicalAppointmentsDtos = medicalAppointments.Select(ma =>
            {

                patientLookup.TryGetValue(ma.PatientId, out var patient);

                return new MedicalAppointmentDto
                (
                    ma.Id,
                    ma.PatientId,
                    patient?.Complement ?? "",
                    patient?.FirstName ?? "",
                    patient?.LastName ?? "",
                    patient?.Gender ?? "",
                    ma.ConsultationDate,
                    ma.Symptoms,
                    ma.Diagnosis,
                    ma.Treatment
                );
                //var patient = patients.SingleOrDefault(patient => patient.Id == medicalAppointment.PatientId);

                //if (patient == null)
                //    return Ok(medicalAppointment.AsDto("", "", "", "Male"));

                //return Ok(medicalAppointment.AsDto(patient.Complement, patient.FirstName, patient.LastName, patient.Gender));
            }).ToList();

            return Ok(medicalAppointmentsDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalAppointmentDto>> GetMedicalAppointmentById(Guid id)
        {
            var medicalAppointment = await _repository.GetAsync(id);

            if (medicalAppointment == null)
                return NotFound();

            var patient = await _petientRepository.GetPatientByIdAsync(medicalAppointment.PatientId);

            if (patient == null)
                return Ok(medicalAppointment.AsDto("", "", "", ""));

            return Ok(medicalAppointment.AsDto(patient.Complement, patient.FirstName, patient.LastName, patient.Gender));
        }

        [HttpPost]
        public async Task<ActionResult<MedicalAppointmentDto>> CreateMedicalAppointment(CreateMedicalAppointmentDto createMedicalAppointmentDto)
        {
            var medicalAppointment = new MedicalAppointment()
            {
                Id = Guid.NewGuid(),
                PatientId = createMedicalAppointmentDto.PatientId,
                ConsultationDate = createMedicalAppointmentDto.ConsultationDate,
                Symptoms = createMedicalAppointmentDto.Symptoms,
                Diagnosis = createMedicalAppointmentDto.Diagnosis,
                Treatment = createMedicalAppointmentDto.Treatment,
            };

            await _repository.CreateAsync(medicalAppointment);

            return CreatedAtAction(nameof(GetMedicalAppointmentById), new { id = medicalAppointment.Id }, medicalAppointment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedicalAppointment(Guid id, UpdateMedicalAppointmentDto updateMedicalAppointmentDto)
        {
            var medicalAppointment = await _repository.GetAsync(id);

            if (medicalAppointment == null)
                return NotFound();

            medicalAppointment.ConsultationDate = updateMedicalAppointmentDto.ConsultationDate;
            medicalAppointment.Symptoms = updateMedicalAppointmentDto.Symptoms;
            medicalAppointment.Diagnosis = updateMedicalAppointmentDto.Diagnosis;
            medicalAppointment.Treatment = updateMedicalAppointmentDto.Treatment;

            await _repository.UpdateAsync(medicalAppointment);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalAppointment(Guid id)
        {
            var medicalAppointment = await _repository.GetAsync(id);

            if (medicalAppointment == null)
                return NotFound();

            await _repository.RemoveAsync(id);

            return NoContent();
        }
    }
}
