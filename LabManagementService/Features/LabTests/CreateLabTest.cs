using ErrorOr;
using FluentValidation;
using LabManagementService.Entities;
using LabManagementService.Enums;
using LabManagementService.Repositories;
using MediatR;

namespace LabManagementService.Features.LabTests
{
    public static class CreateLabTest
    {
        public record Command(
            string TestName,
            string ReferenceRange,
            int SpecimenType
        ) : IRequest<ErrorOr<Guid>>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.TestName).NotEmpty().MaximumLength(50);
                RuleFor(x => x.ReferenceRange).MaximumLength(50);
                RuleFor(x => x.SpecimenType).GreaterThan(-1).LessThan(5);
            }
        }

        internal sealed class Handler : IRequestHandler<Command, ErrorOr<Guid>>
        {
            private readonly IRepository<LabTest> _context;
            private readonly IValidator<Command> _validator;

            public Handler(IRepository<LabTest> context, IValidator<Command> validator)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            }

            public async Task<ErrorOr<Guid>> Handle(Command command, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(command);

                if (!validationResult.IsValid)
                    return Error.Validation("Validation error", "Invalid input data");

                if (command.SpecimenType < 0 || command.SpecimenType > 4)
                    return Error.NotFound("Specimen error", "The specified specimen is out of range.");

                var labTest = new LabTest
                (
                    Guid.NewGuid(),
                    command.TestName,
                    (SpecimenType)command.SpecimenType,
                    command.ReferenceRange
                );

                await _context.CreateAsync(labTest);

                await _context.SaveChangesAsync();

                return labTest.LabTestId;
            }
        }
    }
}
