using System;
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
            || exception is UserNotConfirmedException || exception is ResourceNotFoundException)
            {
                return controller.BadRequest(new { Error = new { Message = exception.Message } });
            }

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
