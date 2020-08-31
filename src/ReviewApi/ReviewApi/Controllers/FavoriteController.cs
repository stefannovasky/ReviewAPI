using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewApi.Application.Interfaces;
using ReviewApi.Controllers.Extensions;

namespace ReviewApi.Controllers
{
    [Route("reviews/{reviewId}/favorites")]
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
    }
}
