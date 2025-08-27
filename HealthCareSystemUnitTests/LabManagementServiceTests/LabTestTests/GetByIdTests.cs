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
    public class GetByIdTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly LabTestsController _controller;

        public GetByIdTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new LabTestsController(_mockMediator.Object);
        }

        [Fact]
        public async Task GetById_WhenCalledWithValidId_ReturnsOkWithLabTest()
        {
            // Arrange
            var labTestId = Guid.NewGuid();

            var labTestResponse = new LabTestResponse
            {
                LabTestId = Guid.NewGuid(),
                TestName = "Blood Test",
                SpecimenType = SpecimenType.Blood,
                ReferenceRange = "5-10 mg/dL"
            };


            _mockMediator.Setup(m => m.Send(It.IsAny<GetLabTest.Query>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(labTestResponse);

            // Act
            var result = await _controller.GetById(labTestId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult?.Value.Should().BeEquivalentTo(labTestResponse);

            _mockMediator.Verify(m => m.Send(It.Is<GetLabTest.Query>(q => q.Id == labTestId), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetById_WhenLabTestNotFound_ReturnsProblemDetails()
        {
            // Arrange
            var labTestId = Guid.NewGuid();

            var error = Error.NotFound("LabTest.NotFound", "The requested lab test was not found.");

            _mockMediator.Setup(m => m.Send(It.IsAny<GetLabTest.Query>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(error);

            // Act
            var result = await _controller.GetById(labTestId);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;

            objectResult?.StatusCode.Should().Be(500);
            var problemDetails = objectResult?.Value as ProblemDetails;
            problemDetails.Should().NotBeNull();
            problemDetails.Detail.Should().Contain("The requested lab test was not found.");
        }
    }
}
