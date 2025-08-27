using ErrorOr;
using FluentAssertions;
using LabManagementService.Contracts.LabResults;
using LabManagementService.Controllers;
using LabManagementService.Features.LabResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.LabManagementServiceTests.LabResultTests
{
    public class GetAllTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly LabResultsController _controller;

        public GetAllTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new LabResultsController(_mockMediator.Object);
        }

        [Fact]
        public async Task GetAll_WhenCalled_ReturnsOkWithLabResults()
        {
            //Arrange
            var labResultResponse = new List<LabResultResponse>
            {
                new LabResultResponse
                {
                    LabResultId = Guid.NewGuid(),
                    LabOrderId = Guid.NewGuid(),
                    PatientId = 5207907,
                    LabTestName = "test",
                    LabStartDate = DateTime.Now,
                    Value = "test",
                    ReportedDate = DateTime.Now,
                },
                new LabResultResponse
                {
                    LabResultId = Guid.NewGuid(),
                    LabOrderId = Guid.NewGuid(),
                    PatientId = 5207907,
                    LabTestName = "test",
                    LabStartDate = DateTime.Now,
                    Value = "test",
                    ReportedDate = DateTime.Now,
                },
                new LabResultResponse
                {
                    LabResultId = Guid.NewGuid(),
                    LabOrderId = Guid.NewGuid(),
                    PatientId = 5207907,
                    LabTestName = "test",
                    LabStartDate = DateTime.Now,
                    Value = "test",
                    ReportedDate = DateTime.Now,
                },
            }.AsReadOnly();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllLabResults.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(labResultResponse);

            //Act
            var result = await _controller.GetAll();

            //Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult?.Value.Should().BeEquivalentTo(labResultResponse);
            okResult?.Value.Should().BeAssignableTo<IReadOnlyList<LabResultResponse>>();

            _mockMediator.Verify(m => m.Send(It.IsAny<GetAllLabResults.Query>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetAll_WhenMediatorReturnsError_ReturnsProblemDetails()
        {
            // Arrange
            var error = Error.Failure("GetAll.Failed", "Could not retrieve lab tests.");

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllLabResults.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(error);

            //Act
            var result = await _controller.GetAll();

            //Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult?.StatusCode.Should().Be(500);
        }
    }
}
