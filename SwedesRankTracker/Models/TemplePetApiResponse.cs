using System.Text.Json.Serialization;

namespace SwedesRankTracker.Models
{
    public class TemplePetApiResponse
    {
        [JsonPropertyName("data")]
        public TemplePetData? Data { get; set; }
    }

    public class TemplePetData
    {
        [JsonPropertyName("items")]
        public TemplePetCount? Items { get; set; }
    }

    public class TemplePetCount
    {
        [JsonPropertyName("all_pets")]
        public List<TemplePet>? ListOfPets { get; set; }
    }

    public class TemplePet
    {
        [JsonPropertyName("id")]
        public int? PetId { get; set; }
        [JsonPropertyName("count")]
        public int? Count { get; set; }
        [JsonPropertyName("date")]
        public string? Date { get; set; }
    }
}
