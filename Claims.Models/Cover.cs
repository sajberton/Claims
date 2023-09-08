using Claims.Models;
using Claims.Models.Enums;
using Newtonsoft.Json;

namespace Claims;

public class Cover : ModelBase
{
    [JsonProperty(PropertyName = "startDate")]
    public DateOnly StartDate { get; set; }
    
    [JsonProperty(PropertyName = "endDate")]
    public DateOnly EndDate { get; set; }
    
    [JsonProperty(PropertyName = "claimType")]
    public CoverTypeEnum Type { get; set; }

    [JsonProperty(PropertyName = "premium")]
    public decimal Premium { get; set; }
}

