using ErrorOr;
using FluentValidation;
using LabManagementService.Entities;
using LabManagementService.Repositories;
using MediatR;

namespace LabManagementService.Features.LabOrders
{
    public static class DeleteLabOrder
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
            private readonly IRepository<LabOrder> _context;

            public Handler(IRepository<LabOrder> context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<ErrorOr<Unit>> Handle(Command command, CancellationToken cancellationToken)
            {
                var labOrder = await _context.GetAsync(command.Id);

                if (labOrder == null)
                    return Error.NotFound("Lab order not found", "The lab order with the given Id was not found");

                _context.DeleteAsync(labOrder);

                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
