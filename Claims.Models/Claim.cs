using Claims.Models;
using Claims.Models.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Claims.Models
{
    public class Claim : ModelBase
    {
        [JsonProperty(PropertyName = "coverId")]
        public string CoverId { get; set; }

        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "claimType")]
        public ClaimTypeEnum Type { get; set; }

        [JsonProperty(PropertyName = "damageCost")]
        public decimal DamageCost { get; set; }
    }
}
