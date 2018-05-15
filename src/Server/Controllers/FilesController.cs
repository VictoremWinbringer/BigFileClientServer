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
        public IActionResult Get(Guid id, long seek)
        {
            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "videos",
                "Video.mp4"
                );

            var fileInfo = new FileInfo(path);

            if (seek < 0 || seek > fileInfo.Length)
                throw new ArgumentOutOfRangeException(nameof(seek));

            if (seek == fileInfo.Length)
                return File(new byte[0], "application/octet-stream", "Video.mp4", new DateTimeOffset(fileInfo.LastWriteTimeUtc), null);

            var stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read
                );

            stream.Seek(seek, SeekOrigin.Begin);

            return File(stream, "application/octet-stream", "Video.mp4", new DateTimeOffset(fileInfo.LastWriteTimeUtc), null);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
