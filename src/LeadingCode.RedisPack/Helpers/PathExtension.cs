using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadingCode.RedisPack.Helpers
{
    public static class PathExtension
    {
        public static string ToUnixPath(this string path)
        {
            path = path.Replace('\\', '/')
                .Replace(":", string.Empty);
            return $"/{path}";
        }

    }
}
