using ElectronicHealthRecordsService;
using ElectronicHealthRecordsService.Controllers;
using ElectronicHealthRecordsService.Entities;
using ElectronicHealthRecordsService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.ElectronicHealthRecordsServiceTests.MedicalHistoryTests
{
    public class GetAllMedicalHistoriesTests
    {
        private readonly Mock<IRepository<MedicalHistory>> _mockRepo;
        private readonly MedicalHistoriesController _controller;
        private readonly Mock<IPatientRepository> _mockPatientRepository;

        public GetAllMedicalHistoriesTests()
        {
            _mockRepo = new Mock<IRepository<MedicalHistory>>();
            _mockPatientRepository = new Mock<IPatientRepository>();
            _controller = new MedicalHistoriesController(_mockRepo.Object, _mockPatientRepository.Object);
        }

        [Fact]
        public async Task GetAllMedicalHistories_ReturnsListOfMedicalHistoriesDtos_WhenMedicalHistoriesExistsWithMatchingPatients()
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
                    PatientId = 5296854,
                    PastIllnesses = "test2",
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

            var patients = new List<Patient>
            {
                new Patient
                {
                    Id = 5207907,
                    Complement = "1-A",
                    FirstName = "Weimar",
                    LastName = "Barea",
                    Gender = "Male",
                },
                new Patient
                {
                    Id = 5296854,
                    Complement = "1-A",
                    FirstName = "Hermilene",
                    LastName = "Sanchez",
                    Gender = "Female",
                }
            };

            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(medicalHistories);
            _mockPatientRepository.Setup(repo => repo.GetAllPatientsAsync()).ReturnsAsync(patients);

            //Act
            var result = await _controller.GetAllMedicalHistories();

            //Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            var historiesDto = Assert.IsAssignableFrom<List<MedicalHistoryDto>>(okResult.Value);

            Assert.Equal(2, historiesDto.Count());

            Assert.Equal("5207907", historiesDto[0].PatientId.ToString());
            Assert.Equal("1-A", historiesDto[0].Complement);
            Assert.Equal("Weimar", historiesDto[0].FirstName);
            Assert.Equal("Barea", historiesDto[0].LastName);
            Assert.Equal("Male", historiesDto[0].Gender);

            Assert.Equal("5296854", historiesDto[1].PatientId.ToString());
            Assert.Equal("1-A", historiesDto[1].Complement);
            Assert.Equal("Hermilene", historiesDto[1].FirstName);
            Assert.Equal("Sanchez", historiesDto[1].LastName);
            Assert.Equal("Female", historiesDto[1].Gender);
        }

        [Fact]
        public async Task GetAllMedicalHistories_ReturnsListOfMedicalHistoriesDtos_WhenMedicalHistoriesExistsWithNoMatchingPatients()
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
                    PatientId = 5296854,
                    PastIllnesses = "test2",
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
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            var historiesDto = Assert.IsAssignableFrom<List<MedicalHistoryDto>>(okResult.Value);

            Assert.Equal(2, historiesDto.Count);

            Assert.Equal("5207907", historiesDto[0].PatientId.ToString());
            Assert.Equal("", historiesDto[0].Complement);
            Assert.Equal("", historiesDto[0].FirstName);
            Assert.Equal("", historiesDto[0].LastName);
            Assert.Equal("", historiesDto[0].Gender);

            Assert.Equal("5296854", historiesDto[1].PatientId.ToString());
            Assert.Equal("", historiesDto[1].Complement);
            Assert.Equal("", historiesDto[1].FirstName);
            Assert.Equal("", historiesDto[1].LastName);
            Assert.Equal("", historiesDto[1].Gender);
        }

        [Fact]
        public async Task GetAllMedicalHistories_ReturnsEmptyList_WhenNoMedicalHistoriesExists()
        {
            //Arrange
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<MedicalHistory>());

            //Act
            var result = await _controller.GetAllMedicalHistories();

            //Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            var histories = Assert.IsAssignableFrom<IEnumerable<MedicalHistoryDto>>(okResult.Value);
            Assert.Empty(histories);
        }

        [Fact]
        public async Task GetAllMedicalHistories_CallsRepositoryOnce()
        {
            //Arrange
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<MedicalHistory>());

            _mockPatientRepository.Setup(repo => repo.GetAllPatientsAsync())
                .ReturnsAsync(new List<Patient>());

            //Act
            var result = await _controller.GetAllMedicalHistories();

            //Assert
            _mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
            _mockPatientRepository.Verify(repo => repo.GetAllPatientsAsync(), Times.Once);
        }
    }
}
