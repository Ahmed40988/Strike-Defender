using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StrikeDefender.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public FilesController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("image/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var path = Path.Combine(_env.WebRootPath, "wwwroot", "Users", fileName);

            if (!System.IO.File.Exists(path))
                return NotFound();

            var contentType = "image/" + Path.GetExtension(fileName).TrimStart('.');

            var bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, contentType);
        }
    }
}
