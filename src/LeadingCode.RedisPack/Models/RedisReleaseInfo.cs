using System;
using Newtonsoft.Json;

namespace LeadingCode.RedisPack.Models
{
    public class RedisReleaseInfo
    {
        public long Id { get; set; }

        public string Name { get; set; }

        [JsonProperty("tarball_url")]
        public string TarBallUrl { get; set; }

        [JsonProperty("zipball_url")]
        public string ZipBallUrl { get; set; }

        [JsonProperty("published_at")]
        public DateTime PublishedAt { get; set; }

        public string Body { get; set; }
    }
}
