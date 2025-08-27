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
    public class CreateTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly LabTestsController _controller;

        public CreateTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new LabTestsController(_mockMediator.Object);
        }

        [Fact]
        public async Task Create_WhenCalledWithValidRequest_ReturnsOkWithLabTest()
        {
            // Arrange
            var request = new CreateLabTestRequest
            {
                TestName = "Test",
                ReferenceRange = "Test",
                SpecimenType = 1
            };
            var createdLabTestResponse = new LabTestResponse
            {
                LabTestId = Guid.NewGuid(),
                TestName = "Blood Test",
                SpecimenType = SpecimenType.Blood,
                ReferenceRange = "5-10 mg/dL"
            };


            _mockMediator.Setup(m => m.Send(It.IsAny<CreateLabTest.Command>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(createdLabTestResponse.LabTestId);

            // Act
            var result = await _controller.Create(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult?.Value.Should().BeEquivalentTo(createdLabTestResponse.LabTestId);

            _mockMediator.Verify(m => m.Send(
                It.Is<CreateLabTest.Command>(cmd =>
                    cmd.TestName == request.TestName &&
                    cmd.ReferenceRange == request.ReferenceRange &&
                    cmd.SpecimenType == request.SpecimenType),
                It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Create_WhenMediatorReturnsError_ReturnsProblemDetails()
        {
            // Arrange
            var request = new CreateLabTestRequest
            {
                TestName = "Test",
                ReferenceRange = "Test",
                SpecimenType = 1
            };
            var error = Error.Validation("ReferenceRange.Invalid", "The reference range cannot be null.");

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateLabTest.Command>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(error);

            // Act
            var result = await _controller.Create(request);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult?.StatusCode.Should().Be(500); 
            var problemDetails = objectResult?.Value as ProblemDetails;
            problemDetails.Should().NotBeNull();
            problemDetails.Detail.Should().Contain("The reference range cannot be null.");
        }
    }
}
