﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Models.User;

namespace ReviewApi.Controllers
{
    [Route("api/users")]
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
            await _userService.Create(model);
            return CreatedAtRoute("", null);
        }
    }
}