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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(string reviewId)
        {
            try
            {
                return Ok(await _favoriteService.Create(this.GetUserIdFromToken(), reviewId));
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }
    }
}
