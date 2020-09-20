using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReviewApi.Application.Converter;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Models;
using ReviewApi.Application.Models.Comment;
using ReviewApi.Application.Validators.Comment;
using ReviewApi.Application.Validators.Extensions;
using ReviewApi.Domain.Entities;
using ReviewApi.Domain.Exceptions;
using ReviewApi.Domain.Interfaces.Repositories;
using ReviewApi.Infra.Redis.Interfaces;
using ReviewApi.Shared.Interfaces;

namespace ReviewApi.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly ICacheDatabase _cacheDatabase;
        private readonly IJsonUtils _jsonUtils;
        private readonly IConverter _converter;
        private readonly string _webApplicationUrl;

        public CommentService(ICommentRepository commentRepository, IReviewRepository reviewRepository, ICacheDatabase cacheDatabase, IJsonUtils jsonUtils, IConverter converter, string webApplicationUrl)
        {
            _commentRepository = commentRepository;
            _reviewRepository = reviewRepository;
            _cacheDatabase = cacheDatabase;
            _jsonUtils = jsonUtils;
            _converter = converter;
            _webApplicationUrl = webApplicationUrl;
        }

        public async Task<IdResponseModel> Create(string reviewId, string userId, CreateCommentRequestModel model)
        {
            await new CreateOrUpdateCommentValidator().ValidateRequestModelAndThrow(model);

            Guid reviewIdGuid = Guid.Parse(reviewId); 
            await ThrowIfReviewNotExists(reviewIdGuid);

            Comment comment = new Comment(model.Text, Guid.Parse(userId), reviewIdGuid);
            await _commentRepository.Create(comment);
            await _commentRepository.Save();

            return new IdResponseModel() { Id = comment.Id };
        }

        public async Task Delete(string commentId, string userId)
        {
            Comment registeredComment; 

            string commentRegisteredOnCacheJson = await _cacheDatabase.Get(commentId);
            if (commentRegisteredOnCacheJson != null)
            {
                registeredComment = _jsonUtils.Deserialize<Comment>(commentRegisteredOnCacheJson);
            }
            else 
            {
                registeredComment = await _commentRepository.GetById(Guid.Parse(commentId));
                if (registeredComment == null)
                {
                    throw new ResourceNotFoundException("comment not found."); 
                }
            }

            ThrowIfAuthenticatedUserNotIsCommentCreator(registeredComment, Guid.Parse(userId));

            _commentRepository.Delete(registeredComment);
            await _commentRepository.Save();
            await _cacheDatabase.Remove(commentId);
        }

        public async Task<PaginationResponseModel<CommentResponseModel>> GetAllFromReview(string reviewId, int page = 1, int quantityPerPage = 14)
        {
            if (page < 1)
            {
                page = 1;
            }
            if (quantityPerPage < 1)
            {
                quantityPerPage = 1;
            }
            Guid reviewIdGuid = Guid.Parse(reviewId);

            await ThrowIfReviewNotExists(reviewIdGuid);

            int totalInsertedsCommentsInReview = await _commentRepository.CountFromReview(reviewIdGuid);

            IEnumerable<Comment> comments;
            string commentsRegisteredOnCacheJson = await _cacheDatabase.Get($"comments?reviewId={reviewId}&page={page}&quantityPerPage={quantityPerPage}");
            if (commentsRegisteredOnCacheJson != null)
            {
                comments = _jsonUtils.Deserialize<IEnumerable<Comment>>(commentsRegisteredOnCacheJson);
                return CreatePaginationResult(comments, totalInsertedsCommentsInReview, reviewId, page, quantityPerPage);
            }
            comments = await _commentRepository.GetAllFromReview(reviewIdGuid, page, quantityPerPage);
            if (comments.Count() > 0)
            {
                await _cacheDatabase.Set($"comments?reviewId={reviewId}&page={page}&quantityPerPage={quantityPerPage}", _jsonUtils.Serialize(comments));
            }
            return CreatePaginationResult(comments, totalInsertedsCommentsInReview, reviewId, page, quantityPerPage);
        }

        public async Task<CommentResponseModel> GetById(string commentId)
        {
            Comment registeredComment;
            string commentRegisteredOnCacheJson = await _cacheDatabase.Get(commentId);
            if (commentRegisteredOnCacheJson != null)
            {
                registeredComment = _jsonUtils.Deserialize<Comment>(commentRegisteredOnCacheJson);
            }
            else
            {
                registeredComment = await _commentRepository.GetByIdIncludingUser(Guid.Parse(commentId));
                if (registeredComment == null)
                {
                    throw new ResourceNotFoundException("comment not found.");
                }
                await _cacheDatabase.Set(commentId, _jsonUtils.Serialize(registeredComment));
            }

            return _converter.ConvertCommentToCommentResponseModel(registeredComment);
        }

        public async Task Update(string commentId, string userId, CreateCommentRequestModel model)
        {
            await new CreateOrUpdateCommentValidator().ValidateRequestModelAndThrow(model);

            Comment registeredComment;
            string commentRegisteredOnCacheJson = await _cacheDatabase.Get(commentId);
            if (commentRegisteredOnCacheJson != null)
            {
                registeredComment = _jsonUtils.Deserialize<Comment>(commentRegisteredOnCacheJson);
            }
            else 
            {
                registeredComment = await _commentRepository.GetByIdIncludingUser(Guid.Parse(commentId));
                if (registeredComment == null)
                {
                    throw new ResourceNotFoundException("comment not found.");
                }
            }

            ThrowIfAuthenticatedUserNotIsCommentCreator(registeredComment, Guid.Parse(userId));
            registeredComment.UpdateText(model.Text);
            _commentRepository.Update(registeredComment);
            await _commentRepository.Save();
            await _cacheDatabase.Set(commentId, _jsonUtils.Serialize(registeredComment));
        }

        private void ThrowIfAuthenticatedUserNotIsCommentCreator(Comment comment, Guid userId)
        {
            if (comment.UserId != userId)
            {
                throw new ForbiddenException();
            }
        }

        private async Task ThrowIfReviewNotExists(Guid reviewId)
        {
            if (await _cacheDatabase.Get(reviewId.ToString()) == null)
            {
                if (!await _reviewRepository.AlreadyExists(reviewId))
                {
                    throw new ResourceNotFoundException("review not found.");
                }
            }
        }

        private PaginationResponseModel<CommentResponseModel> CreatePaginationResult(IEnumerable<Comment> comments, int totalCommentsInserteds, string reviewId, int actualPage, int quantityPerPage)
        {
            int previousPage = actualPage - 1;
            string previousPageUrl = previousPage > 0 ? $"{_webApplicationUrl}/reviews/{reviewId}/comments?page={previousPage}&quantity={quantityPerPage}" : null;
            return new PaginationResponseModel<CommentResponseModel>()
            {
                Data = comments.Select(comment => _converter.ConvertCommentToCommentResponseModel(comment)),
                NextPage = $"{_webApplicationUrl}/reviews/{reviewId}/comments?page={actualPage + 1}&quantity={quantityPerPage}",
                PreviousPage = previousPageUrl,
                Total = totalCommentsInserteds
            };
        }
    }
}
