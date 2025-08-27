using Moq;
using PatientManagementService.Entities;
using PatientManagementService.Enums;
using PatientManagementService.Repositories;

namespace Tests;

[TestClass]
public class GetAllPatientsTest
{
    [TestMethod]
    public async Task CreatePatientMethod_WhenAllDataIsCorrect_ShouldReturnPatientDto()
    {
        //Arrange
        var mockPatientRepository = new Mock<IPatientRepository>();

        var patients = new List<Patient>
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
                EmergencyContactPhone = "70725926",
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
                EmergencyContactPhone = "70725926",
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
                EmergencyContactPhone = "70725926",
            },
        };
    }
}
