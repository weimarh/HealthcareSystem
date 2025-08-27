using ErrorOr;
using FluentAssertions;
using LabManagementService.Controllers;
using LabManagementService.Entities;
using LabManagementService.Features.LabResults;
using LabManagementService.Features.LabTests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.LabManagementServiceTests.LabResultTests
{
    public class DeleteTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly LabResultsController _controller;

        public DeleteTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new LabResultsController(_mockMediator.Object);
        }

        [Fact]
        public async Task Delete_WhenCalledWithValidId_ReturnsOkWithSuccessResult()
        {
            var labResultId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteLabResult.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.Delete(labResultId);

            //Assert
            result.Should().BeOfType<OkObjectResult>();

            _mockMediator.Verify(m => m.Send(
                It.Is<DeleteLabResult.Command>(cmd => cmd.Id == labResultId),
                It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Delete_WhenMediatorReturnsError_ReturnsProblemDetails()
        {
            var labResultId = Guid.NewGuid();
            var error = Error.NotFound("LabResult.NotFound", "The lab result to be deleted was not found.");

            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteLabResult.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(error);

            // Act
            var result = await _controller.Delete(labResultId);

            //Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult?.StatusCode.Should().Be(500);
            var problemDetails = objectResult?.Value as ProblemDetails;
            problemDetails.Should().NotBeNull();
            problemDetails.Detail.Should().Contain("The lab result to be deleted was not found.");

            _mockMediator.Verify(m => m.Send(
                It.Is<DeleteLabResult.Command>(cmd => cmd.Id == labResultId),
                It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
