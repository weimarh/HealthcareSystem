using ErrorOr;
using FluentAssertions;
using LabManagementService.Contracts.LabResults;
using LabManagementService.Controllers;
using LabManagementService.Features.LabResults;
using LabManagementService.Features.LabTests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.LabManagementServiceTests.LabResultTests
{
    public class CreateTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly LabResultsController _controller;

        public CreateTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new LabResultsController(_mockMediator.Object);
        }

        [Fact]
        public async Task Create_WhenCalledWithValidRequest_ReturnsOkWithLabResult()
        {
            //Arrange
            var request = new CreateLabResultRequest
            {
                LabOrderId = Guid.NewGuid(),
                Value = "Test",
                ReportedDate = DateTime.UtcNow,
            };

            var createdLabResultResponse = new LabResultResponse
            {
                LabResultId = Guid.NewGuid(),
                LabOrderId = Guid.NewGuid(),
                PatientId = 5207907,
                LabTestName = "test",
                LabStartDate = DateTime.Now,
                Value = "test",
                ReportedDate = DateTime.Now,
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateLabResult.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdLabResultResponse.LabResultId);

            //Act
            var result = await _controller.Create(request);

            //Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult?.Value.Should().BeEquivalentTo(createdLabResultResponse.LabResultId);

            _mockMediator.Verify(m => m.Send(
                It.Is<CreateLabResult.Command>(cmd =>
                    cmd.LabOrderId == request.LabOrderId &&
                    cmd.Value == request.Value &&
                    cmd.ReportedDate == request.ReportedDate),
                It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Create_WhenMediatorReturnsError_ReturnsProblemDetails()
        {
            //Arrange
            var request = new CreateLabResultRequest
            {
                LabOrderId = Guid.NewGuid(),
                Value = "Test",
                ReportedDate = DateTime.UtcNow,
            };

            var error = Error.Validation("ReferenceRange.Invalid", "The reference range cannot be null.");

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateLabResult.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(error);

            //Act
            var result = await _controller.Create(request);

            //Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult?.StatusCode.Should().Be(500);
            var problemDetails = objectResult?.Value as ProblemDetails;
            problemDetails.Should().NotBeNull();
            problemDetails.Detail.Should().Contain("The reference range cannot be null.");
        }
    }
}
