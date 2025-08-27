using ErrorOr;
using FluentValidation;
using LabManagementService.Entities;
using LabManagementService.Repositories;
using MediatR;

namespace LabManagementService.Features.LabOrders
{
    public static class UpdateLabResults
    {
        public record Command(
            Guid LabOrderId,
            Guid LabResultId
        ) : IRequest<ErrorOr<Unit>>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.LabOrderId).NotEmpty();
                RuleFor(x => x.LabResultId).NotEmpty();
            }
        }

        internal sealed class Handler : IRequestHandler<Command, ErrorOr<Unit>>
        {
            private readonly IValidator<Command> _validator;
            private readonly IRepository<LabOrder> _context;
            private readonly IRepository<LabResult> _labResultContext;

            public Handler(IValidator<Command> validator, IRepository<LabOrder> context, IRepository<LabResult> labResultContext)
            {
                _validator = validator ?? throw new ArgumentNullException(nameof(validator));
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _labResultContext = labResultContext ?? throw new ArgumentNullException(nameof(labResultContext));
            }

            public async Task<ErrorOr<Unit>> Handle(Command command, CancellationToken cancellationToken)
            {
                var validatorResult = _validator.Validate(command);
                if (!validatorResult.IsValid)
                    return Error.Validation("Validation error", "Invalid input data");

                var labOrder = await _context.GetAsync(command.LabOrderId);

                if (labOrder == null)
                    return Error.NotFound("Lab order not found", "The lab order with the given Id was not found");

                var labResult = await _labResultContext.GetAsync(command.LabResultId);

                if (labResult == null)
                    return Error.NotFound("Lab result not found", "The lab result with the given Id was not found");

                LabOrder UpdateLabResult = LabOrder.UpdateLabResults(
                    labOrder,
                    labResult,
                    command.LabResultId);

                _context.UpdateAsync(UpdateLabResult);

                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
