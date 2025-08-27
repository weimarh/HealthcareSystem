using LabManagementService.Enums;

namespace LabManagementService.Entities
{
    public class LabOrder
    {
        public LabOrder(Guid orderId, int patientId, LabTest labTest, Guid labtestId, DateTime orderDate, Status status, string orderedBy)
        {
            LabOrderId = orderId;
            PatientId = patientId;
            LabTest = labTest;
            LabTestId = labtestId;
            OrderDate = orderDate;
            Status = status;
            OrderedBy = orderedBy;
        }

        public LabOrder() { }

        public Guid LabOrderId { get; set; }
        public int PatientId { get; set; }
        public Guid LabTestId { get; set; }
        public LabTest? LabTest { get; set; }
        public DateTime OrderDate { get; set; }
        public Status Status { get; set; }
        public string OrderedBy { get; set; } = string.Empty;
        public Guid? LabResultId { get; set; }
        public LabResult? LabResult { get; set; }

        public static LabOrder UpdateLabOrder(LabOrder labOrder, LabTest labTest, Guid labtestId, DateTime orderDate, Status status, string orderedBy)
        {
            labOrder.OrderDate = orderDate;
            labOrder.LabTest = labTest;
            labOrder.LabTestId = labtestId;
            labOrder.Status = status;
            labOrder.OrderedBy = orderedBy;

            return labOrder;
        }

        public static LabOrder UpdateLabResults(LabOrder labOrder, LabResult labResult, Guid labResultId)
        {
            labOrder.LabResultId = labResultId;
            labOrder.LabResult = labResult;

            return labOrder;
        }
    }
}
