using System;
using Microsoft.AspNetCore.Mvc;
using ReviewApi.Application.Interfaces;
using ReviewApi.Controllers.Extensions;

namespace ReviewApi.Controllers
{
    [Route("images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpGet]
        [Route("{name}")]
        public IActionResult GetByName(string name)
        {
            try
            {
                return File(_imageService.GetImageByName(name), "image/jpeg");
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }
    }
}
