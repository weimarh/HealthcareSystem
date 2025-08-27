using ElectronicHealthRecordsService.Controllers;
using ElectronicHealthRecordsService.Entities;
using ElectronicHealthRecordsService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.ElectronicHealthRecordsServiceTests.MedicalAppointmentTests
{
    public class DeleteMedicalAppointmentTests
    {
        private readonly Mock<IRepository<MedicalAppointment>> _mockRepository;
        private readonly MedicalAppointmentsController _controller;

        public DeleteMedicalAppointmentTests()
        {
            _mockRepository = new Mock<IRepository<MedicalAppointment>>();
            _controller = new MedicalAppointmentsController(_mockRepository.Object);
        }

        [Fact]
        public async Task DeleteMedicalAppointment_ReturnsNoContest_WhenIdIsValid()
        {
            //Arrange
            var id = new Guid("90a9b853-8cd0-4781-95ba-7253c8f38beb");
            _mockRepository.Setup(repo => repo.GetAsync(id)).ReturnsAsync(new MedicalAppointment { Id = id });
            _mockRepository.Setup(repo => repo.RemoveAsync(id)).Returns(Task.CompletedTask);

            //Act
            var result = await _controller.DeleteMedicalAppointment(id);

            //Assert
            Assert.IsType<NoContentResult>(result);
            _mockRepository.Verify(repo => repo.RemoveAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteMedicalAppointment_InvalidId_ReturnsNotFound()
        {
            //Arrange
            var id = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.RemoveAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

            //Act
            var result = await _controller.DeleteMedicalAppointment(id);

            //Assert
            Assert.IsType<NotFoundResult>(result);
            _mockRepository.Verify(repo => repo.RemoveAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}
