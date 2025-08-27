using ErrorOr;
using FluentAssertions;
using LabManagementService.Contracts.LabOrders;
using LabManagementService.Controllers;
using LabManagementService.Enums;
using LabManagementService.Features.LabOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.LabManagementServiceTests.LabOrderTests
{
    public class GetAllTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly LabOrdersController _controller;

        public GetAllTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new LabOrdersController(_mockMediator.Object);
        }

        [Fact]
        public async Task GetAll_WhenCalled_ReturnsOkWithLabOrders()
        {
            //Arrange
            var labOrderResponse = new List<LabOrderResponse>
            {
                new LabOrderResponse
                {
                    LabOrderId = Guid.NewGuid(),
                    PatientId = 5207907,
                    OrderDate = DateTime.Now,
                    Status = Status.Pending,
                    OrderedBy = "Doctor"
                },
                new LabOrderResponse
                {
                    LabOrderId = Guid.NewGuid(),
                    PatientId = 5207907,
                    OrderDate = DateTime.Now,
                    Status = Status.Pending,
                    OrderedBy = "Doctor"
                },
                new LabOrderResponse
                {
                    LabOrderId = Guid.NewGuid(),
                    PatientId = 5207907,
                    OrderDate = DateTime.Now,
                    Status = Status.Pending,
                    OrderedBy = "Doctor"
                }
            }.AsReadOnly();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllLabOrders.Query>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(labOrderResponse);

            //Act
            var result = await _controller.GetAll();

            //Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult?.Value.Should().BeEquivalentTo(labOrderResponse);
            okResult?.Value.Should().BeAssignableTo<IReadOnlyList<LabOrderResponse>>();

            _mockMediator.Verify(m => m.Send(It.IsAny<GetAllLabOrders.Query>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetAll_WhenMediatorReturnsError_ReturnsProblemDetails()
        {
            // Arrange
            var error = Error.Failure("GetAll.Failed", "Could not retrieve lab tests.");

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllLabOrders.Query>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(error);

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult?.StatusCode.Should().Be(500); 
        }
    }    
}
