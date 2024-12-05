using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

public class CaptchaResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("challenge_ts")]
    public DateTime ChallengeTimestamp { get; set; }

    [JsonProperty("hostname")]
    public string Hostname { get; set; }
}
