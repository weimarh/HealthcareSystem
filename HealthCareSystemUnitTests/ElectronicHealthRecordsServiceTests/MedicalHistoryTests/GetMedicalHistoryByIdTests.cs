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

        public GetMedicalHistoryByIdTests()
        {
            _mockRepo = new Mock<IRepository<MedicalHistory>>();
            _controller = new MedicalHistoriesController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetMedicalHistoryById_ReturnsMedicalHistoryDto_WhenMedicalHistoryExists()
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
            var actionResult = Assert.IsType<ActionResult<MedicalHistoryDto>>(result);

            Assert.Equal(medicalHistory.Id, result.Value?.Id);
        }

        [Fact]
        public async Task GetMedicalHistoryById_ReturnsNotFound_WhenMedicalHistoryDoesNotExists()
        {
            //Arrange
            _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<Guid>())).ReturnsAsync((MedicalHistory)null!);

            //Act
            var result = await _controller.GetMedicalHistoryById(Guid.NewGuid());

            //Assert
            var actionResult = Assert.IsType<ActionResult<MedicalHistoryDto>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetMedicalHistoryById_CallsRepositoryOnce()
        {
            //Arrange
            Guid id = new Guid("9c0de042-8c1b-4b58-9cec-0cd2819e518a");

            //Act
            await _controller.GetMedicalHistoryById(id);

            //Assert
            _mockRepo.Verify(repo => repo.GetAsync(id), Times.Once());
        }
    }
}
