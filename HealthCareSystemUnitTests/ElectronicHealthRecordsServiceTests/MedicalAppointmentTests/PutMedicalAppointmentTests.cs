using ElectronicHealthRecordsService;
using ElectronicHealthRecordsService.Controllers;
using ElectronicHealthRecordsService.Entities;
using ElectronicHealthRecordsService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.ElectronicHealthRecordsServiceTests.MedicalAppointmentTests
{
    public class PutMedicalAppointmentTests
    {
        private readonly Mock<IRepository<MedicalAppointment>> _mockRepository;
        private readonly MedicalAppointmentsController _controller;
        private readonly Mock<IPatientRepository> _mockPatientRepository;

        public PutMedicalAppointmentTests()
        {
            _mockRepository = new Mock<IRepository<MedicalAppointment>>();
            _mockPatientRepository = new Mock<IPatientRepository>();
            _controller = new MedicalAppointmentsController(_mockRepository.Object, _mockPatientRepository.Object);
        }

        [Fact]
        public async Task PutMedicalAppointment_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            //Arrange
            var medicalAppointmentId = new Guid("90a9b853-8cd0-4781-95ba-7253c8f38beb");
            var existingMedicalAppointment = new MedicalAppointment { Id = medicalAppointmentId };
            var updatedMedicalAppointment = new UpdateMedicalAppointmentDto(
                DateTime.UtcNow,
                "Tests",
                "Tests",
                "Tests");

            _mockRepository.Setup(repo => repo.GetAsync(medicalAppointmentId))
                .ReturnsAsync(existingMedicalAppointment);
            _mockRepository.Setup(repo => repo.UpdateAsync(existingMedicalAppointment))
                .Returns(Task.CompletedTask);

            //Act
            var result = await _controller.PutMedicalAppointment(medicalAppointmentId, updatedMedicalAppointment);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutMedicalAppointment_ReturnsNotFound_WhenMedicalAppointmentDoesNotExist()
        {
            //Assert
            var medicalAppointmentId = new Guid("90a9b853-8cd0-4781-95ba-7253c8f38beb");
            _mockRepository.Setup(repo => repo.GetAsync(It.IsAny<Guid>())).ReturnsAsync((MedicalAppointment)null!);

            var updatedMedicalAppointment = new UpdateMedicalAppointmentDto(
                DateTime.UtcNow,
                "Tests",
                "Tests",
                "Tests");

            //Act
            var result = await _controller.PutMedicalAppointment(medicalAppointmentId, updatedMedicalAppointment);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
