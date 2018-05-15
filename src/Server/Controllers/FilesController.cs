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
        public IActionResult Get(Guid id, long seek)
        {
            var stream = new FileStream(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos", "Video.mp4"),
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read
                );

            if (seek < 0 || seek > stream.Length)
                throw new ArgumentOutOfRangeException(nameof(seek));

            if (seek == stream.Length)
                return File(new byte[0], "application/octet-stream");

            stream.Seek(seek, SeekOrigin.Begin);

            return File(stream, "application/octet-stream");
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
