using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Models.Review;
using ReviewApi.Controllers.Extensions;
using ReviewApi.Models;

namespace ReviewApi.Controllers
{
    [Route("reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] CreateOrUpdateReviewModel model)
        {
            try
            {
                CreateOrUpdateReviewRequestModel serviceModel = new CreateOrUpdateReviewRequestModel()
                {
                    Image = model.Image.OpenReadStream(),
                    Stars = model.Stars,
                    Text = model.Text,
                    Title = model.Title
                };
                return Ok(await _reviewService.Create(this.GetUserIdFromToken(), serviceModel));
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery(Name = "page")] int page = 1)
        {
            try
            {
                return Ok(await _reviewService.GetAll(page));
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }
    }
}
