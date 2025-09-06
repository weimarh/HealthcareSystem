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
            //var medicalHistories = await _repository.GetAllAsync();
            //var patients = await _petientRepository.GetAllPatientsAsync();
            //Patient? patient;

            //var medicalHistoriesDtos = medicalHistories.Select(medicalHistory =>
            //{
            //    if (patients == null)
            //        return Ok(medicalHistory.AsDto("", "", "", "Male"));

            //    patient = patients.SingleOrDefault(patient => patient.Id == medicalHistory.PatientId);

            //    if (patient == null)
            //        return Ok(medicalHistory.AsDto("", "", "", "Male"));

            //    return Ok(medicalHistory.AsDto(patient.Complement, patient.FirstName, patient.LastName, patient.Gender));
            //});

            //return Ok(medicalHistoriesDtos);

            var medicalHistories = await _repository.GetAllAsync();
            var patients = await _petientRepository.GetAllPatientsAsync();


            var patientLookup = patients?.ToDictionary(p => p.Id) ?? new Dictionary<int, Patient>();

            var medicalHistoryDtos = medicalHistories.Select(mh =>
            {
                // Try to find the matching patient using the dictionary
                patientLookup.TryGetValue(mh.PatientId, out var patient);


                return new MedicalHistoryDto
                (
                    mh.Id,
                    mh.PatientId,
                    patient?.Complement ?? "",
                    patient?.FirstName ?? "",
                    patient?.LastName ?? "",
                    patient?.Gender ?? "",
                    mh.PastIllnesses,
                    mh.Surgeries,
                    mh.Hospitalizations,
                    mh.Allergies,
                    mh.CurrentMedications,
                    mh.SubstanceAbuseHistory,
                    mh.FamilyMedicalHistory,
                    mh.Occupation,
                    mh.Lifestyle
                );

            }).ToList();

            return Ok(medicalHistoryDtos);
        }
            

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalHistoryDto>> GetMedicalHistoryById(Guid id)
        {
            var medicalHistory = await _repository.GetAsync(id);
                       
            if (medicalHistory == null)
                return NotFound();

            var patient = await _petientRepository.GetPatientByIdAsync(medicalHistory.PatientId);

            if (patient == null)
                return Ok(medicalHistory.AsDto("", "", "", ""));

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
