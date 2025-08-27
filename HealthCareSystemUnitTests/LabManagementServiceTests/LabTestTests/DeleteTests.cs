using ErrorOr;
using FluentAssertions;
using LabManagementService.Controllers;
using LabManagementService.Features.LabTests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.LabManagementServiceTests.LabTestTests
{
    public class DeleteTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly LabTestsController _controller;

        public DeleteTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new LabTestsController(_mockMediator.Object);
        }

        [Fact]
        public async Task Delete_WhenCalledWithValidId_ReturnsOkWithSuccessResult()
        {
            // Arrange
            var labTestId = Guid.NewGuid();
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteLabTest.Command>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.Delete(labTestId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            _mockMediator.Verify(m => m.Send(
                It.Is<DeleteLabTest.Command>(cmd => cmd.Id == labTestId),
                It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Delete_WhenMediatorReturnsError_ReturnsProblemDetails()
        {
            // Arrange
            var labTestId = Guid.NewGuid();
            var error = Error.NotFound("LabTest.NotFound", "The lab test to be deleted was not found.");

            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteLabTest.Command>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(error);

            // Act
            var result = await _controller.Delete(labTestId);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult?.StatusCode.Should().Be(500); 
            var problemDetails = objectResult?.Value as ProblemDetails;
            problemDetails.Should().NotBeNull();
            problemDetails.Detail.Should().Contain("The lab test to be deleted was not found.");

            _mockMediator.Verify(m => m.Send(
                It.Is<DeleteLabTest.Command>(cmd => cmd.Id == labTestId),
                It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
