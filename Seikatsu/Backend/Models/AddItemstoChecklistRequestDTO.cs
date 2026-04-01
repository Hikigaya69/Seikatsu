namespace Seikatsu.Backend.Models
{
    public class AddItemstoChecklistRequestDTO
    {
        public List<CheckListItemDTO> Items { get; set; } = new();
    }

    
}
