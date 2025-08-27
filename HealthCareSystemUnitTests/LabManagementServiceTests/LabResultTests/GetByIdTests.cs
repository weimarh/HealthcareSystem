using ErrorOr;
using FluentAssertions;
using LabManagementService.Contracts.LabResults;
using LabManagementService.Contracts.LabTests;
using LabManagementService.Controllers;
using LabManagementService.Entities;
using LabManagementService.Features.LabResults;
using LabManagementService.Features.LabTests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthCareSystemUnitTests.LabManagementServiceTests.LabResultTests
{
    public class GetByIdTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly LabResultsController _controller;

        public GetByIdTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new LabResultsController(_mockMediator.Object);
        }

        [Fact]
        public async Task GetById_WhenCalledWithValidId_ReturnsOkWithLabResult()
        {
            //Arrange
            var labResultId = Guid.NewGuid();

            var labResultResponse = new LabResultResponse
            {
                LabResultId = labResultId,
                LabOrderId = Guid.NewGuid(),
                PatientId = 5207907,
                LabTestName = "test",
                LabStartDate = DateTime.Now,
                Value = "test",
                ReportedDate = DateTime.Now,
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetLabResult.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(labResultResponse);

            //Act
            var result = await _controller.GetById(labResultId);

            //Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult?.Value.Should().BeEquivalentTo(labResultResponse);

            _mockMediator.Verify(m => m.Send(It.Is<GetLabResult.Query>(q => q.Id == labResultId), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetById_WhenLabResultNotFound_ReturnsProblemDetails()
        {
            //Arrange
            var labResultId = Guid.NewGuid();

            var error = Error.NotFound("LabResult.NotFound", "The requested lab result was not found.");

            _mockMediator.Setup(m => m.Send(It.IsAny<GetLabResult.Query>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(error);

            // Act
            var result = await _controller.GetById(labResultId);

            //Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;

            objectResult?.StatusCode.Should().Be(500);
            var problemDetails = objectResult?.Value as ProblemDetails;
            problemDetails.Should().NotBeNull();
            problemDetails.Detail.Should().Contain("The requested lab result was not found.");
        }
    }
}
