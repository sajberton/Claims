using Newtonsoft.Json;

namespace Claims.Models
{
    public class ModelBase
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}