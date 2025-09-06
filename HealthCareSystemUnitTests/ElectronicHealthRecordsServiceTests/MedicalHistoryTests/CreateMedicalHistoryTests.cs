using ElectronicHealthRecordsService;
using ElectronicHealthRecordsService.Controllers;
using ElectronicHealthRecordsService.Entities;
using ElectronicHealthRecordsService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.ElectronicHealthRecordsServiceTests.MedicalHistoryTests
{
    public class CreateMedicalHistoryTests
    {
        private readonly Mock<IRepository<MedicalHistory>> _mockRepo;
        private readonly MedicalHistoriesController _controller;
        private readonly Mock<IPatientRepository> _mockPatientRepository;

        public CreateMedicalHistoryTests()
        {
            _mockRepo = new Mock<IRepository<MedicalHistory>>();
            _mockPatientRepository = new Mock<IPatientRepository>();
            _controller = new MedicalHistoriesController(_mockRepo.Object, _mockPatientRepository.Object);
        }

        [Fact]
        public async Task CreateMedicalHistory_ReturnsCreatedAtActionResult_WithCorrectMedicalHistory()
        {
            //Arrange
            var createMedicalHistoryDto = new CreateMedicalHistoryDto(
                1,
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test");

            var medicalHistory = new MedicalHistory { Id = new Guid("90a9b853-8cd0-4781-95ba-7253c8f38beb") };

            _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<MedicalHistory>())).Returns(Task.CompletedTask)
                .Callback<MedicalHistory>(m => m.Id = medicalHistory.Id);

            //Act
            var result = await _controller.CreateMedicalHistory(createMedicalHistoryDto);

            //Assert
            var actionResult = Assert.IsType<ActionResult<MedicalHistoryDto>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);

            Assert.Equal(nameof(MedicalHistoriesController.GetMedicalHistoryById), createdAtActionResult.ActionName);
            Assert.NotNull(createdAtActionResult.RouteValues);
            Assert.Equal(medicalHistory.Id, createdAtActionResult.RouteValues["id"]);
            var returnValue = Assert.IsType<MedicalHistory>(createdAtActionResult.Value);
            Assert.Equal(createMedicalHistoryDto.FamilyMedicalHistory, returnValue.FamilyMedicalHistory);
            Assert.Equal(createMedicalHistoryDto.Occupation, returnValue.Occupation);
        }

        [Fact]
        public async Task CreateMedicalHistory_CallsRepositoryOnce_WithCorrectMedicalHistory()
        {
            //Arrange
            var createMedicalHistoryDto = new CreateMedicalHistoryDto(
                1,
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test");

            MedicalHistory? medicalHistory = null;
            _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<MedicalHistory>()))
                .Callback<MedicalHistory>(m => medicalHistory = m)
                .Returns(Task.CompletedTask);

            //Act
            await _controller.CreateMedicalHistory(createMedicalHistoryDto);

            //Assert
            _mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<MedicalHistory>()), Times.Once);
            Assert.NotNull(medicalHistory);
        }

        [Fact]
        public async Task CreateMedicalHistory_ReturnsCorrectResponseType()
        {
            //Arrange
            var createMedicalHistoryDto = new CreateMedicalHistoryDto(
                1,
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test");

            _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<MedicalHistory>()))
                .Returns(Task.CompletedTask);

            //Act
            var result = await _controller.CreateMedicalHistory(createMedicalHistoryDto);

            //Assert
            Assert.IsType<ActionResult<MedicalHistoryDto>>(result);
        }
    }
}
