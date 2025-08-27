using ErrorOr;
using FluentAssertions;
using LabManagementService.Contracts.LabOrders;
using LabManagementService.Controllers;
using LabManagementService.Features.LabOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.LabManagementServiceTests.LabOrderTests
{
    public class UpdateLabResultTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly LabOrdersController _controller;

        public UpdateLabResultTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new LabOrdersController(_mockMediator.Object);
        }

        [Fact]
        public async Task UpdateLabResult_WhenCalledWithValidData_ReturnsOkWithUpdatedLabOrder()
        {
            //Arrange
            var labOrderId = Guid.NewGuid();
            var request = new UpdateLabResultsRequest
            {
                LabOrderId = labOrderId,
                LabResultId = Guid.NewGuid(),
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateLabResults.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            //Act
            var result = await _controller.UpdateLabResult(labOrderId, request);

            //Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult?.Value.Should().BeEquivalentTo(Unit.Value);

            _mockMediator.Verify(m => m.Send(
                It.Is<UpdateLabResults.Command>(cmd =>
                cmd.LabResultId == request.LabResultId &&
                cmd.LabOrderId == request.LabOrderId),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Update_WhenMediatorReturnsError_ReturnsProblemDetails()
        {
            //Arrange
            var labOrderId = Guid.NewGuid();
            var request = new UpdateLabResultsRequest
            {
                LabOrderId = labOrderId,
                LabResultId = Guid.NewGuid(),
            };

            var error = Error.NotFound("LabOrder.NotFound", "The lab Order to be updated was not found.");

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateLabResults.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(error);

            //Act
            var result = await _controller.UpdateLabResult(labOrderId, request);

            //Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult?.StatusCode.Should().Be(500);
            var problemDetails = objectResult?.Value as ProblemDetails;
            problemDetails.Should().NotBeNull();
            problemDetails.Detail.Should().Contain("The lab Order to be updated was not found.");
        }

        [Fact]
        public async Task Update_WhenRouteIdDoesNotMatchBodyId_ReturnsProblemDetails()
        {
            //Arrange
            var labOrderId = Guid.NewGuid();
            var request = new UpdateLabResultsRequest
            {
                LabOrderId = Guid.NewGuid(),
                LabResultId = Guid.NewGuid(),
            };

            //Act
            var result = await _controller.UpdateLabResult(labOrderId, request);

            //Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult?.StatusCode.Should().Be(500);
            var problemDetails = objectResult?.Value as ProblemDetails;
            problemDetails.Should().NotBeNull();

            _mockMediator.Verify(m => m.Send(It.IsAny<UpdateLabResults.Command>(), It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
