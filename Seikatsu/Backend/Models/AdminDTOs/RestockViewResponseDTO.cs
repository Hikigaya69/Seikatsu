using Seikatsu.Backend.Enums;
using System.Text.Json.Serialization;

namespace Seikatsu.Backend.Models.AdminDTOs
{
    public class RestockViewResponseDTO
    {
        public string productName {get; set; } 

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RestockFrequency Frequency { get; set; }

        public DateTime? NextOrderDate { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RestockItemsStatus? Status { get; set; } 


    }
}
