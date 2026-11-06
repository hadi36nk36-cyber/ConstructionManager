using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Application.DTOs
{
    public class EquipmentDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public EquipmentType Type { get; set; }
        public string SerialNumber { get; set; }
        public EquipmentStatus Status { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public string Description { get; set; }
    }
}