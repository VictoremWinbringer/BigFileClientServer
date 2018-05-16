using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Server.Controllers
{
    [Route("api/v1/[controller]")]
    public class FilesController : Controller
    {
        // GET api/values
        [HttpGet("{id}")]
        public IActionResult Get(string id, long seek)
        {
            var path = GetFilePath(id);
            var fileInfo = new FileInfo(path);
            if (seek < 0 || seek > fileInfo.Length)
                throw new ArgumentOutOfRangeException(nameof(seek));
            if (seek == fileInfo.Length)
                return File(new byte[0], "application/octet-stream", id, new DateTimeOffset(fileInfo.LastWriteTimeUtc), null);
            var stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read
                );
            stream.Seek(seek, SeekOrigin.Begin);
            return File(stream, "application/octet-stream", id, new DateTimeOffset(fileInfo.LastWriteTimeUtc), null);
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, bool append = false)
        {
            var path = GetFilePath(id);
            if (!append && System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            using (var stream = append ? System.IO.File.Open(path, FileMode.Append) : System.IO.File.Create(path))
            {
                Copy(Request.Body, stream);
            }
            return Ok();
        }

        [HttpGet("{id}/[action]")]
        public long Size(string id)
        {
            var fileInfo = new FileInfo(GetFilePath(id));
            return fileInfo.Exists ? fileInfo.Length : 0;
        }

        private static string GetFilePath(string id)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos", id);
        }

        private static long Copy(Stream from, Stream to)
        {
            long copiedByteCount = 0;
            byte[] buffer = new byte[2 << 16];
            var len = 0;
            while (from.CanRead && (len = from.Read(buffer, 0, buffer.Length)) > 0)
            {
                to.Write(buffer, 0, len);
                to.Flush();
                copiedByteCount += len;
            }
            return copiedByteCount;
        }
    }
}
