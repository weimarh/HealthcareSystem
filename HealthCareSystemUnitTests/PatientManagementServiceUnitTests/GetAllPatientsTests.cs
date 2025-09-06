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
    public class GetAllPatientsTests
    {
        private readonly Mock<IPatientRepository> _mockPatientRepository;
        private readonly PatientsController _patientsController;
        private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;

        public GetAllPatientsTests()
        {
            _mockPatientRepository = new Mock<IPatientRepository>();
            _mockPublishEndpoint = new Mock<IPublishEndpoint>();
            _patientsController = new PatientsController(_mockPatientRepository.Object, _mockPublishEndpoint.Object);
        }

        [Fact]
        public async Task GetAllPatients_ReturnsListOfPatientDtos_WhenPatientsExist()
        {
            //Arrange
            var testPatients = new List<Patient>
            {
                new Patient
                {
                    Id = 1,
                    Complement = "1A",
                    FirstName = "Test1",
                    LastName = "Test1",
                    Gender = Gender.Male,
                    HomeAddress = "Test1",
                    EmergencyContactName = "Test1",
                    EmergencyContactPhone = "70725926"
                },
                new Patient
                {
                    Id = 2,
                    Complement = "1A",
                    FirstName = "Test2",
                    LastName = "Test2",
                    Gender = Gender.Male,
                    HomeAddress = "Test2",
                    EmergencyContactName = "Test2",
                    EmergencyContactPhone = "70725926"
                },
                new Patient
                {
                    Id = 3,
                    Complement = "",
                    FirstName = "Test3",
                    LastName = "Test3",
                    Gender = Gender.Female,
                    HomeAddress = "Test3",
                    EmergencyContactName = "Test3",
                    EmergencyContactPhone = "70725926"
                },
            };

            _mockPatientRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(testPatients);

            //Act
            var result = await _patientsController.GetAllPatients();

            //Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            var patientDto = Assert.IsType<List<PatientDto>>(okResult.Value);

            Assert.Equal(3, patientDto.Count());

            Assert.Equal(1 , patientDto[0].Id);
            Assert.Equal("1A", patientDto[0].Complement);
            Assert.Equal("Test1", patientDto[0].FirstName);
            Assert.Equal("Test1", patientDto[0].LastName);
            Assert.Equal("Male", patientDto[0].Gender.ToString());
            Assert.Equal("Test1", patientDto[0].HomeAddress);
            Assert.Equal("Test1", patientDto[0].EmergencyContactName);
            Assert.Equal("70725926", patientDto[0].EmergencyContactPhone);

            Assert.Equal(2, patientDto[1].Id);
            Assert.Equal("1A", patientDto[1].Complement);
            Assert.Equal("Test2", patientDto[1].FirstName);
            Assert.Equal("Test2", patientDto[1].LastName);
            Assert.Equal("Male", patientDto[1].Gender.ToString());
            Assert.Equal("Test2", patientDto[1].HomeAddress);
            Assert.Equal("Test2", patientDto[1].EmergencyContactName);
            Assert.Equal("70725926", patientDto[1].EmergencyContactPhone);

            Assert.Equal(3, patientDto[2].Id);
            Assert.Equal("", patientDto[2].Complement);
            Assert.Equal("Test3", patientDto[2].FirstName);
            Assert.Equal("Test3", patientDto[2].LastName);
            Assert.Equal("Female", patientDto[2].Gender.ToString());
            Assert.Equal("Test3", patientDto[2].HomeAddress);
            Assert.Equal("Test3", patientDto[2].EmergencyContactName);
            Assert.Equal("70725926", patientDto[2].EmergencyContactPhone);

        }

        [Fact]
        public async Task GetAllPatients_ReturnsEmptyList_WhenNoPatientsExist()
        {
            // Arrange
            _mockPatientRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Patient>());

            // Act
            var result = await _patientsController.GetAllPatients();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var patientDto = Assert.IsType<List<PatientDto>>(okResult.Value);
            Assert.Empty(patientDto);
        }

        [Fact]
        public async Task GetAllPatients_CallsRepositoryOnce()
        {
            // Arrange
            _mockPatientRepository.Setup(repo => repo.GetAllAsync())
                    .ReturnsAsync(new List<Patient>());

            // Act
            await _patientsController.GetAllPatients();

            // Assert
            _mockPatientRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }
    }
}
