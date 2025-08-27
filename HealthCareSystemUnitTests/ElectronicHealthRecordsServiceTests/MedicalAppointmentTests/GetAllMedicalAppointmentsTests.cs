using ElectronicHealthRecordsService;
using ElectronicHealthRecordsService.Controllers;
using ElectronicHealthRecordsService.Entities;
using ElectronicHealthRecordsService.Repositories;
using Moq;

namespace HealthCareSystemUnitTests.ElectronicHealthRecordsServiceTests.MedicalAppointmentTests
{
    public class GetAllMedicalAppointmentsTests
    {
        private readonly Mock<IRepository<MedicalAppointment>> _mockRepository;
        private readonly MedicalAppointmentsController _controller;

        public GetAllMedicalAppointmentsTests()
        {
            _mockRepository = new Mock<IRepository<MedicalAppointment>>();
            _controller = new MedicalAppointmentsController(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllMedicalAppointments_ReturnsListOfMedicalAppointments_WhenMedicalAppointmentsExists()
        {
            //Arrange
            var medicalAppointments = new List<MedicalAppointment>
            {
                new MedicalAppointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = 5245687,
                    ConsultationDate = DateTime.UtcNow,
                    Symptoms = "Tests",
                    Diagnosis = "Tests",
                    Treatment = "Tests"
                },
                new MedicalAppointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = 5245687,
                    ConsultationDate = DateTime.UtcNow,
                    Symptoms = "Tests",
                    Diagnosis = "Tests",
                    Treatment = "Tests"
                },
                new MedicalAppointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = 5245687,
                    ConsultationDate = DateTime.UtcNow,
                    Symptoms = "Tests",
                    Diagnosis = "Tests",
                    Treatment = "Tests"
                }
            };

            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(medicalAppointments);

            //Act
            var result = await _controller.GetAllMedicalAppointments();

            //Assert
            Assert.IsAssignableFrom<IEnumerable<MedicalAppointmentDto>>(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetAllMedicalAppointments_ReturnsEmptyList_WhenNoMedicalAppointmetsExist()
        {
            //Arrange
            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<MedicalAppointment>());

            //Act
            var result = await _controller.GetAllMedicalAppointments();

            //Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllMedicalAppointmentsMapsAllPropertiesCorrectly()
        {
            //Arrange
            var medicalAppointment = new MedicalAppointment
            {
                Id = Guid.NewGuid(),
                PatientId = 5245687,
                ConsultationDate = DateTime.UtcNow,
                Symptoms = "Tests",
                Diagnosis = "Tests",
                Treatment = "Tests"
            };

            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<MedicalAppointment> { medicalAppointment });

            //Act
            var result = (await _controller.GetAllMedicalAppointments()).First();

            //Assert
            Assert.Equal(medicalAppointment.Id, result.Id);
            Assert.Equal(medicalAppointment.PatientId, result.PatientId);
            Assert.Equal(medicalAppointment.ConsultationDate, result.ConsultationDate);
            Assert.Equal(medicalAppointment.Symptoms, result.Symptoms);
            Assert.Equal(medicalAppointment.Diagnosis, result.Diagnosis);
            Assert.Equal(medicalAppointment.Treatment, result.Treatment);
        }
    }
}
