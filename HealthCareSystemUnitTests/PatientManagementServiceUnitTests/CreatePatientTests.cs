using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PatientManagementService;
using PatientManagementService.Controllers;
using PatientManagementService.Entities;
using PatientManagementService.Enums;
using PatientManagementService.Repositories;
using System.ComponentModel.DataAnnotations;

namespace HealthCareSystemUnitTests.PatientManagementServiceUnitTests
{
    public class CreatePatientTests
    {
        private readonly Mock<IPatientRepository> _mockRepo;
        private readonly PatientsController _controller;

        public CreatePatientTests()
        {
            _mockRepo = new Mock<IPatientRepository>();
            _controller = new PatientsController(_mockRepo.Object);
        }

        [Fact]
        public async Task CreatePatient_ReturnsCreatedAtActionResult_WithCorrectPatient()
        {
            var createDto = new CreatePatientDto(
                1,
                "3A",
                "John",
                "Doe",
                1,
                "123 Main St",
                "Jane Doe",
                "70725926"
            );

            _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<Patient>()))
                    .Returns(Task.CompletedTask)
                    .Callback<Patient>(p => p.Id = createDto.Id);

            // Act
            var result = await _controller.CreatePatient(createDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<PatientDto>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);

            Assert.Equal(nameof(PatientsController.GetPatientById), createdAtActionResult.ActionName);
            Assert.NotNull(createdAtActionResult.RouteValues);
            Assert.Equal(createDto.Id, createdAtActionResult.RouteValues["id"]);
            var returnValue = Assert.IsType<Patient>(createdAtActionResult.Value);
            Assert.Equal(createDto.FirstName, returnValue.FirstName);
            Assert.Equal(createDto.LastName, returnValue.LastName);
        }

        [Fact]
        public async Task CreatePatient_CallsRepositoryOnce_WithCorrectPatient()
        {
            // Arrange
            var createDto = new CreatePatientDto(
                1,
                "3A",
                "John",
                "Doe",
                1,
                "123 Main St",
                "Jane Doe",
                "70725926"
            );

            Patient? createdPatient = null;
            _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<Patient>()))
                    .Callback<Patient>(p => createdPatient = p)
                    .Returns(Task.CompletedTask);

            // Act
            await _controller.CreatePatient(createDto);

            // Assert
            _mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<Patient>()), Times.Once);
            Assert.NotNull(createdPatient);
        }

        [Fact]
        public async Task CreatePatient_MapsAllPropertiesCorrectly()
        {
            // Arrange
            var createDto = new CreatePatientDto(
                1,
                "3A",
                "John",
                "Doe",
                1,
                "123 Main St",
                "Jane Doe",
                "70725926"
            );

            Patient? createdPatient = null;
            _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<Patient>()))
                    .Callback<Patient>(p => createdPatient = p)
                    .Returns(Task.CompletedTask);

            // Act
            await _controller.CreatePatient(createDto);

            // Assert
            Assert.NotNull(createdPatient);
            Assert.Equal(createDto.Id, createdPatient.Id);
            Assert.Equal(createDto.FirstName, createdPatient.FirstName);
            Assert.Equal(createDto.LastName, createdPatient.LastName);
            Assert.Equal(createDto.Complement, createdPatient.Complement);
            Assert.Equal((Gender)createDto.Gender, createdPatient.Gender);
            Assert.Equal(createDto.HomeAddress, createdPatient.HomeAddress);
            Assert.Equal(createDto.EmergencyContactName, createdPatient.EmergencyContactName);
        }

        [Fact]
        public async Task CreatePatient_ReturnsCorrectResponseType()
        {
            // Arrange
            var createDto = new CreatePatientDto(
                1,
                "3A",
                "John",
                "Doe",
                1,
                "123 Main St",
                "Jane Doe",
                "70725926"
            );

            _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<Patient>()))
                    .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreatePatient(createDto);

            // Assert
            Assert.IsType<ActionResult<PatientDto>>(result);
        }
    }
}
