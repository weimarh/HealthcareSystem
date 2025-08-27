using ElectronicHealthRecordsService;
using ElectronicHealthRecordsService.Controllers;
using ElectronicHealthRecordsService.Entities;
using ElectronicHealthRecordsService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.ElectronicHealthRecordsServiceTests.MedicalAppointmentTests
{
    public class GetMedicalAppointmentByIdTests
    {
        private readonly Mock<IRepository<MedicalAppointment>> _mockRepository;
        private readonly MedicalAppointmentsController _controller;

        public GetMedicalAppointmentByIdTests()
        {
            _mockRepository = new Mock<IRepository<MedicalAppointment>>();
            _controller = new MedicalAppointmentsController(_mockRepository.Object);
        }

        [Fact]
        public async Task GetMedicalAppointmentById_ReturnsMedicalAppointmentDto_WhenMedicalAppointmentExists()
        {
            //Arrange
            var medicalAppointment = new MedicalAppointment
            {
                Id = new Guid("99187560-ff27-4e6b-8ec0-556247637a0c"),
                PatientId = 5245687,
                ConsultationDate = DateTime.UtcNow,
                Symptoms = "Tests",
                Diagnosis = "Tests",
                Treatment = "Tests"
            };

            _mockRepository.Setup(repo => repo.GetAsync(new Guid("99187560-ff27-4e6b-8ec0-556247637a0c"))).ReturnsAsync(medicalAppointment);

            //Act
            var result = await _controller.GetMedicalAppointmentById(new Guid("99187560-ff27-4e6b-8ec0-556247637a0c"));

            //Assert
            var actionResult = Assert.IsType<ActionResult<MedicalAppointmentDto>>(result);

            Assert.Equal(medicalAppointment.Id, result.Value?.Id);
            Assert.Equal(medicalAppointment.PatientId, result.Value?.PatientId);
            Assert.Equal(medicalAppointment.ConsultationDate, result.Value?.ConsultationDate);
            Assert.Equal(medicalAppointment.Symptoms, result.Value?.Symptoms);
            Assert.Equal(medicalAppointment.Diagnosis, result.Value?.Diagnosis);
            Assert.Equal(medicalAppointment.Treatment, result.Value?.Treatment);
        }

        [Fact]
        public async Task GetMedicalAppointmentById_ReturnsNotFound_WhenMedicalAppointmentDoesNotExists()
        {
            //Arrange
            _mockRepository.Setup(repo => repo.GetAsync(It.IsAny<Guid>())).ReturnsAsync((MedicalAppointment)null!);

            //Act
            var result = await _controller.GetMedicalAppointmentById(Guid.NewGuid());

            //Assert
            var actionResult = Assert.IsType<ActionResult<MedicalAppointmentDto>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetMedicalAppointmentById_CallsRepositoryOnce()
        {
            //Arrange
            var id = new Guid("99187560-ff27-4e6b-8ec0-556247637a0c");
            _mockRepository.Setup(repo => repo.GetAsync(id)).ReturnsAsync(new MedicalAppointment());

            //Act
            var result = await _controller.GetMedicalAppointmentById(id);

            //Assert
            _mockRepository.Verify(repo => repo.GetAsync(id), Times.Once);
        }
    }
}
