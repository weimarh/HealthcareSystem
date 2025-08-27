using ErrorOr;
using LabManagementService.Contracts.LabOrders;
using LabManagementService.Features.LabOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LabManagementService.Controllers
{
    [Route("api/orders")]
    public class LabOrdersController : ApiController
    {
        private readonly IMediator _mediator;

        public LabOrdersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var labOrders = await _mediator.Send(new GetAllLabOrders.Query());

            return labOrders.Match(
                labOrders => Ok(labOrders), 
                errors => Problem(string.Join(", ", errors.Select(e => e.Description))));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var labOrder = await _mediator.Send(new GetLabOrder.Query(id));

            return labOrder.Match(
                labOrder => Ok(labOrder),
                errors => Problem(string.Join(", ", errors.Select(e => e.Description))));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLabOrderRequest request)
        {
            var result = await _mediator.Send(new CreateLabOrder.Command(request.PatientId, request.LabTestId, request.OrderDate, request.Status, request.OrderedBy));

            return result.Match(
                labOrder => Ok(labOrder), 
                errors => Problem(string.Join(", ", errors.Select(e => e.Description))));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id,  [FromBody] UpdateLabOrderRequest request)
        {
            if (id != request.LabOrderId)
            {
                List<Error> errors = new()
                {
                    Error.NotFound()
                };

                return Problem(string.Join(", ", errors.Select(e => e.Description)));
            }

            var result = await _mediator.Send(new UpdateLabOrder.Command(request.LabOrderId, request.PatientId, request.LabTestId, request.OrderDate, request.Status, request.OrderedBy));

            return result.Match(
                labOrder => Ok(labOrder),
                errors => Problem(string.Join(", ", errors.Select(e => e.Description))));
        }

        [HttpPut("results/{id}")]
        public async Task<IActionResult> UpdateLabResult(Guid id, [FromBody] UpdateLabResultsRequest request)
        {
            if (id != request.LabOrderId)
            {
                List<Error> errors = new()
                {
                    Error.NotFound()
                };

                return Problem(string.Join(", ", errors.Select(e => e.Description)));
            }

            var result = await _mediator.Send(new UpdateLabResults.Command(request.LabOrderId,request.LabResultId));

            return result.Match(
                labOrder => Ok(labOrder),
                errors => Problem(string.Join(", ", errors.Select(e => e.Description))));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteLabOrder.Command(id));

            return result.Match(
                labOrder => Ok(labOrder),
                errors => Problem(string.Join(", ", errors.Select(e => e.Description))));
        }
    }
}
