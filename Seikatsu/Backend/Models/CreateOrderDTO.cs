namespace Seikatsu.Backend.Models
{
    public class CreateOrderDTO
    {
        public Guid CartId { get; set; }
        public Guid AddressId { get; set; }
    }
}
