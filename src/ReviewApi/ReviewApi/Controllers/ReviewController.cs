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
        public async Task<IActionResult> GetAll([FromQuery(Name = "page")] int page = 1, [FromQuery(Name = "quantity")] int quantity = 14)
        {
            try
            {
                return Ok(await _reviewService.GetAll(page, quantity));
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _reviewService.Delete(this.GetUserIdFromToken(), id);
                return NoContent();
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [HttpPut]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] CreateOrUpdateReviewModel model)
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
                await _reviewService.Update(this.GetUserIdFromToken(), id, serviceModel);
                return Ok();
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                return Ok(await _reviewService.GetById(this.GetUserIdFromToken(), id));
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }
    }
}
