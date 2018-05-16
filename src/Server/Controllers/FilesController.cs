using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/v1/[controller]")]
    public class FilesController : Controller
    {
        // GET api/values
        [HttpGet("{id}")]
        public void Get(string id, long seek)
        {
            var path = GetFilePath(id);
            var fileInfo = new FileInfo(path);
            if (seek < 0 || seek > fileInfo.Length)
                throw new ArgumentOutOfRangeException(nameof(seek));
            if (seek == fileInfo.Length)
                return;
            HttpContext.Response.ContentType = "application/octet-stream";
            HttpContext.Response.Headers.Add("Content-Disposition", "attachment; filename=" + id);
            HttpContext.Response.Headers.Add("Content-Length", (fileInfo.Length - seek).ToString());
            using (var fileStream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read
            ))
            {
                fileStream.Seek(seek, SeekOrigin.Begin);
                Copy(fileStream, HttpContext.Response.Body);
                fileStream.Close();
            }
        }

        [HttpPut("{id}")]
        public void Put(string id, bool append = false)
        {
            var path = GetFilePath(id);
            if (!append && System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            using (var fileStream = append ? System.IO.File.Open(path, FileMode.Append) : System.IO.File.Create(path))
            {
                Copy(Request.Body, fileStream);
                fileStream.Close();
            }
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
            while (from.CanRead && to.CanWrite && (len = from.Read(buffer, 0, buffer.Length)) > 0)
            {
                to.Write(buffer, 0, len);
                to.Flush();
                copiedByteCount += len;
            }
            return copiedByteCount;
        }
    }
}
