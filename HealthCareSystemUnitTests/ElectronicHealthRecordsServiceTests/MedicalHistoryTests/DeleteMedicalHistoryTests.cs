using ElectronicHealthRecordsService.Controllers;
using ElectronicHealthRecordsService.Entities;
using ElectronicHealthRecordsService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.ElectronicHealthRecordsServiceTests.MedicalHistoryTests
{
    public class DeleteMedicalHistoryTests
    {
        private readonly Mock<IRepository<MedicalHistory>> _mockRepo;
        private readonly MedicalHistoriesController _controller;

        public DeleteMedicalHistoryTests()
        {
            _mockRepo = new Mock<IRepository<MedicalHistory>>();
            _controller = new MedicalHistoriesController(_mockRepo.Object);
        }

        [Fact]
        public async Task DeleteMedicalHistory_ReturnsNoContest_WhenIdIsValid()
        {
            //Arrange
            var id = new Guid("9c0de042-8c1b-4b58-9cec-0cd2819e518a");

            _mockRepo.Setup(repo => repo.GetAsync(id)).ReturnsAsync(new MedicalHistory {Id = id});
            _mockRepo.Setup(repo => repo.RemoveAsync(id)).Returns(Task.CompletedTask);

            //Act
            var result = await _controller.DeleteMedicalHistory(id);

            //Assert
            Assert.IsType<NoContentResult>(result);
            _mockRepo.Verify(repo => repo.RemoveAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteMedicalHistoryt_InvalidId_ReturnsNotFound()
        {
            //Arrange
            var id = Guid.NewGuid();
            _mockRepo.Setup(repo => repo.RemoveAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

            //Act
            var result = await _controller.DeleteMedicalHistory(id);

            //Assert
            Assert.IsType<NotFoundResult>(result);
            _mockRepo.Verify(repo => repo.RemoveAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}
