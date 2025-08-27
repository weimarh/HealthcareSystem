using ErrorOr;
using FluentValidation;
using LabManagementService.Entities;
using LabManagementService.Enums;
using LabManagementService.Repositories;
using MediatR;

namespace LabManagementService.Features.LabOrders
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
                RuleFor(x => x.Status).GreaterThan(-1).LessThan(4);
                RuleFor(x => x.OrderedBy).NotEmpty();
            }
        }

        internal sealed class Handler : IRequestHandler<Command, ErrorOr<Guid>>
        {
            private readonly IRepository<LabOrder> _context;
            private readonly IRepository<LabTest> _labTestContext;
            private readonly IValidator<Command> _validator;

            public Handler(IRepository<LabOrder> context, IValidator<Command> validator, IRepository<LabTest> labTestContext)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _validator = validator ?? throw new ArgumentNullException(nameof(validator));
                _labTestContext = labTestContext ?? throw new ArgumentNullException(nameof(labTestContext));
            }

            public async Task<ErrorOr<Guid>> Handle(Command command, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(command);

                if (!validationResult.IsValid)
                    return Error.Validation("Validation error", "Invalid input data");

                var labTest = await _labTestContext.GetAsync(command.LabTestId);

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


                await _context.CreateAsync(labOrder);

                await _context.SaveChangesAsync();

                return labOrder.LabOrderId;
            }
        }
    }
}
