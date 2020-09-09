﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewApi.Application.Interfaces;
using ReviewApi.Controllers.Extensions;

namespace ReviewApi.Controllers
{
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService; 
        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpPut]
        [Authorize]
        [Route("reviews/{reviewId}/favorites")]
        public async Task<IActionResult> Update(string reviewId)
        {
            try
            {
                await _favoriteService.CreateOrDelete(this.GetUserIdFromToken(), reviewId);
                return Ok();
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("reviews/{reviewId}/favorites")]
        public async Task<IActionResult> GetAllFromReview(string reviewId, [FromQuery(Name = "page")] int page, [FromQuery(Name = "quantityPerPage")] int quantityPerPage) 
        {
            try
            {
                return Ok(await _favoriteService.GetAllFromReview(reviewId, page, quantityPerPage));
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("users/{userName}/favorites")]
        public async Task<IActionResult> GetAllFromUsers(string userName, [FromQuery(Name = "page")] int page, [FromQuery(Name = "quantityPerPage")] int quantityPerPage)
        {
            try
            {
                return Ok(await _favoriteService.GetAllFromUser(userName, page, quantityPerPage));
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }
    }
}
