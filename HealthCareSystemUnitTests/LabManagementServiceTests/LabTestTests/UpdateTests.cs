using ErrorOr;
using FluentAssertions;
using LabManagementService.Contracts.LabTests;
using LabManagementService.Controllers;
using LabManagementService.Enums;
using LabManagementService.Features.LabTests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.LabManagementServiceTests.LabTestTests
{
    public class UpdateTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly LabTestsController _controller;

        public UpdateTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new LabTestsController(_mockMediator.Object);
        }

        [Fact]
        public async Task Update_WhenCalledWithValidData_ReturnsOkWithUpdatedLabTest()
        {
            // Arrange
            var labTestId = Guid.NewGuid();
            var request = new UpdateLabTestRequest
            {
                LabTestId = labTestId,
                TestName = "Test",
                ReferenceRange = "Test",
                SpecimenType = 1
            };

            var updatedLabTestResponse = new LabTestResponse
            {
                LabTestId = Guid.NewGuid(),
                TestName = "Blood Test",
                SpecimenType = SpecimenType.Blood,
                ReferenceRange = "5-10 mg/dL"
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateLabTest.Command>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.Update(labTestId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult?.Value.Should().BeEquivalentTo(Unit.Value);

            _mockMediator.Verify(m => m.Send(
                It.Is<UpdateLabTest.Command>(cmd =>
                    cmd.TestName == request.TestName &&
                    cmd.ReferenceRange == request.ReferenceRange &&
                    cmd.SpecimenType == request.SpecimenType),
                It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Update_WhenMediatorReturnsError_ReturnsProblemDetails()
        {
            // Arrange
            var labTestId = Guid.NewGuid();
            var request = new UpdateLabTestRequest
            {
                LabTestId = labTestId,
                TestName = "Test",
                ReferenceRange = "Test",
                SpecimenType = 1
            };
            // Create a not found error instance.
            var error = Error.NotFound("LabTest.NotFound", "The lab test to be updated was not found.");

            // Set up the mock mediator to return the failed result with the not found error.
            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateLabTest.Command>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(error);

            // Act
            var result = await _controller.Update(labTestId, request);

            // Assert
            // The result should be a ProblemDetails result.
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult?.StatusCode.Should().Be(500); 
            var problemDetails = objectResult?.Value as ProblemDetails;
            problemDetails.Should().NotBeNull();
            problemDetails.Detail.Should().Contain("The lab test to be updated was not found.");
        }

        [Fact]
        public async Task Update_WhenRouteIdDoesNotMatchBodyId_ReturnsProblemDetails()
        {
            // Arrange
            var routeId = Guid.NewGuid();
            var request = new UpdateLabTestRequest
            {
                LabTestId = Guid.NewGuid(),
                TestName = "Test",
                ReferenceRange = "Test",
                SpecimenType = 1
            };

            // Act
            var result = await _controller.Update(routeId, request);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult?.StatusCode.Should().Be(500); 
            var problemDetails = objectResult?.Value as ProblemDetails;
            problemDetails.Should().NotBeNull();

            _mockMediator.Verify(m => m.Send(It.IsAny<UpdateLabTest.Command>(), It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
