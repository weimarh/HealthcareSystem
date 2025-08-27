using Microsoft.AspNetCore.Mvc;
using Moq;
using PatientManagementService;
using PatientManagementService.Controllers;
using PatientManagementService.Entities;
using PatientManagementService.Enums;
using PatientManagementService.Repositories;

namespace HealthCareSystemUnitTests.PatientManagementServiceUnitTests
{
    public class GetPatientByIdTests
    {
        private readonly Mock<IPatientRepository> _mockRepo;
        private readonly PatientsController _controller;

        public GetPatientByIdTests()
        {
            _mockRepo = new Mock<IPatientRepository>();
            _controller = new PatientsController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetPatientById_ReturnsPatientDto_WhenPatientExists()
        {
            //Arrange
            var testPatient = new Patient
            {
                Id = 1,
                Complement = "1A",
                FirstName = "Test1",
                LastName = "Test1",
                Gender = Gender.Male,
                HomeAddress = "Test1",
                EmergencyContactName = "Test1",
                EmergencyContactPhone = "70725926"
            };

            _mockRepo.Setup(repo => repo.GetAsync(1)).ReturnsAsync(testPatient);

            //Act
            var result = await _controller.GetPatientById(1);

            //Assert
            var actionResult = Assert.IsType<ActionResult<PatientDto>>(result);

            Assert.Equal(testPatient.Id, result.Value?.Id);
            Assert.Equal(testPatient.Complement, result.Value?.Complement);
            Assert.Equal(testPatient.FirstName, result.Value?.FirstName);
            Assert.Equal(testPatient.LastName, result.Value?.LastName);
            Assert.Equal(testPatient.Gender.ToString(), result.Value?.Gender);
            Assert.Equal(testPatient.HomeAddress, result.Value?.HomeAddress);
            Assert.Equal(testPatient.EmergencyContactName, result.Value?.EmergencyContactName);
            Assert.Equal(testPatient.EmergencyContactPhone, result.Value?.EmergencyContactPhone);
        }

        [Fact]
        public async Task GetPatientById_ReturnsNotFound_WhenPatientDoesNotExists()
        {
            //Arrange
            _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<int>())).ReturnsAsync((Patient)null!);

            //Act
            var result = await _controller.GetPatientById(999);

            //Assert
            var actionResult = Assert.IsType<ActionResult<PatientDto>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetPatientById_CallsRepositoryOnce()
        {
            //Arrange
            int testId = 1;
            _mockRepo.Setup(repo => repo.GetAsync(testId)).ReturnsAsync(new Patient());

            //Act
            await _controller.GetPatientById(testId);

            //Assert
            _mockRepo.Verify(repo => repo.GetAsync(testId), Times.Once());
        }
    }
}
