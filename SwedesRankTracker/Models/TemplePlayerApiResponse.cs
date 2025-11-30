using System.Text.Json.Serialization;

namespace SwedesRankTracker.Models
{
    public class TemplePlayerApiResponse
    {
        [JsonPropertyName("data")]
        public TemplePlayerData? Data { get; set; }
    }

    public class TemplePlayerData
    {
        [JsonPropertyName("info")]
        public TemplePlayerInfo? Info { get; set; }

        [JsonPropertyName("Ehb")]
        public double? Ehb { get; set; }

        [JsonPropertyName("Ehp")]
        public double? Ehp { get; set; }
        [JsonPropertyName("Collections")]
        public double? Collections { get; set; }
    }

    public class TemplePlayerInfo
    {
        [JsonPropertyName("Username")]
        public string? Username { get; set; }
        [JsonPropertyName("Game mode")]
        public int? GameMode { get; set; }
        [JsonPropertyName("Primary_ehp")]
        public string EhbType { get; set; }
    }
}