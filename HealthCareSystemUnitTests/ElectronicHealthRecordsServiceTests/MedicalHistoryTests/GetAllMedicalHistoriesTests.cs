using ElectronicHealthRecordsService;
using ElectronicHealthRecordsService.Controllers;
using ElectronicHealthRecordsService.Entities;
using ElectronicHealthRecordsService.Repositories;
using Moq;

namespace HealthCareSystemUnitTests.ElectronicHealthRecordsServiceTests.MedicalHistoryTests
{
    public class GetAllMedicalHistoriesTests
    {
        private readonly Mock<IRepository<MedicalHistory>> _mockRepo;
        private readonly MedicalHistoriesController _controller;

        public GetAllMedicalHistoriesTests()
        {
            _mockRepo = new Mock<IRepository<MedicalHistory>>();
            _controller = new MedicalHistoriesController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllMedicalHistories_ReturnsListOfMedicalHistoriesDtos_WhenMedicalHistoriesExists()
        {
            //Arrange
            var medicalHistories = new List<MedicalHistory>
            {
                new MedicalHistory
                {
                    Id = Guid.NewGuid(),
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
                },
                new MedicalHistory
                {
                    Id = Guid.NewGuid(),
                    PatientId = 5207907,
                    PastIllnesses = "test2",
                    Surgeries = "test",
                    Hospitalizations = "test",
                    Allergies = "test",
                    CurrentMedications = "test",
                    SubstanceAbuseHistory = "test",
                    FamilyMedicalHistory = "test",
                    Occupation = "test",
                    Lifestyle = "test",
                },
                new MedicalHistory
                {
                    Id = Guid.NewGuid(),
                    PatientId = 5207907,
                    PastIllnesses = "test3",
                    Surgeries = "test",
                    Hospitalizations = "test",
                    Allergies = "test",
                    CurrentMedications = "test",
                    SubstanceAbuseHistory = "test",
                    FamilyMedicalHistory = "test",
                    Occupation = "test",
                    Lifestyle = "test",
                }
            };

            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(medicalHistories);

            //Act
            var result = await _controller.GetAllMedicalHistories();

            //Assert
            Assert.IsAssignableFrom<IEnumerable<MedicalHistoryDto>>(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetAllMedicalHistories_ReturnsEmptyList_WhenNoMedicalHistoriesExists()
        {
            //Arrange
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<MedicalHistory>());

            //Act
            var result = await _controller.GetAllMedicalHistories();

            //Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllMedicalHistories_CallsRepositoryOnce()
        {
            //Arrange
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<MedicalHistory>());

            //Act
            var result = await _controller.GetAllMedicalHistories();

            //Assert
            _mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllMedicalHistories_MapsAllPropertiesCorrectly()
        {
            //Arrange
            var medicalHistory = new MedicalHistory
            {
                Id = Guid.NewGuid(),
                PatientId = 5207907,
                PastIllnesses = "test",
                Surgeries = "test",
                Hospitalizations = "test",
                Allergies = "test",
                CurrentMedications = "test",
                SubstanceAbuseHistory = "test",
                FamilyMedicalHistory = "test",
                Occupation = "test",
                Lifestyle = "test",
            };

            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<MedicalHistory> { medicalHistory });

            //Act
            var result = (await _controller.GetAllMedicalHistories()).First();

            //Assert
            Assert.Equal(medicalHistory.PatientId, result.PatientId);
            Assert.Equal(medicalHistory.PastIllnesses, result.PastIllnesses);
            Assert.Equal(medicalHistory.Surgeries, result.Surgeries);
            Assert.Equal(medicalHistory.Hospitalizations, result.Hospitalizations);
            Assert.Equal(medicalHistory.Allergies, result.Allergies);
            Assert.Equal(medicalHistory.CurrentMedications, result.CurrentMedications);
            Assert.Equal(medicalHistory.SubstanceAbuseHistory, result.SubstanceAbuseHistory);
            Assert.Equal(medicalHistory.FamilyMedicalHistory, result.FamilyMedicalHistory);
            Assert.Equal(medicalHistory.Occupation, result.Occupation);
            Assert.Equal(medicalHistory.Lifestyle, result.Lifestyle);
        }
    }
}
