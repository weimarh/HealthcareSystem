using ErrorOr;
using FluentValidation;
using LabManagementService.Entities;
using LabManagementService.Repositories;
using MediatR;

namespace LabManagementService.Features.LabResults
{
    public static class CreateLabResult
    {
        public record Command(
            Guid LabOrderId,
            string Value,
            DateTime ReportedDate
        ) : IRequest<ErrorOr<Guid>>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.LabOrderId).NotEmpty();
                RuleFor(x => x.Value).MaximumLength(40).NotEmpty();
                RuleFor(x => x.ReportedDate).NotEmpty();
            }
        }

        internal sealed class Handler : IRequestHandler<Command, ErrorOr<Guid>>
        {
            private readonly IValidator<Command> _validator;
            private readonly IRepository<LabResult> _context;
            private readonly IRepository<LabOrder> _labOrderContext;

            public Handler(IValidator<Command> validator, IRepository<LabResult> context, IRepository<LabOrder> labOrderContext)
            {
                _validator = validator ?? throw new ArgumentNullException(nameof(validator));
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _labOrderContext = labOrderContext ?? throw new ArgumentNullException(nameof(labOrderContext));
            }

            public async Task<ErrorOr<Guid>> Handle(Command command, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(command);

                if (!validationResult.IsValid)
                    return Error.Validation("Validation error", "Invalid input data");

                var labOrder = await _labOrderContext.GetAsync(command.LabOrderId);

                if (labOrder != null)
                    return Error.NotFound("Lab order not found", "The specified Lab order does not exist.");

                var labResult = new LabResult
                {
                    LabResultId = Guid.NewGuid(),
                    LabOrderId = command.LabOrderId,
                    LabOrder = labOrder,
                    Value = command.Value,
                    ReportedDate = command.ReportedDate,
                };

                await _context.CreateAsync(labResult);

                await _context.SaveChangesAsync();

                return labResult.LabResultId;
            }
        }
    }
}

