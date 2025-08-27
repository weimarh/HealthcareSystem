using ElectronicHealthRecordsService;
using ElectronicHealthRecordsService.Controllers;
using ElectronicHealthRecordsService.Entities;
using ElectronicHealthRecordsService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PatientManagementService.Entities;

namespace HealthCareSystemUnitTests.ElectronicHealthRecordsServiceTests.MedicalHistoryTests
{
    public class PutMedicalHistoryTests
    {
        private readonly Mock<IRepository<MedicalHistory>> _mockRepo;
        private readonly MedicalHistoriesController _controller;

        public PutMedicalHistoryTests()
        {
            _mockRepo = new Mock<IRepository<MedicalHistory>>();
            _controller = new MedicalHistoriesController(_mockRepo.Object);
        }

        [Fact]
        public async Task PutMedicalHistory_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            //Arrange
            var id = new Guid("9c0de042-8c1b-4b58-9cec-0cd2819e518a");
            var existingMedicalHistory = new MedicalHistory { Id = id };
            var updateMedicalHistory = new UpdateMedicalHistoryDto(
                "test",
                "test",
                "test",
                "test",
                "test",
                "test",
                "test",
                "test",
                "test");

            _mockRepo.Setup(repo => repo.GetAsync(id)).ReturnsAsync(existingMedicalHistory);
            _mockRepo.Setup(repo => repo.UpdateAsync(existingMedicalHistory)).Returns(Task.CompletedTask);

            //Act
            var result = await _controller.PutMedicalHistory(id, updateMedicalHistory);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutMedicalHistory_ReturnsNotFound_WhenMedicalHistoryDoesNotExists()
        {
            var id = new Guid("9c0de042-8c1b-4b58-9cec-0cd2819e518a");
            _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<Guid>())).ReturnsAsync((MedicalHistory)null!);

            var updateMedicalHistory = new UpdateMedicalHistoryDto(
                "test",
                "test",
                "test",
                "test",
                "test",
                "test",
                "test",
                "test",
                "test");

            //Act
            var result = await _controller.PutMedicalHistory(id, updateMedicalHistory);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
