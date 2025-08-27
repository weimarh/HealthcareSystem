using ErrorOr;
using LabManagementService.Contracts.LabTests;
using LabManagementService.Entities;
using LabManagementService.Repositories;
using MediatR;

namespace LabManagementService.Features.LabTests
{
    public class GetAllLabTests
    {
        public record Query() : IRequest<ErrorOr<IReadOnlyList<LabTestResponse>>>;

        internal sealed class Handler : IRequestHandler<Query, ErrorOr<IReadOnlyList<LabTestResponse>>>
        {
            private readonly IRepository<LabTest> _context;

            public Handler(IRepository<LabTest> context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<ErrorOr<IReadOnlyList<LabTestResponse>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var labTests = await _context.GetAllAsync();

                var labTestsResponse = labTests
                    .Select(x => new LabTestResponse
                    {
                        LabTestId = x.LabTestId,
                        TestName = x.TestName,
                        ReferenceRange = x.ReferenceRange,
                        SpecimenType = x.SpecimenType,
                    }).ToList();

                return labTestsResponse;
            }
        }
    }
}
