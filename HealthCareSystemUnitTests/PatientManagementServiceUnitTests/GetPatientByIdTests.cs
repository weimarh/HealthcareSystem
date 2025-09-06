using MassTransit;
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
        private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;

        public GetPatientByIdTests()
        {
            _mockRepo = new Mock<IPatientRepository>();
            _mockPublishEndpoint = new Mock<IPublishEndpoint>();
            _controller = new PatientsController(_mockRepo.Object, _mockPublishEndpoint.Object);
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
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            var patient = Assert.IsType<PatientDto>(okResult.Value);

            Assert.Equal(testPatient.Id, patient.Id);
            Assert.Equal(testPatient.Complement, patient.Complement);
            Assert.Equal(testPatient.FirstName, patient.FirstName);
            Assert.Equal(testPatient.LastName, patient.LastName);
            Assert.Equal(testPatient.Gender.ToString(), patient.Gender);
            Assert.Equal(testPatient.HomeAddress, patient.HomeAddress);
            Assert.Equal(testPatient.EmergencyContactName, patient.EmergencyContactName);
            Assert.Equal(testPatient.EmergencyContactPhone, patient.EmergencyContactPhone);
        }

        [Fact]
        public async Task GetPatientById_ReturnsNotFound_WhenPatientDoesNotExists()
        {
            //Arrange
            _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<int>())).ReturnsAsync((Patient)null!);

            //Act
            var result = await _controller.GetPatientById(999);

            //Assert
            var okResult = Assert.IsAssignableFrom<NotFoundResult>(result.Result);
            Assert.IsType<NotFoundResult>(okResult);
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
