using ErrorOr;
using FluentAssertions;
using LabManagementService.Controllers;
using LabManagementService.Features.LabOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.LabManagementServiceTests.LabOrderTests
{
    public class DeleteTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly LabOrdersController _controller;

        public DeleteTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new LabOrdersController(_mockMediator.Object);
        }

        [Fact]
        public async Task Delete_WhenCalledWithValidId_ReturnsOkWithSuccessResult()
        {
            //Arrange
            var labOrderId = Guid.NewGuid();
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteLabOrder.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            //Act
            var result = await _controller.Delete(labOrderId);

            //Assert
            result.Should().BeOfType<OkObjectResult>();

            _mockMediator.Verify(m => m.Send(
                It.Is<DeleteLabOrder.Command>(cmd => cmd.Id == labOrderId),
                It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Delete_WhenMediatorReturnsError_ReturnsProblemDetails()
        {
            //Arrange
            var labOrderId = Guid.NewGuid();
            var error = Error.NotFound("LabOrder.NotFound", "The lab order to be deleted was not found.");

            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteLabOrder.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(error);

            //Act
            var result = await _controller.Delete(labOrderId);

            //Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult?.StatusCode.Should().Be(500);
            var problemDetails = objectResult?.Value as ProblemDetails;
            problemDetails.Should().NotBeNull();
            problemDetails.Detail.Should().Contain("The lab order to be deleted was not found.");

            _mockMediator.Verify(m => m.Send(
                It.Is<DeleteLabOrder.Command>(cmd => cmd.Id == labOrderId),
                It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
