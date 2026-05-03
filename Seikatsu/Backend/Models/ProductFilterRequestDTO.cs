namespace Seikatsu.Backend.Models
{
    public class ProductFilterRequestDTO
    {
        public string? CountryName { get; set; }
        public Guid? CategoryId { get; set; }

        public decimal? MinPrice { get; set; }  
        public decimal? MaxPrice { get; set; }

        public string? StorageType { get; set; }
    }
}
