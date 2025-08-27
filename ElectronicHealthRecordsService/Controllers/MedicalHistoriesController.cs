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

        public MedicalHistoriesController(IRepository<MedicalHistory> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        public async Task<IEnumerable<MedicalHistoryDto>> GetAllMedicalHistories() =>
            (await _repository.GetAllAsync()).Select(x => x.AsDto()).ToList();

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalHistoryDto>> GetMedicalHistoryById(Guid id)
        {
            var medicalHistory = await _repository.GetAsync(id);

            if (medicalHistory == null)
                return NotFound();

            return medicalHistory.AsDto();
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
