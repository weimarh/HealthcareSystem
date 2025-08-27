using Microsoft.AspNetCore.Mvc;
using Moq;
using PatientManagementService;
using PatientManagementService.Controllers;
using PatientManagementService.Entities;
using PatientManagementService.Repositories;

namespace HealthCareSystemUnitTests.PatientManagementServiceUnitTests
{
    public class PutPatientTests
    {
        private readonly Mock<IPatientRepository> _mockRepo;
        private readonly PatientsController _controller;

        public PutPatientTests()
        {
            _mockRepo = new Mock<IPatientRepository>();
            _controller = new PatientsController(_mockRepo.Object);

        }

        [Fact]
        public async Task PutPatient_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var patientId = 1;
            var existingPatient = new Patient { Id = patientId };
            var updateDto = new UpdatePatientDto(
                "3A",
                "John",
                "Doe",
                1,
                "123 Main St",
                "Jane Doe",
                "70725926"
            );

            _mockRepo.Setup(repo => repo.GetAsync(patientId))
                    .ReturnsAsync(existingPatient);
            _mockRepo.Setup(repo => repo.UpdateAsync(existingPatient))
                    .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PutPatient(patientId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutPatient_ReturnsNotFound_WhenPatientDoesNotExist()
        {
            // Arrange
            var patientId = 1;
            _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<int>())).ReturnsAsync((Patient)null!);

            var updateDto = new UpdatePatientDto(
                "3A",
                "John",
                "Doe",
                1,
                "123 Main St",
                "Jane Doe",
                "70725926"
            );

            // Act
            var result = await _controller.PutPatient(patientId, updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
