namespace Seikatsu.Backend.Models
{
    public class ModifyItemsResponseDTO
    {
        public Guid ChecklistId { get; set; }
        public List<CheckListItemResponseDTO> ModifiedItems { get; set; } = new();
    }
}
