using ErrorOr;
using FluentAssertions;
using LabManagementService.Contracts.LabOrders;
using LabManagementService.Controllers;
using LabManagementService.Features.LabOrders;
using LabManagementService.Features.LabTests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.LabManagementServiceTests.LabOrderTests
{
    public class UpdateTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly LabOrdersController _controller;

        public UpdateTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new LabOrdersController(_mockMediator.Object);
        }

        [Fact]
        public async Task Update_WhenCalledWithValidData_ReturnsOkWithUpdatedLabOrder()
        {
            //Arrange
            var labOrderId = Guid.NewGuid();
            var request = new UpdateLabOrderRequest
            {
                LabOrderId = labOrderId,
                PatientId = 5207907,
                LabTestId = Guid.NewGuid(),
                OrderDate = DateTime.Now,
                Status = 1,
                OrderedBy = "Test"
            };

            var updatedLabOrderResponse = new LabOrderResponse
            {
                LabOrderId = labOrderId,
                PatientId = 5207907,
                OrderDate = DateTime.Now,
                Status = LabManagementService.Enums.Status.Completed,
                OrderedBy = "Test"
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateLabOrder.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            //Act
            var result = await _controller.Update(labOrderId, request);

            //Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult?.Value.Should().BeEquivalentTo(Unit.Value);

            _mockMediator.Verify(m => m.Send(
                It.Is<UpdateLabOrder.Command>(cmd => 
                cmd.PatientId == request.PatientId &&
                cmd.OrderedBy == request.OrderedBy),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Update_WhenMediatorReturnsError_ReturnsProblemDetails()
        {
            //Arrange
            var labOrderId = Guid.NewGuid();
            var request = new UpdateLabOrderRequest
            {
                LabOrderId = labOrderId,
                PatientId = 5207907,
                LabTestId = Guid.NewGuid(),
                OrderDate = DateTime.Now,
                Status = 1,
                OrderedBy = "Test"
            };

            var error = Error.NotFound("LabOrder.NotFound", "The lab Order to be updated was not found.");

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateLabOrder.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(error);

            //Act
            var result = await _controller.Update(labOrderId, request);

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
            var request = new UpdateLabOrderRequest
            {
                LabOrderId = Guid.NewGuid(),
                PatientId = 5207907,
                LabTestId = Guid.NewGuid(),
                OrderDate = DateTime.Now,
                Status = 1,
                OrderedBy = "Test"
            };

            //Act
            var result = await _controller.Update(labOrderId, request);

            //Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult?.StatusCode.Should().Be(500);
            var problemDetails = objectResult?.Value as ProblemDetails;
            problemDetails.Should().NotBeNull();

            _mockMediator.Verify(m => m.Send(It.IsAny<UpdateLabOrder.Command>(), It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
