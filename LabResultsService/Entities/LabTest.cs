using LabResultsService.Enums;

namespace LabResultsService.Entities
{
    public class LabTest
    {
        public LabTest() { }

        public LabTest(Guid labTestId, string testName, SpecimenType specimenType, string referenceRange)
        {
            LabTestId = labTestId;
            TestName = testName;
            ReferenceRange = referenceRange;
            SpecimenType = specimenType;
        }

        public Guid LabTestId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string ReferenceRange { get; set; } = string.Empty;
        public SpecimenType SpecimenType { get; set; }
        public ICollection<LabOrder>? LabOrders { get; set; }

        public static LabTest UpdateLabTest(LabTest labTest, string testName, SpecimenType specimenType, string referenceRange)
        {
            labTest.TestName = testName;
            labTest.ReferenceRange = referenceRange;
            labTest.SpecimenType = specimenType;

            return labTest;
        }
    }
}
