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
    public class CreateTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly LabOrdersController _controller;

        public CreateTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new LabOrdersController(_mockMediator.Object);
        }

        [Fact]
        public async Task Create_WhenCalledWithValidRequest_ReturnsOkWithLabOrder()
        {
            //Arrange
            var request = new CreateLabOrderRequest
            {
                PatientId = 5207907,
                LabTestId = Guid.NewGuid(),
                OrderDate = DateTime.Now,
                Status = 1,
                OrderedBy = "Test"
            };

            var createdLabOrderResponse = new LabOrderResponse
            {
                LabOrderId = Guid.NewGuid(),
                PatientId = 5207907,
                OrderDate = DateTime.Now,
                Status = LabManagementService.Enums.Status.Completed,
                OrderedBy = "Test"
            };

            _mockMediator.Setup(repo => repo.Send(It.IsAny<CreateLabOrder.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdLabOrderResponse.LabOrderId);

            //Act
            var result = await _controller.Create(request);

            //Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult?.Value.Should().BeEquivalentTo(createdLabOrderResponse.LabOrderId);

            _mockMediator.Verify(m => m.Send(
                It.Is<CreateLabOrder.Command>(cmd =>
                cmd.PatientId == request.PatientId &&
                cmd.OrderDate == request.OrderDate),
                It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Create_WhenMediatorReturnsError_ReturnsProblemDetails()
        {
            //Arrange
            var request = new CreateLabOrderRequest
            {
                PatientId = 5207907,
                LabTestId = Guid.NewGuid(),
                OrderDate = DateTime.Now,
                Status = 1,
                OrderedBy = "Test"
            };

            var error = Error.Validation("ReferenceRange.Invalid", "The reference range cannot be null.");

            _mockMediator.Setup(repo => repo.Send(It.IsAny<CreateLabOrder.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(error);

            //Act
            var result = await _controller.Create(request);

            //Arrange
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult?.StatusCode.Should().Be(500);
            var problemDetails = objectResult?.Value as ProblemDetails;
            problemDetails.Should().NotBeNull();
            problemDetails.Detail.Should().Contain("The reference range cannot be null.");
        }
    }
}
