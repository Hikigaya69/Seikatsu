namespace Seikatsu.Backend.Models
{
    public class ModifyItemsinChecklistRequestDTO
    {
        public List<CheckListItemResponseDTO> Items { get; set; } = new();
    }
}
