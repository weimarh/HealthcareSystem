using ErrorOr;
using FluentValidation;
using LabManagementService.Entities;
using LabManagementService.Repositories;
using MediatR;

namespace LabManagementService.Features.LabResults
{
    public static class DeleteLabResult
    {
        public record Command(Guid Id) : IRequest<ErrorOr<Unit>>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }

        internal sealed class Handler : IRequestHandler<Command, ErrorOr<Unit>>
        {
            private readonly IValidator<Command> _validator;
            private readonly IRepository<LabResult> _context;
            public Handler(IValidator<Command> validator, IRepository<LabResult> context)
            {
                _validator = validator ?? throw new ArgumentNullException(nameof(validator));
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<ErrorOr<Unit>> Handle(Command command, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(command);

                if (validationResult.IsValid)
                    return Error.NotFound("Invalid data", "Invalid input data");

                var labResult = await _context.GetAsync(command.Id);

                if (labResult == null)
                    return Error.NotFound("Lab result not found", "The lab result with the given Id was not found");

                _context.DeleteAsync(labResult);

                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
