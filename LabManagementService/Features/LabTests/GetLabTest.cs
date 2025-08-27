using ErrorOr;
using LabManagementService.Contracts.LabTests;
using LabManagementService.Entities;
using LabManagementService.Repositories;
using MediatR;

namespace LabManagementService.Features.LabTests
{
    public static class GetLabTest
    {
        public record Query(Guid Id) : IRequest<ErrorOr<LabTestResponse>>;

        internal sealed class Handler : IRequestHandler<Query, ErrorOr<LabTestResponse>>
        {
            private readonly IRepository<LabTest> _context;

            public Handler(IRepository<LabTest> context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<ErrorOr<LabTestResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var labTest = await _context.GetAsync(request.Id);

                if (labTest == null)
                    return Error.NotFound("Lab test not found", "The lab test with the given Id number was not found.");

                var labTestResponse = new LabTestResponse()
                {
                    LabTestId = labTest.LabTestId,
                    TestName = labTest.TestName,
                    ReferenceRange = labTest.ReferenceRange,
                    SpecimenType = labTest.SpecimenType,
                };

                return labTestResponse;
            }
        }
    }
}
