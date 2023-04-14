using System;
using System.Collections.Generic;
using System.Linq;
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
}
