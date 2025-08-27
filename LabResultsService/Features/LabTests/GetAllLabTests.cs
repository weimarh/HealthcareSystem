using Carter;
using ErrorOr;
using LabResultsService.Contracts.LabTests;
using LabResultsService.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabResultsService.Features.LabTests
{
    public static class GetAllLabTests
    {
        public record Query() : IRequest<ErrorOr<IReadOnlyList<LabTestResponse>>>;

        internal sealed class Handler : IRequestHandler<Query, ErrorOr<IReadOnlyList<LabTestResponse>>>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<ErrorOr<IReadOnlyList<LabTestResponse>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var labTests = await _context.LabTests
                    .Select(x => new LabTestResponse
                    {
                        LabTestId = x.LabTestId,
                        TestName = x.TestName,
                        ReferenceRange = x.ReferenceRange,
                        SpecimenType = x.SpecimenType,
                    }).ToListAsync(cancellationToken);

                return labTests;
            }
        }
    }

    public class GetAllLabTestsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/labtests", async ([FromServices] ISender sender) =>
            {
                var query = new GetAllLabTests.Query();
                var result = await sender.Send(query);

                return result.Match(
                    labTestId => Results.Ok(labTestId),
                    errors => Results.Problem(string.Join("; ", errors.Select(e => e.Description))));
            });
        }
    }
}
