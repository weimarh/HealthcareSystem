using ErrorOr;
using FluentValidation;
using LabManagementService.Entities;
using LabManagementService.Enums;
using LabManagementService.Repositories;
using MediatR;

namespace LabManagementService.Features.LabTests
{
    public static class UpdateLabTest
    {
        public record Command(
            Guid LabTestId,
            string TestName,
            string ReferenceRange,
            int SpecimenType
        ) : IRequest<ErrorOr<Unit>>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.LabTestId).NotEmpty();
                RuleFor(x => x.TestName).MaximumLength(50).NotEmpty();
                RuleFor(x => x.ReferenceRange).MaximumLength(50).NotEmpty();
                RuleFor(x => x.SpecimenType).GreaterThan(-1).LessThan(5);
            }
        }

        internal sealed class Handler : IRequestHandler<Command, ErrorOr<Unit>>
        {
            private readonly IValidator<Command> _validator;
            private readonly IRepository<LabTest> _context;

            public Handler(IValidator<Command> validator, IRepository<LabTest> context)
            {
                _validator = validator ?? throw new ArgumentNullException(nameof(validator));
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<ErrorOr<Unit>> Handle(Command command, CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(command);

                if (!validationResult.IsValid)
                    return Error.Validation("Validation error", "Invalid input data");

                var labTest = await _context.GetAsync(command.LabTestId);

                if (labTest == null)
                    return Error.NotFound("Lab test not found", "The lab test with the given Id was not found");

                LabTest updatedLabTest = LabTest.UpdateLabTest(
                    labTest,
                    command.TestName,
                    (SpecimenType)command.SpecimenType,
                    command.ReferenceRange);

                _context.UpdateAsync(updatedLabTest);

                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
