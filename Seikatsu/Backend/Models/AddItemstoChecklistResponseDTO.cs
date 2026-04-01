namespace Seikatsu.Backend.Models
{
    public class AddItemstoChecklistResponseDTO
    {
        public Guid ChecklistId { get; set; }
        public List<CheckListItemResponseDTO> AddedItems { get; set; } = new();
    }
}