using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/v1/[controller]")]
    public class FilesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get(long seek)
        {
            Console.WriteLine(seek);
            var stream = new FileStream(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos", "Video.mp4"),
                FileMode.Open, FileAccess.Read, FileShare.Read);
            if (seek < 0 || seek > stream.Length)
                throw new ArgumentOutOfRangeException(nameof(seek));
            stream.Seek(seek, SeekOrigin.Begin);
            Console.WriteLine(stream.Position);
            return File(stream, "application/octet-stream");
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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
