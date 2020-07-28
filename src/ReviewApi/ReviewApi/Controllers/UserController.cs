using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
                return this.HandleException(exception);
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
                return this.HandleException(exception);
            }
        }

        [HttpPost]
        [Route("confirmation")]
        public async Task<IActionResult> Confirmation(ConfirmUserRequestModel model)
        {
            try
            {
                await _userService.ConfirmUser(model);
                return NoContent();
            }
            catch (Exception exception)
            {
                return this.HandleException(exception);
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
                return this.HandleException(exception);
            }
        }
    }
}
