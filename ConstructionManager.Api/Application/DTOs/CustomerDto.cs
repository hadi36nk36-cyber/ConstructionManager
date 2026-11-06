using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Application.DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public CustomerType CustomerType { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Address { get; set; }
    }
}