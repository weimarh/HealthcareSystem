using Carter;
using ErrorOr;
using FluentValidation;
using LabResultsService.Contracts.LabResults;
using LabResultsService.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabResultsService.Features.LabResults
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
            private readonly ApplicationDbContext _context;
            public Handler(IValidator<Command> validator, ApplicationDbContext context)
            {
                _validator = validator ?? throw new ArgumentNullException(nameof(validator));
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<ErrorOr<Unit>> Handle(Command command, CancellationToken cancellationToken)
            {
                var validationResult =_validator.Validate(command);

                if (validationResult.IsValid)
                    return Error.NotFound("Invalid data", "Invalid input data");

                var labResult = await _context.LabResults.FirstOrDefaultAsync(x => x.LabOrderId == command.Id);

                if (labResult == null)
                    return Error.NotFound("Lab result not found", "The lab result with the given Id was not found");

                _context.LabResults.Remove(labResult);

                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }

    public class DeleteLabResultEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/labresults/{id}", async (string id, [FromBody]DeleteLabResultRequest request, [FromServices] ISender sender) =>
            {
                if (id != request.Id.ToString())
                    return Results.BadRequest("Identification number in the URL does not match the request body.");

                var command = new DeleteLabResult.Command(request.Id);

                var result = await sender.Send(command);

                return result.Match(
                    id => Results.Ok(id),
                    errors => Results.NotFound(errors));
            });
        }
    }
}
