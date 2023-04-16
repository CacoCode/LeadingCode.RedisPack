using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LeadingCode.RedisPack.Models;
using WebApiClientCore.Attributes;

namespace LeadingCode.RedisPack.Apis
{
    [HttpHost("https://api.github.com/")]
    public interface IGithubRedisApi
    {
        [HttpGet("repos/redis/redis/releases")]
        Task<List<RedisReleaseInfo>> GetAsync();
    }

    [HttpHost("https://github.com/")]
    public interface IGithubRedisFileApi
    {
        [HttpGet("redis/redis/archive/refs/tags/{fileName}")]
        Task<HttpResponseMessage> DownloadAsync(string fileName);
    }
}
