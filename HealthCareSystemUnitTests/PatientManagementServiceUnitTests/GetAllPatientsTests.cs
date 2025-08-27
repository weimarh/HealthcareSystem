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

        public GetAllPatientsTests()
        {
            _mockPatientRepository = new Mock<IPatientRepository>();
            _patientsController = new PatientsController(_mockPatientRepository.Object);
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
            Assert.IsAssignableFrom<IEnumerable<PatientDto>>(result);
            Assert.Equal(3, result.Count());
            Assert.Contains(result, p => p.FirstName == "Test1");
            Assert.Contains(result, p => p.FirstName == "Test2");
            Assert.Contains(result, p => p.FirstName == "Test3");
        }

        [Fact]
        public async Task GetAllPatients_ReturnsEmptyList_WhenNoPatientsExist()
        {
            // Arrange
            _mockPatientRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Patient>());

            // Act
            var result = await _patientsController.GetAllPatients();

            // Assert
            Assert.Empty(result);
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

        [Fact]
        public async Task GetAllPatients_MapsAllPropertiesCorrectly()
        {
            // Arrange
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

            _mockPatientRepository.Setup(repo => repo.GetAllAsync())
                    .ReturnsAsync(new List<Patient> { testPatient });

            // Act
            var result = (await _patientsController.GetAllPatients()).First();

            // Assert
            Assert.Equal(testPatient.Id, result.Id);
            Assert.Equal(testPatient.Complement, result.Complement);
            Assert.Equal(testPatient.FirstName, result.FirstName);
            Assert.Equal(testPatient.LastName, result.LastName);
            Assert.Equal(testPatient.Gender.ToString(), result.Gender);
            Assert.Equal(testPatient.HomeAddress, result.HomeAddress);
            Assert.Equal(testPatient.EmergencyContactName, result.EmergencyContactName);
            Assert.Equal(testPatient.EmergencyContactPhone, result.EmergencyContactPhone);
        }
    }
}
