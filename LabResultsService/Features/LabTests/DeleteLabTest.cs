using Carter;
using ErrorOr;
using FluentValidation;
using LabResultsService.Contracts.LabTests;
using LabResultsService.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabResultsService.Features.LabTests
{
    public static class DeleteLabTest
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
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<ErrorOr<Unit>> Handle(Command command, CancellationToken cancellationToken)
            {
                var labTest = await _context.LabTests.FirstOrDefaultAsync(x => x.LabTestId == command.Id);

                if (labTest == null)
                    return Error.NotFound("Lab test not found", "The lab test with the given Id was not found");

                _context.LabTests.Remove(labTest);

                await _context.SaveChangesAsync();
                
                return Unit.Value;
            }
        }
    }

    public class DeleteLabTestEndpoint() : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/labtests/{id}",
                async (string id, [FromBody]DeleteLabTestRequest request, [FromServices]ISender sender) =>
                {
                    if (id != request.LabTestId.ToString())
                        return Results.BadRequest("Identification number in the URL does not match the request body.");

                    var command = new DeleteLabTest.Command(request.LabTestId);

                    var result = await sender.Send(command);

                    return result.Match(
                        id => Results.Ok(id),
                        errors => Results.NotFound(errors));
                });
        }
    }
}
