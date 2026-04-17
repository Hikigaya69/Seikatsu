using Seikatsu.Backend.Enums;

namespace Seikatsu.Backend.Models
{
    public class UpdateRestockCartStatusResponseDTO
    {
       
       public RestockItemsStatus EffectiveStatus { get; set; } // what actually runs (user choose a status) 

    }
}
