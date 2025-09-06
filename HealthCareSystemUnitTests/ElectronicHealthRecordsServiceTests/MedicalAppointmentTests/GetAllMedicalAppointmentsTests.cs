using ElectronicHealthRecordsService;
using ElectronicHealthRecordsService.Controllers;
using ElectronicHealthRecordsService.Entities;
using ElectronicHealthRecordsService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.ElectronicHealthRecordsServiceTests.MedicalAppointmentTests
{
    public class GetAllMedicalAppointmentsTests
    {
        private readonly Mock<IRepository<MedicalAppointment>> _mockRepository;
        private readonly MedicalAppointmentsController _controller;
        private readonly Mock<IPatientRepository> _mockPatientRepository;

        public GetAllMedicalAppointmentsTests()
        {
            _mockRepository = new Mock<IRepository<MedicalAppointment>>();
            _mockPatientRepository = new Mock<IPatientRepository>();
            _controller = new MedicalAppointmentsController(_mockRepository.Object, _mockPatientRepository.Object);
        }

        [Fact]
        public async Task GetAllMedicalAppointments_ReturnsListOfMedicalAppointments_WhenMedicalAppointmentsExistsWithMatchingPatients()
        {
            //Arrange
            var medicalAppointments = new List<MedicalAppointment>
            {
                new MedicalAppointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = 5207907,
                    ConsultationDate = DateTime.UtcNow,
                    Symptoms = "Tests",
                    Diagnosis = "Tests",
                    Treatment = "Tests"
                },
                new MedicalAppointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = 5296854,
                    ConsultationDate = DateTime.UtcNow,
                    Symptoms = "Tests",
                    Diagnosis = "Tests",
                    Treatment = "Tests"
                }
            };

            var patients = new List<Patient>
            {
                new Patient
                {
                    Id = 5207907,
                    Complement = "1-A",
                    FirstName = "Weimar",
                    LastName = "Barea",
                    Gender = "Male",
                },
                new Patient
                {
                    Id = 5296854,
                    Complement = "1-A",
                    FirstName = "Hermilene",
                    LastName = "Sanchez",
                    Gender = "Female",
                }
            };

            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(medicalAppointments);
            _mockPatientRepository.Setup(repo => repo.GetAllPatientsAsync()).ReturnsAsync(patients);

            //Act
            var result = await _controller.GetAllMedicalAppointments();

            //Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            var appointmentDtos = Assert.IsAssignableFrom<List<MedicalAppointmentDto>>(okResult.Value);

            Assert.Equal(2, appointmentDtos.Count);

            Assert.Equal("5207907", appointmentDtos[0].PatientId.ToString());
            Assert.Equal("1-A", appointmentDtos[0].Complement);
            Assert.Equal("Weimar", appointmentDtos[0].FirstName);
            Assert.Equal("Barea", appointmentDtos[0].LastName);
            Assert.Equal("Male", appointmentDtos[0].Gender);

            Assert.Equal("5296854", appointmentDtos[1].PatientId.ToString());
            Assert.Equal("1-A", appointmentDtos[1].Complement);
            Assert.Equal("Hermilene", appointmentDtos[1].FirstName);
            Assert.Equal("Sanchez", appointmentDtos[1].LastName);
            Assert.Equal("Female", appointmentDtos[1].Gender);
        }

        [Fact]
        public async Task GetAllMedicalAppointments_ReturnsListOfMedicalAppointments_WhenMedicalAppointmentsExistsWithNoPatients()
        {
            //Arrange
            var medicalAppointments = new List<MedicalAppointment>
            {
                new MedicalAppointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = 5207907,
                    ConsultationDate = DateTime.UtcNow,
                    Symptoms = "Tests",
                    Diagnosis = "Tests",
                    Treatment = "Tests"
                },
                new MedicalAppointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = 5296854,
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
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            var appointmentDtos = Assert.IsAssignableFrom<List<MedicalAppointmentDto>>(okResult.Value);

            Assert.Equal(2, appointmentDtos.Count);

            Assert.Equal("5207907", appointmentDtos[0].PatientId.ToString());
            Assert.Equal("", appointmentDtos[0].Complement);
            Assert.Equal("", appointmentDtos[0].FirstName);
            Assert.Equal("", appointmentDtos[0].LastName);
            Assert.Equal("", appointmentDtos[0].Gender);

            Assert.Equal("5296854", appointmentDtos[1].PatientId.ToString());
            Assert.Equal("", appointmentDtos[1].Complement);
            Assert.Equal("", appointmentDtos[1].FirstName);
            Assert.Equal("", appointmentDtos[1].LastName);
            Assert.Equal("", appointmentDtos[1].Gender);
        }

        [Fact]
        public async Task GetAllMedicalAppointments_ReturnsEmptyList_WhenNoMedicalAppointmetsExist()
        {
            //Arrange
            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<MedicalAppointment>());

            //Act
            var result = await _controller.GetAllMedicalAppointments();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var appointments = Assert.IsAssignableFrom<IEnumerable<MedicalAppointmentDto>>(okResult.Value);
            Assert.Empty(appointments);
        }

        [Fact]
        public async Task GetAllMedicalAppointments_CallsRepositoriesOnce()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<MedicalAppointment>());

            _mockPatientRepository.Setup(repo => repo.GetAllPatientsAsync())
                .ReturnsAsync(new List<Patient>());

            // Act
            await _controller.GetAllMedicalAppointments();

            // Assert
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
            _mockPatientRepository.Verify(repo => repo.GetAllPatientsAsync(), Times.Once);
        }
    }
}
