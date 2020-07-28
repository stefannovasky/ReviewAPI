using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReviewApi.Domain.Exceptions;

namespace ReviewApi.Controllers.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static IActionResult HandleException(this ControllerBase controller, Exception exception)
        {
            if (exception is AlreadyExistsException || exception is FluentValidation.ValidationException
            || exception is UserNotConfirmedException || exception is ResourceNotFoundException || exception is InvalidPasswordException)
            {
                return controller.BadRequest(new { Error = new { Message = exception.Message } });
            }

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public static string GetUserIdFromToken(this ControllerBase controller)
        {
            string id = controller.HttpContext.User.Claims.First(i => i.Type == ClaimTypes.NameIdentifier).Value;
            return id;
        }
    }
}
