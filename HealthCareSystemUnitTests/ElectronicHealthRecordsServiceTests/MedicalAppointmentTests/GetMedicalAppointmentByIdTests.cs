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
        private readonly Mock<IPatientRepository> _mockPatientRepository;

        public GetMedicalAppointmentByIdTests()
        {
            _mockRepository = new Mock<IRepository<MedicalAppointment>>();
            _mockPatientRepository = new Mock<IPatientRepository>();
            _controller = new MedicalAppointmentsController(_mockRepository.Object, _mockPatientRepository.Object);
        }

        [Fact]
        public async Task GetMedicalAppointmentById_ReturnsMedicalAppointmentDto_WhenMedicalAppointmentExistsWithMatchingPatients()
        {
            //Arrange
            var medicalAppointment = new MedicalAppointment
            {
                Id = new Guid("99187560-ff27-4e6b-8ec0-556247637a0c"),
                PatientId = 5207907,
                ConsultationDate = DateTime.UtcNow,
                Symptoms = "Tests",
                Diagnosis = "Tests",
                Treatment = "Tests"
            };

            var patient = new Patient
            {
                Id = 5207907,
                Complement = "1-A",
                FirstName = "Weimar",
                LastName = "Barea",
                Gender = "Male",
            };

            _mockRepository.Setup(repo => repo.GetAsync(new Guid("99187560-ff27-4e6b-8ec0-556247637a0c"))).ReturnsAsync(medicalAppointment);
            _mockPatientRepository.Setup(repo => repo.GetPatientByIdAsync(5207907)).ReturnsAsync(patient);

            //Act
            var result = await _controller.GetMedicalAppointmentById(new Guid("99187560-ff27-4e6b-8ec0-556247637a0c"));

            //Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            var appointments = Assert.IsType<MedicalAppointmentDto>(okResult.Value);

            Assert.Equal(medicalAppointment.Id, appointments.Id);
            Assert.Equal(medicalAppointment.PatientId, appointments.PatientId);
            Assert.Equal("1-A", appointments.Complement);
            Assert.Equal("Weimar", appointments.FirstName);
            Assert.Equal("Barea", appointments.LastName);
            Assert.Equal("Male", appointments.Gender);
            Assert.Equal(medicalAppointment.ConsultationDate, appointments.ConsultationDate);
            Assert.Equal(medicalAppointment.Symptoms, appointments.Symptoms);
            Assert.Equal(medicalAppointment.Diagnosis, appointments.Diagnosis);
            Assert.Equal(medicalAppointment.Treatment, appointments.Treatment);
        }

        [Fact]
        public async Task GetMedicalAppointmentById_ReturnsMedicalAppointmentDto_WhenMedicalAppointmentExistsWithNoMatchingPatients()
        {
            //Arrange
            var medicalAppointment = new MedicalAppointment
            {
                Id = new Guid("99187560-ff27-4e6b-8ec0-556247637a0c"),
                PatientId = 5207907,
                ConsultationDate = DateTime.UtcNow,
                Symptoms = "Tests",
                Diagnosis = "Tests",
                Treatment = "Tests"
            };

            _mockRepository.Setup(repo => repo.GetAsync(new Guid("99187560-ff27-4e6b-8ec0-556247637a0c"))).ReturnsAsync(medicalAppointment);

            //Act
            var result = await _controller.GetMedicalAppointmentById(new Guid("99187560-ff27-4e6b-8ec0-556247637a0c"));

            //Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            var appointments = Assert.IsType<MedicalAppointmentDto>(okResult.Value);

            Assert.Equal(medicalAppointment.Id, appointments.Id);
            Assert.Equal(medicalAppointment.PatientId, appointments.PatientId);
            Assert.Equal("", appointments.Complement);
            Assert.Equal("", appointments.FirstName);
            Assert.Equal("", appointments.LastName);
            Assert.Equal("", appointments.Gender);
            Assert.Equal(medicalAppointment.ConsultationDate, appointments.ConsultationDate);
            Assert.Equal(medicalAppointment.Symptoms, appointments.Symptoms);
            Assert.Equal(medicalAppointment.Diagnosis, appointments.Diagnosis);
            Assert.Equal(medicalAppointment.Treatment, appointments.Treatment);
        }

        [Fact]
        public async Task GetMedicalAppointmentById_ReturnsNotFound_WhenMedicalAppointmentDoesNotExists()
        {
            //Arrange
            _mockRepository.Setup(repo => repo.GetAsync(It.IsAny<Guid>())).ReturnsAsync((MedicalAppointment)null!);

            //Act
            var result = await _controller.GetMedicalAppointmentById(Guid.NewGuid());

            //Assert
            var okResult = Assert.IsAssignableFrom<NotFoundResult>(result.Result);
            Assert.IsType<NotFoundResult>(okResult);
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
