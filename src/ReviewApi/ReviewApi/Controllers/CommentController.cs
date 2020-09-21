using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Models;
using ReviewApi.Application.Models.Comment;
using ReviewApi.Controllers.Extensions;
using ReviewApi.Domain.Dto;
using ReviewApi.Domain.Exceptions;

namespace ReviewApi.Controllers
{
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [Authorize]
        [HttpPost]
        [Route("reviews/{reviewId}/comments")]
        public async Task<IActionResult> Create(string reviewId, CreateCommentRequestModel model)
        {
            try
            {
                IdResponseModel responseModel = await _commentService.Create(reviewId, this.GetUserIdFromToken(), model);
                return CreatedAtRoute("", model, responseModel);
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("reviews/{reviewId}/comments")]
        public async Task<IActionResult> GetAllFromReview(string reviewId, [FromQuery(Name = "page")] int page, [FromQuery(Name = "quantityPerPage")] int quantityPerPage)
        {
            try
            {
                return Ok(await _commentService.GetAllFromReview(reviewId, new PaginationDTO(page, quantityPerPage)));
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("reviews/{reviewId}/comments/{commentId}")]
        public async Task<IActionResult> GetById(string commentId)
        {
            try
            {
                return Ok(await _commentService.GetById(commentId));
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [Authorize]
        [HttpPut]
        [Route("reviews/{reviewId}/comments/{commentId}")]
        public async Task<IActionResult> Update(string commentId, CreateCommentRequestModel model)
        {
            try
            {
                await _commentService.Update(commentId, this.GetUserIdFromToken(), model);
                return Ok();
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("reviews/{reviewId}/comments/{commentId}")]
        public async Task<IActionResult> Delete(string commentId)
        {
            try
            {
                await _commentService.Delete(commentId, this.GetUserIdFromToken());
                return NoContent();
            }
            catch (ResourceNotFoundException)
            {
                return NoContent();
            }
            catch (Exception exception)
            {
                return this.HandleExceptionToUserAndLogIfExceptionIsUnexpected(exception);
            }
        }
    }
}
