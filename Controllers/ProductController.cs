using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public ProductController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPut("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile formFile, string productcode)
        {
            try
            {
                string filePath = GetFilePath(productcode);
                if(!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }

                string imagePath = filePath + "\\" + productcode + ".png";
                if(System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                using(FileStream stream = System.IO.File.Create(imagePath))
                {
                    await formFile.CopyToAsync(stream);
                }
                return NoContent();

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("MultiUploadImage")]
        public async Task<IActionResult> MultiUploadImage(IFormFileCollection fileCollection, string productcode)
        {
            int passcount = 0;
            int errorCount = 0;
            try
            {
                string filePath = GetFilePath(productcode);
                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }

                foreach(var file in fileCollection)
                {
                    string imagePath = filePath + "\\" + file.FileName;
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    using (FileStream stream = System.IO.File.Create(imagePath))
                    {
                        await file.CopyToAsync(stream);
                        passcount++;
                    }
                }
                
                return NoContent();

            }
            catch (Exception ex)
            {
                errorCount++;
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetImage")]
        public async Task<IActionResult> GetImage(string productcode)
        {
            string imageUrl = string.Empty;
            string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            try
            {
                string filePath = GetFilePath(productcode);
                string imagePath = filePath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagePath))
                {
                    imageUrl = hosturl + "/Upload/product/" + productcode + "/" + productcode + ".png";

                }
                else
                {
                    return NotFound();
                }
            }catch(Exception ex)
            {

            }
            return Ok(imageUrl);
        }


        [HttpGet("GetImageCollection")]
        public async Task<IActionResult> GetMultiImage(string productcode)
        {
            List<string> imageUrl = new List<string>();
            string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            try
            {
                string filePath = GetFilePath(productcode);
                if (System.IO.Directory.Exists(filePath))
                {
                    DirectoryInfo directoryInfo =new DirectoryInfo(filePath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        string fileName = fileInfo.Name;
                        string imagePath = filePath + "\\" +fileName;
                        if (System.IO.File.Exists(imagePath))
                        {
                            string _imageUrl = hosturl + "/Upload/product/" + productcode + "/" + fileName;
                            imageUrl.Add(_imageUrl);
                        }

                    }
                }
                
            }
            catch (Exception ex)
            {

            }
            return Ok(imageUrl);
        }


        [HttpGet("downloadImage")]
        public async Task<IActionResult> DownloadImage(string productcode)
        {

            try
            {
                string filePath = GetFilePath(productcode);
                string imagePath = filePath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagePath))
                {
                    MemoryStream stream = new MemoryStream();
                    using(FileStream fileStream = new FileStream(imagePath, FileMode.Open))
                    {
                        await fileStream.CopyToAsync(stream);
                    }
                    stream.Position = 0;
                    return File(stream, "image/png", productcode + ".png");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
            
        }


        [HttpDelete("deleteImage")]
        public async Task<IActionResult> DeleteImage(string productcode)
        {

            try
            {
                string filePath = GetFilePath(productcode);
                string imagePath = filePath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

        }

        [HttpDelete("multiDeleteImage")]
        public async Task<IActionResult> MultiDeleteImage(string productcode)
        {

            try
            {
                string filePath = GetFilePath(productcode);
                if (System.IO.Directory.Exists(filePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        fileInfo.Delete();

                    }
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

        }


        [NonAction]
        private string GetFilePath(string productcode)
        {
            return _environment.WebRootPath + "\\Upload\\product\\" + productcode;
        }

    }
}
