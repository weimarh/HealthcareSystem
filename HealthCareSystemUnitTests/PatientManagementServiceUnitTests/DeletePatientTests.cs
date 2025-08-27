using Microsoft.AspNetCore.Mvc;
using Moq;
using PatientManagementService.Controllers;
using PatientManagementService.Entities;
using PatientManagementService.Repositories;

namespace HealthCareSystemUnitTests.PatientManagementServiceUnitTests
{
    public class DeletePatientTests
    {
        private readonly Mock<IPatientRepository> _mockRepo;
        private readonly PatientsController _controller;

        public DeletePatientTests()
        {
            _mockRepo = new Mock<IPatientRepository>();
            _controller = new PatientsController(_mockRepo.Object);
        }

        [Fact]
        public async Task DeletePatient_ReturnsNoContest_WhenIdIsValid()
        {
            //Arrange
            int testId = 1;

            _mockRepo.Setup(repo => repo.GetAsync(testId)).ReturnsAsync(new Patient { Id = testId });
            _mockRepo.Setup(repo => repo.RemoveAsync(testId)).Returns(Task.CompletedTask);

            //Act
            var result = await _controller.DeletePatient(testId);

            //Assert
            Assert.IsType<NoContentResult>(result);
            _mockRepo.Verify(repo => repo.RemoveAsync(testId), Times.Once);
        }

        [Fact]
        public async Task DeletePatient_InvalidId_ReturnsNotFound()
        {
            // Arrange
            int testId = 999;
            _mockRepo.Setup(repo => repo.RemoveAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeletePatient(testId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockRepo.Verify(repo => repo.RemoveAsync(It.IsAny<int>()), Times.Never); 
        }
    }
}
