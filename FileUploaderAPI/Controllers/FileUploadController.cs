using FileUploaderAPI.DataAccess;
using FileUploaderAPI.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileUploaderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private FileUploaderDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FileUploadController(FileUploaderDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        }

        // GET: api/FileUpload/GenerateUploadURL
        [HttpGet]
        public IActionResult GenerateUploadURL()
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            _context.UploadURLTokens
                .Add(new UploadURLToken()
                {
                    Token = token,
                    IsExpired = false
                });
            _context.SaveChanges();

            var url = Url.Content("~/") + "api/FileUpload/" + token;

            return Ok(url);
        }

        // POST api/<FileUploadController>
        [HttpPost("{token}")]
        public IActionResult Post([FromRoute] string token, [FromBody] UploadedFileMetadata metadata)
        {
            var uploadURLToken = _context.UploadURLTokens.FirstOrDefault(t => t.Token == token);
            uploadURLToken.IsExpired = true;

            Guid guid = Guid.NewGuid();
            metadata.Id = guid;
            _context.UploadedFileMetadata.Add(metadata);

            _context.SaveChanges();

            return Ok(guid.ToString());
        }

        // PUT api/<FileUploadController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string guid, IFormFile file)
        {
            if (file.Length > 0)
            {
                // Create a folder "Uploads" in the hosting content root path.
                string path = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Create a path by combining the above folder with GUID and file name
                var filePath = Path.Combine(path, guid, file.FileName);

                // Write file to the path
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyToAsync(stream);
                }
            }

            return Ok();
        }

    }
}
