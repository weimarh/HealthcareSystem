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
    public class GetAllTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly LabTestsController _controller;

        public GetAllTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new LabTestsController(_mockMediator.Object);
        }

        [Fact]
        public async Task GetAll_WhenCalled_ReturnsOkWithLabTests()
        {
            // Arrange
            var labTestsResponse = new List<LabTestResponse>
            {
                new LabTestResponse{ LabTestId = Guid.NewGuid(), TestName = "Blood Test", SpecimenType = SpecimenType.Blood, ReferenceRange = "5-10 mg/dL" },
                new LabTestResponse{ LabTestId = Guid.NewGuid(), TestName = "Blood Test", SpecimenType = SpecimenType.Blood, ReferenceRange = "5-10 mg/dL" }
            }.AsReadOnly();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllLabTests.Query>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(labTestsResponse);

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult?.Value.Should().BeEquivalentTo(labTestsResponse);
            okResult?.Value.Should().BeAssignableTo<IReadOnlyList<LabTestResponse>>();

            _mockMediator.Verify(m => m.Send(It.IsAny<GetAllLabTests.Query>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetAll_WhenMediatorReturnsError_ReturnsProblemDetails()
        {
            // Arrange
            var error = Error.Failure("GetAll.Failed", "Could not retrieve lab tests.");

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllLabTests.Query>(), It.IsAny<CancellationToken>()))
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
