using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Models.User;
using ReviewApi.Controllers.Extensions;

namespace ReviewApi.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRequestModel model)
        {
            try
            {
                await _userService.Create(model);
                return CreatedAtRoute("", null);
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [HttpPost]
        [Route("auth")]
        public async Task<IActionResult> Authenticate(AuthenticationUserRequestModel model)
        {
            try
            {
                return Ok(await _userService.Authenticate(model));
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [HttpPost]
        [Route("confirmation")]
        public async Task<IActionResult> Confirmation(ConfirmUserRequestModel model)
        {
            try
            {
                await _userService.ConfirmUser(model);
                return Ok();
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [HttpPut]
        [Authorize]
        [Route("name")]
        public async Task<IActionResult> UpdateName(UpdateNameUserRequestModel model)
        {
            try
            {
                await _userService.UpdateUserName(this.GetUserIdFromToken(), model);
                return Ok();
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [HttpPut]
        [Authorize]
        [Route("password")]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordUserRequestModel model)
        {
            try
            {
                await _userService.UpdatePassword(this.GetUserIdFromToken(), model);
                return Ok();
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("delete")]
        public async Task<IActionResult> Delete(DeleteUserRequestModel model)
        {
            try
            {
                await _userService.Delete(this.GetUserIdFromToken(), model);
                return NoContent();
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordUserRequestModel model)
        {
            try
            {
                await _userService.ForgotPassword(model);
                return Ok();
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordUserRequestModel model)
        {
            try
            {
                await _userService.ResetPassword(model);
                return Ok();
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("profile")]
        public async Task<IActionResult> GetAuthenticatedUserProfile()
        {
            try
            {
                return Ok(await _userService.GetAuthenticatedUserProfile(this.GetUserIdFromToken()));
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [HttpPut]
        [Authorize]
        [Route("image")]
        public async Task<IActionResult> UpdateProfileImage(IFormFile image)
        {
            try
            {
                this.ValidateImageFileAndThrow(image);
                await _userService.UpdateProfileImage(this.GetUserIdFromToken(), image.OpenReadStream());
                return Ok();
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }
    }
}
