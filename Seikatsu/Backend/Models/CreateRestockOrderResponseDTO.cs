namespace Seikatsu.Backend.Models
{
    public class CreateRestockOrderResponseDTO
    {
        public  Guid OrderId { get; set; }
    
        public decimal Amount { get; set; }

        public Guid PayemntID { get; set; } 


    }
}
