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
        [JsonPropertyName("1")]
        public TemplePetIndex? Index1 { get; set; }
    }

    public class TemplePetIndex
    {
        [JsonPropertyName("pet_count")]
        public double? PetCount { get; set; }
    }
}
