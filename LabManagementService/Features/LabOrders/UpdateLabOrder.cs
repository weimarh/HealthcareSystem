using ErrorOr;
using FluentValidation;
using LabManagementService.Entities;
using LabManagementService.Enums;
using LabManagementService.Repositories;
using MediatR;

namespace LabManagementService.Features.LabOrders
{
    public static class UpdateLabOrder
    {
        public record Command(
            Guid LabOrderId,
            int PatientId,
            Guid LabTestId,
            DateTime OrderDate,
            int Status,
            string OrderedBy
        ) : IRequest<ErrorOr<Unit>>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.LabOrderId).NotEmpty();
                RuleFor(x => x.PatientId).NotEmpty();
                RuleFor(x => x.LabTestId).NotEmpty();
                RuleFor(x => x.OrderDate).NotEmpty();
                RuleFor(x => x.Status).GreaterThan(-1).LessThan(4);
                RuleFor(x => x.OrderedBy).NotEmpty();
            }
        }

        internal sealed class Handler : IRequestHandler<Command, ErrorOr<Unit>>
        {
            private readonly IRepository<LabOrder> _context;
            private readonly IRepository<LabTest> _labTestContext;
            private readonly IValidator<Command> _validator;

            public Handler(IValidator<Command> validator, IRepository<LabOrder> context, IRepository<LabTest> labTestContext)
            {
                _validator = validator ?? throw new ArgumentNullException(nameof(validator));
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _labTestContext = labTestContext ?? throw new ArgumentNullException(nameof(labTestContext));
            }

            public async Task<ErrorOr<Unit>> Handle(Command command, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(command);

                if (!validationResult.IsValid)
                    return Error.Validation("Validation error", "Invalid input data");

                var labOrder = await _context.GetAsync(command.LabOrderId);

                if (labOrder == null)
                    return Error.NotFound("Lab order not found", "The lab order with the given Id was not found");

                var labTest = await _labTestContext.GetAsync(command.LabTestId);

                if (labTest == null)
                    return Error.NotFound("Lab Test not found", "The specified Lab Test does not exist.");

                LabOrder updatedLabOrder = LabOrder.UpdateLabOrder(
                    labOrder,
                    labTest,
                    command.LabTestId,
                    command.OrderDate,
                    (Status)command.Status,
                    command.OrderedBy);

                _context.UpdateAsync(updatedLabOrder);

                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
