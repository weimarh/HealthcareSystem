using Carter;
using ErrorOr;
using LabResultsService.Contracts.LabOrders;
using LabResultsService.Contracts.LabTests;
using LabResultsService.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabResultsService.Features.LabTests
{
    public static class GetLabTest
    {
        public record Query(Guid Id) : IRequest<ErrorOr<LabTestResponse>>;

        internal sealed class Handler : IRequestHandler<Query, ErrorOr<LabTestResponse>>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<ErrorOr<LabTestResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var labTest = await _context.LabTests
                    .Where(x => x.LabTestId == request.Id)
                    .Select(x => new LabTestResponse
                    {
                        LabTestId = request.Id,
                        TestName = x.TestName,
                        ReferenceRange = x.ReferenceRange,
                        SpecimenType = x.SpecimenType,
                    }).FirstOrDefaultAsync(cancellationToken);

                if (labTest == null)
                    return Error.NotFound("Lab test not found", "The lab test with the given Id number was not found.");

                return labTest;
            }
        }
    }

    public class GatLabTestEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/labtests/{id}", async (string id, [FromServices] ISender sender) =>
            {
                var query = new GetLabTest.Query(Guid.Parse(id));
                var result = await sender.Send(query);

                return result.Match(
                    labTest => Results.Ok(labTest),
                    errors => Results.BadRequest(errors));
            });
        }
    }
}
