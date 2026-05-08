namespace Seikatsu.Backend.Models.AdminDTOs
{
    public class EditProductRequestDTO
    {
        public string?Name { get; set; } = string.Empty;
        public string? Description { get; set; }     

        public decimal? Price { get; set; }

        public bool? IsFood { get; set; }        
        public string? StorageType { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }       
        public string? CountryName { get; set; } = string.Empty; 
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }    

    }
}
