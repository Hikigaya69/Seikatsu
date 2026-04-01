namespace Seikatsu.Backend.Models
{
    public class GetChecklistDTO
    {
        public Guid ChecklistId { get; set; }
       
        public List<CheckListItemResponseDTO> Items { get; set; } = new();
    }
}
