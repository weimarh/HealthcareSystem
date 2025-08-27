using Carter;
using ErrorOr;
using FluentValidation;
using LabResultsService.Contracts.LabOrders;
using LabResultsService.Database;
using LabResultsService.Entities;
using LabResultsService.Enums;
using LabResultsService.Features.LabOrders;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabResultsService.Features.LabOrders
{
    public static class CreateLabOrder
    {
        public record Command(
            int PatientId,
            Guid LabTestId,
            DateTime OrderDate,
            int Status,
            string OrderedBy
        ) : IRequest<ErrorOr<Guid>>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.PatientId).NotEmpty();
                RuleFor(x => x.LabTestId).NotEmpty();
                RuleFor(x => x.OrderDate).NotEmpty();
                RuleFor(x => x.Status).NotEmpty();
                RuleFor(x => x.OrderedBy).NotEmpty();
            }
        }

        internal sealed class Handler : IRequestHandler<Command, ErrorOr<Guid>>
        {
            private readonly ApplicationDbContext _context;
            private readonly IValidator<Command> _validator;

            public Handler(ApplicationDbContext context, IValidator<Command> validator)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            }

            public async Task<ErrorOr<Guid>> Handle(Command command, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(command);

                if (!validationResult.IsValid)
                    return Error.Validation("Validation error", "Invalid input data");

                var labTest = await _context.LabTests
                    .FirstOrDefaultAsync(labTest => labTest.LabTestId == command.LabTestId, cancellationToken);

                if (labTest == null)
                    return Error.NotFound("Lab Test not found", "The specified Lab Test does not exist.");

                if (command.Status < 0 || command.Status > 3)
                    return Error.NotFound("Status error", "The specified status is out of range.");

                var labOrder = new LabOrder
                (
                    Guid.NewGuid(),
                    command.PatientId,
                    labTest, 
                    command.LabTestId,
                    command.OrderDate,
                    (Status)command.Status,
                    command.OrderedBy
                );

                _context.LabOrders.Add(labOrder);

                await _context.SaveChangesAsync(cancellationToken);

                return labOrder.LabOrderId;
            }
        }
    }

    public class CreateLabOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/laborders", async ([FromBody]CreateLabOrderRequest request, [FromServices]ISender sender) =>
            {
                var command = request.Adapt<CreateLabOrder.Command>();

                var result = await sender.Send(command);

                return result.Match
                (
                    labOrderId => Results.Ok(labOrderId),
                    errors => Results.BadRequest(errors)
                );
            });
        }
    }
}

