using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ReviewApi.Domain.Exceptions;

namespace ReviewApi.Controllers.Extensions
{
    public static class ControllerBaseExtensions
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static IActionResult HandleExceptionToUserAndLogIfExceptionIsUnexpected(this ControllerBase controller, Exception exception)
        {
            if (exception is AlreadyExistsException || exception is FluentValidation.ValidationException
            || exception is UserNotConfirmedException || exception is ResourceNotFoundException || exception is InvalidPasswordException)
            {
                return controller.BadRequest(new { Error = new { Message = exception.Message } });
            }
            else if (exception is ForbiddenException)
            {
                return controller.Forbid();
            }

            log.Error(JsonConvert.SerializeObject(exception));
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public static string GetUserIdFromToken(this ControllerBase controller)
        {
            string id = controller.HttpContext.User.Claims.First(i => i.Type == ClaimTypes.NameIdentifier).Value;
            return id;
        }

        public static void ValidateImageFileAndThrow(this ControllerBase controller, IFormFile image)
        {
            if (image.Length == 0)
            {
                throw new FluentValidation.ValidationException("image file not informed");
            }
            string fileExtension = Path.GetExtension(image.FileName).ToLower();
            if (fileExtension != ".jpg" && fileExtension != ".png" && fileExtension != ".jpeg")
            {
                throw new FluentValidation.ValidationException("invalid image file");
            }
        }
    }
}
