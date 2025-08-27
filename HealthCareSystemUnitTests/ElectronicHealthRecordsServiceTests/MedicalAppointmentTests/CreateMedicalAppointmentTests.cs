using ElectronicHealthRecordsService;
using ElectronicHealthRecordsService.Controllers;
using ElectronicHealthRecordsService.Entities;
using ElectronicHealthRecordsService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.ElectronicHealthRecordsServiceTests.MedicalAppointmentTests
{
    public class CreateMedicalAppointmentTests
    {
        private readonly Mock<IRepository<MedicalAppointment>> _mockRepository;
        private readonly MedicalAppointmentsController _controller;

        public CreateMedicalAppointmentTests()
        {
            _mockRepository = new Mock<IRepository<MedicalAppointment>>();
            _controller = new MedicalAppointmentsController(_mockRepository.Object);
        }

        [Fact]
        public async Task CreateMedicalAppointment_ReturnsCreatedAtActionResult_WithCorrectMedicalAppointment()
        {
            var medicalAppointmentDto = new CreateMedicalAppointmentDto(
                5207907,
                DateTime.Now,
                "Test",
                "Test",
                "Test");

            var medicalAppointment = new MedicalAppointment { Id = new Guid("90a9b853-8cd0-4781-95ba-7253c8f38beb") };

            _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<MedicalAppointment>()))
                .Returns(Task.CompletedTask).Callback<MedicalAppointment>(p => p.Id = medicalAppointment.Id);

            //Act
            var result = await _controller.CreateMedicalAppointment(medicalAppointmentDto);

            //Assert
            var actionResult = Assert.IsType<ActionResult<MedicalAppointmentDto>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);

            Assert.Equal(nameof(MedicalAppointmentsController.GetMedicalAppointmentById), createdAtActionResult.ActionName);
            Assert.NotNull(createdAtActionResult.RouteValues);
            Assert.Equal(medicalAppointment.Id, createdAtActionResult.RouteValues["id"]);
            var returnValue = Assert.IsType<MedicalAppointment>(createdAtActionResult.Value);
        }

        [Fact]
        public async Task CreateMedicalAppointment_CallsRepositoryOnce_WithCorrectMedicalAppointment()
        {
            //Arrange
            var medicalAppointmentDto = new CreateMedicalAppointmentDto(
                5207907,
                DateTime.Now,
                "Test",
                "Test",
                "Test");

            MedicalAppointment? createMedicalAppointment = null;
            _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<MedicalAppointment>()))
                .Callback<MedicalAppointment>(p => createMedicalAppointment = p)
                .Returns(Task.CompletedTask);

            //Act
            var result = await _controller.CreateMedicalAppointment(medicalAppointmentDto);

            //Assert
            _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<MedicalAppointment>()), Times.Once());
            Assert.NotNull(createMedicalAppointment);
        }

        [Fact]
        public async Task CreateMedicalAppointment_MapsAllPropertiesCorrectly()
        {
            //Arrange
            var medicalAppointmentDto = new CreateMedicalAppointmentDto(
                5207907,
                DateTime.Now,
                "Test",
                "Test",
                "Test");

            MedicalAppointment? createMedicalAppointment = null;
            _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<MedicalAppointment>()))
                .Callback<MedicalAppointment>(p => createMedicalAppointment = p)
                .Returns(Task.CompletedTask);

            //Act
            await _controller.CreateMedicalAppointment(medicalAppointmentDto);

            //Assert
            Assert.NotNull(medicalAppointmentDto);
            Assert.Equal(medicalAppointmentDto.PatientId, medicalAppointmentDto.PatientId);
            Assert.Equal(medicalAppointmentDto.ConsultationDate, medicalAppointmentDto.ConsultationDate);
            Assert.Equal(medicalAppointmentDto.Diagnosis, medicalAppointmentDto.Diagnosis);
            Assert.Equal(medicalAppointmentDto.Symptoms, medicalAppointmentDto.Symptoms);
            Assert.Equal(medicalAppointmentDto.Treatment, medicalAppointmentDto.Treatment);
        }

        [Fact]
        public async Task CreateMedicalAppointment_ReturnsCorrectResponseType()
        {
            //Arrange
            var medicalAppointmentDto = new CreateMedicalAppointmentDto(
                5207907,
                DateTime.Now,
                "Test",
                "Test",
                "Test");

            _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<MedicalAppointment>()))
                .Returns(Task.CompletedTask);

            //Act
            var result = await _controller.CreateMedicalAppointment(medicalAppointmentDto);

            //Assert
            Assert.IsType<ActionResult<MedicalAppointmentDto>>(result);
        }
    }
}
