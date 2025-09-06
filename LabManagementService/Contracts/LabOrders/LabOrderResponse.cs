using LabManagementService.Enums;

namespace LabManagementService.Contracts.LabOrders
{
    public class LabOrderResponse
    {
        public Guid LabOrderId { get; set; }
        public int PatientId { get; set; }
        public string Complement { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public Status Status { get; set; }
        public string OrderedBy { get; set; } = string.Empty;
    }
}
