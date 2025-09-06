using ElectronicHealthRecordsService;
using ElectronicHealthRecordsService.Controllers;
using ElectronicHealthRecordsService.Entities;
using ElectronicHealthRecordsService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.ElectronicHealthRecordsServiceTests.MedicalHistoryTests
{
    public class GetMedicalHistoryByIdTests
    {
        private readonly Mock<IRepository<MedicalHistory>> _mockRepo;
        private readonly MedicalHistoriesController _controller;
        private readonly Mock<IPatientRepository> _mockPatientRepository;

        public GetMedicalHistoryByIdTests()
        {
            _mockRepo = new Mock<IRepository<MedicalHistory>>();
            _mockPatientRepository = new Mock<IPatientRepository>();
            _controller = new MedicalHistoriesController(_mockRepo.Object, _mockPatientRepository.Object);
        }

        [Fact]
        public async Task GetMedicalHistoryById_ReturnsMedicalHistoryDto_WhenMedicalHistoryExistsWithMatchingPatients()
        {
            //Arrange
            var medicalHistoryId = new Guid("9c0de042-8c1b-4b58-9cec-0cd2819e518a");
            var medicalHistory = new MedicalHistory
            {
                Id = medicalHistoryId,
                PatientId = 5207907,
                PastIllnesses = "test1",
                Surgeries = "test",
                Hospitalizations = "test",
                Allergies = "test",
                CurrentMedications = "test",
                SubstanceAbuseHistory = "test",
                FamilyMedicalHistory = "test",
                Occupation = "test",
                Lifestyle = "test",
            };

            var patient = new Patient
            {
                Id = 5207907,
                Complement = "1-A",
                FirstName = "Weimar",
                LastName = "Barea",
                Gender = "Male",
            };

            _mockRepo.Setup(repo => repo.GetAsync(new Guid("9c0de042-8c1b-4b58-9cec-0cd2819e518a"))).ReturnsAsync(medicalHistory);
            _mockPatientRepository.Setup(repo => repo.GetPatientByIdAsync(patient.Id)).ReturnsAsync(patient);

            //Act
            var result = await _controller.GetMedicalHistoryById(new Guid("9c0de042-8c1b-4b58-9cec-0cd2819e518a"));

            //Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            var historyDto = Assert.IsType<MedicalHistoryDto>(okResult.Value);

            Assert.Equal("5207907", historyDto.PatientId.ToString());
            Assert.Equal("1-A", historyDto.Complement);
            Assert.Equal("Weimar", historyDto.FirstName);
            Assert.Equal("Barea", historyDto.LastName);
            Assert.Equal("Male", historyDto.Gender);
        }

        [Fact]
        public async Task GetMedicalHistoryById_ReturnsMedicalHistoryDto_WhenMedicalHistoryExistsWithNoMatchingPatients()
        {
            //Arrange
            var medicalHistoryId = new Guid("9c0de042-8c1b-4b58-9cec-0cd2819e518a");
            var medicalHistory = new MedicalHistory
            {
                Id = medicalHistoryId,
                PatientId = 5207907,
                PastIllnesses = "test1",
                Surgeries = "test",
                Hospitalizations = "test",
                Allergies = "test",
                CurrentMedications = "test",
                SubstanceAbuseHistory = "test",
                FamilyMedicalHistory = "test",
                Occupation = "test",
                Lifestyle = "test",
            };

            _mockRepo.Setup(repo => repo.GetAsync(new Guid("9c0de042-8c1b-4b58-9cec-0cd2819e518a"))).ReturnsAsync(medicalHistory);

            //Act
            var result = await _controller.GetMedicalHistoryById(new Guid("9c0de042-8c1b-4b58-9cec-0cd2819e518a"));

            //Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            var historyDto = Assert.IsType<MedicalHistoryDto>(okResult.Value);

            Assert.Equal("5207907", historyDto.PatientId.ToString());
            Assert.Equal("", historyDto.Complement);
            Assert.Equal("", historyDto.FirstName);
            Assert.Equal("", historyDto.LastName);
            Assert.Equal("", historyDto.Gender);
        }

        [Fact]
        public async Task GetMedicalHistoryById_ReturnsNotFound_WhenMedicalHistoryDoesNotExists()
        {
            //Arrange
            _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<Guid>())).ReturnsAsync((MedicalHistory)null!);

            //Act
            var result = await _controller.GetMedicalHistoryById(Guid.NewGuid());

            //Assert
            var okResult = Assert.IsAssignableFrom<NotFoundResult>(result.Result);
            Assert.IsType<NotFoundResult>(okResult);
        }

        [Fact]
        public async Task GetMedicalHistoryById_CallsRepositoryOnce()
        {
            //Arrange
            Guid id = new Guid("9c0de042-8c1b-4b58-9cec-0cd2819e518a");
            _mockRepo.Setup(repo => repo.GetAsync(id)).ReturnsAsync((MedicalHistory)null!);

            //Act
            await _controller.GetMedicalHistoryById(id);

            //Assert
            _mockRepo.Verify(repo => repo.GetAsync(id), Times.Once());
        }
    }
}
