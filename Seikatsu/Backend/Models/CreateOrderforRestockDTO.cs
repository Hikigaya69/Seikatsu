namespace Seikatsu.Backend.Models
{
    public class CreateOrderforRestockDTO
    {
        public Guid RestockCartId { get; set; }
        public Guid AddressId { get; set; }

    }
}
