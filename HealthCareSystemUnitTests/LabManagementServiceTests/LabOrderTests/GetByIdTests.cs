using ErrorOr;
using FluentAssertions;
using LabManagementService.Contracts.LabOrders;
using LabManagementService.Contracts.LabTests;
using LabManagementService.Controllers;
using LabManagementService.Enums;
using LabManagementService.Features.LabOrders;
using LabManagementService.Features.LabTests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.LabManagementServiceTests.LabOrderTests
{
    public class GetByIdTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly LabOrdersController _controller;

        public GetByIdTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new LabOrdersController(_mockMediator.Object);
        }

        [Fact]
        public async Task GetById_WhenCalledWithValidId_ReturnsOkWithLabOrders()
        {
            //Arrange
            var labOrderId = Guid.NewGuid();

            var labOrderResponse = new LabOrderResponse
            {
                LabOrderId = Guid.NewGuid(),
                PatientId = 5207907,
                OrderDate = DateTime.Now,
                Status = Status.Pending,
                OrderedBy = "Doctor"
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetLabOrder.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(labOrderResponse);

            //Act
            var result = await _controller.GetById(labOrderId);

            //Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult?.Value.Should().BeEquivalentTo(labOrderResponse);

            _mockMediator.Verify(m => m.Send(It.Is<GetLabOrder.Query>(q => q.Id == labOrderId), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetById_WhenLabOrderNotFound_ReturnsProblemDetails()
        {
            // Arrange
            var labOrderId = Guid.NewGuid();

            var error = Error.NotFound("LabOrder.NotFound", "The requested lab order was not found.");

            _mockMediator.Setup(m => m.Send(It.IsAny<GetLabOrder.Query>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(error);

            // Act
            var result = await _controller.GetById(labOrderId);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;

            objectResult?.StatusCode.Should().Be(500);
            var problemDetails = objectResult?.Value as ProblemDetails;
            problemDetails.Should().NotBeNull();
            problemDetails.Detail.Should().Contain("The requested lab order was not found.");
        }
    }
}
