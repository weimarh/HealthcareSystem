using LabManagementService.Contracts.LabResults;
using LabManagementService.Features.LabResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LabManagementService.Controllers
{
    [Route("api/results")]
    public class LabResultsController : ApiController
    {
        private readonly IMediator _mediator;

        public LabResultsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var labResults = await _mediator.Send(new GetAllLabResults.Query());

            return labResults.Match(
                labResults => Ok(labResults),
                errors => Problem(string.Join(", ", errors.Select(e => e.Description))));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var labResult = await _mediator.Send(new GetLabResult.Query(id));

            return labResult.Match(
                labResult => Ok(labResult),
                errors => Problem(string.Join(", ", errors.Select(e => e.Description))));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateLabResultRequest request)
        {
            var result = await _mediator.Send(new CreateLabResult.Command(request.LabOrderId, request.Value, request.ReportedDate));

            return result.Match(
                labResult => Ok(labResult),
                errors => Problem(string.Join(", ", errors.Select(e => e.Description))));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateLabResultRequest request)
        {
            var result = await _mediator.Send(new UpdateLabResult.Command(request.LabResultId, request.Value, request.ReportedDate));

            return result.Match(
                labResult => Ok(labResult),
                errors => Problem(string.Join(", ", errors.Select(e => e.Description))));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteLabResult.Command(id));

            return result.Match(
                labResult => Ok(labResult),
                errors => Problem(string.Join(", ", errors.Select(e => e.Description))));
        }
    }
}
