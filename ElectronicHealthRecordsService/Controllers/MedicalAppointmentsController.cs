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
        public MedicalAppointmentsController(IRepository<MedicalAppointment> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        public async Task<IEnumerable<MedicalAppointmentDto>> GetAllMedicalAppointments() =>
            (await _repository.GetAllAsync()).Select(x => x.AsDto()).ToList();

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalAppointmentDto>> GetMedicalAppointmentById(Guid id)
        {
            var medicalAppointment = await _repository.GetAsync(id);

            if (medicalAppointment == null)
                return NotFound();

            return medicalAppointment.AsDto();
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
