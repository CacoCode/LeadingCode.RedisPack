using System;
using Newtonsoft.Json;

namespace LeadingCode.RedisPack.Models
{
    public class RedisReleaseInfo
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string tarball_url { get; set; }

        public string zipball_url { get; set; }

        public DateTime published_at { get; set; }

        public string Body => $"https://github.com/redis/redis/releases/tag/{Name}";

        public string PublishedAt => published_at.ToShortDateString();
    }
}
