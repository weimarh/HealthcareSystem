using ErrorOr;
using LabManagementService.Contracts.LabTests;
using LabManagementService.Features.LabTests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LabManagementService.Controllers
{
    [Route("api/tests")]
    public class LabTestsController : ApiController
    {
        private readonly IMediator _mediator;

        public LabTestsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var labTests = await _mediator.Send(new GetAllLabTests.Query());

            return labTests.Match(
                labTests => Ok(labTests), 
                errors => Problem(string.Join(", ", errors.Select(e => e.Description))));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var labTest = await _mediator.Send(new GetLabTest.Query(id));

            return labTest.Match(
                labTest => Ok(labTest), 
                errors => Problem(string.Join(", ", errors.Select(e => e.Description))));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateLabTestRequest request)
        {
            var result = await _mediator.Send(new CreateLabTest.Command(request.TestName, request.ReferenceRange, request.SpecimenType));

            return result.Match(
                labTest => Ok(labTest),
                errors => Problem(string.Join(", ", errors.Select(e => e.Description))));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody]UpdateLabTestRequest request)
        {
            if (id != request.LabTestId)
            {
                List<Error> errors = new()
                {
                    Error.NotFound()
                };

                return Problem(string.Join(", ", errors.Select(e => e.Description)));
            }

            var result = await _mediator.Send(new UpdateLabTest.Command(request.LabTestId, request.TestName, request.ReferenceRange, request.SpecimenType));

            return result.Match(
                labTest => Ok(labTest),
                errors => Problem(string.Join(", ", errors.Select(e => e.Description))));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteLabTest.Command(id));

            return result.Match(
                labTest => Ok(labTest),
                errors => Problem(string.Join(", ", errors.Select(e => e.Description))));
        }
    }
}
