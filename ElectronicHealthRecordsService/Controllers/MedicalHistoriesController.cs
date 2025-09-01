using ElectronicHealthRecordsService.Entities;
using ElectronicHealthRecordsService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicHealthRecordsService.Controllers
{
    [ApiController]
    [Route("api/medicalhistories")]
    public class MedicalHistoriesController : ControllerBase
    {
        private readonly IRepository<MedicalHistory> _repository;
        private readonly IPatientRepository _petientRepository;

        public MedicalHistoriesController(IRepository<MedicalHistory> repository, IPatientRepository petientRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _petientRepository = petientRepository ?? throw new ArgumentNullException(nameof(petientRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalHistoryDto>>> GetAllMedicalHistories()
        {
            var medicalHistories = await _repository.GetAllAsync();
            var patients = await _petientRepository.GetAllPatientsAsync();

            var medicalHistoriesDtos = medicalHistories.Select(medicalHistory =>
            {
                var patient = patients.Single(patient => patient.Id == medicalHistory.PatientId);
                return medicalHistory.AsDto(patient.Complement, patient.FirstName, patient.LastName, patient.Gender);
            });

            return Ok(medicalHistoriesDtos);
        }
            

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalHistoryDto>> GetMedicalHistoryById(Guid id)
        {
            var medicalHistory = await _repository.GetAsync(id);
                       
            if (medicalHistory == null)
                return NotFound();

            var patient = await _petientRepository.GetPatientByIdAsync(medicalHistory.PatientId);

            return Ok(medicalHistory.AsDto(patient.Complement, patient.FirstName, patient.LastName, patient.Gender));
        }

        [HttpPost]
        public async Task<ActionResult<MedicalHistoryDto>> CreateMedicalHistory(CreateMedicalHistoryDto medicalHistoryDto)
        {
            var medicalHistory = new MedicalHistory
            {
                Id = Guid.NewGuid(),
                PatientId = medicalHistoryDto.PatientId,
                PastIllnesses = medicalHistoryDto.PastIllnesses,
                Surgeries = medicalHistoryDto.Surgeries,
                Hospitalizations = medicalHistoryDto.Hospitalizations,
                Allergies = medicalHistoryDto.Allergies,
                CurrentMedications = medicalHistoryDto.CurrentMedications,
                SubstanceAbuseHistory = medicalHistoryDto.SubstanceAbuseHistory,
                FamilyMedicalHistory = medicalHistoryDto.FamilyMedicalHistory,
                Occupation = medicalHistoryDto.Occupation,
                Lifestyle = medicalHistoryDto.Lifestyle,
            };

            await _repository.CreateAsync(medicalHistory);

            return CreatedAtAction(nameof(GetMedicalHistoryById), new { id = medicalHistory.Id }, medicalHistory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedicalHistory(Guid id, UpdateMedicalHistoryDto updateMedicalHistoryDto)
        {
            var existingMedicalHistory = await _repository.GetAsync(id);

            if (existingMedicalHistory == null)
                return NotFound();

            existingMedicalHistory.PastIllnesses = updateMedicalHistoryDto.PastIllnesses;
            existingMedicalHistory.Surgeries = updateMedicalHistoryDto.Surgeries;
            existingMedicalHistory.Hospitalizations = updateMedicalHistoryDto.Hospitalizations;
            existingMedicalHistory.Allergies = updateMedicalHistoryDto.Allergies;
            existingMedicalHistory.CurrentMedications = updateMedicalHistoryDto.CurrentMedications;
            existingMedicalHistory.SubstanceAbuseHistory = updateMedicalHistoryDto.SubstanceAbuseHistory;
            existingMedicalHistory.FamilyMedicalHistory = updateMedicalHistoryDto.FamilyMedicalHistory;
            existingMedicalHistory.Occupation = updateMedicalHistoryDto.Occupation;
            existingMedicalHistory.Lifestyle = updateMedicalHistoryDto.Lifestyle;

            await _repository.UpdateAsync(existingMedicalHistory);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalHistory(Guid id)
        {
            var existingMedicalHistory = await _repository.GetAsync(id);

            if (existingMedicalHistory == null)
                return NotFound();

            await _repository.RemoveAsync(id);

            return NoContent();
        }
    }
}
