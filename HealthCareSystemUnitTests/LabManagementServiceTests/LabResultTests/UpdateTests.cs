using ErrorOr;
using FluentAssertions;
using LabManagementService.Contracts.LabResults;
using LabManagementService.Controllers;
using LabManagementService.Features.LabResults;
using LabManagementService.Features.LabTests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.LabManagementServiceTests.LabResultTests
{
    public class UpdateTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly LabResultsController _controller;

        public UpdateTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new LabResultsController(_mockMediator.Object);
        }

        [Fact]
        public async Task Update_WhenCalledWithValidData_ReturnsOkWithUpdatedLabResult()
        {
            //Arrange
            var labResultId = Guid.NewGuid();
            var request = new UpdateLabResultRequest
            {
                LabResultId = labResultId,
                Value = "Test",
                ReportedDate = DateTime.UtcNow,
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateLabResult.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            //Act
            var result = await _controller.Update(request);

            //Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult?.Value.Should().BeEquivalentTo(Unit.Value);

            _mockMediator.Verify(m => m.Send(
                It.Is<UpdateLabResult.Command>(cmd =>
                    cmd.LabResultId == request.LabResultId &&
                    cmd.Value == request.Value &&
                    cmd.ReportedDate == request.ReportedDate),
                It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Update_WhenMediatorReturnsError_ReturnsProblemDetails()
        {
            //Arrange
            var labResultId = Guid.NewGuid();
            var request = new UpdateLabResultRequest
            {
                LabResultId = labResultId,
                Value = "Test",
                ReportedDate = DateTime.UtcNow,
            };

            var error = Error.NotFound("LabTest.NotFound", "The lab test to be updated was not found.");

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateLabResult.Command>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(error);

            //Act
            var result = await _controller.Update(request);

            //Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult?.StatusCode.Should().Be(500);
            var problemDetails = objectResult?.Value as ProblemDetails;
            problemDetails.Should().NotBeNull();
            problemDetails.Detail.Should().Contain("The lab test to be updated was not found.");
        }
    }
}
