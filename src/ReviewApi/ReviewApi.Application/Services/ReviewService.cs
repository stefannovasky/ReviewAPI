using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ReviewApi.Application.Converter;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Models;
using ReviewApi.Application.Models.Review;
using ReviewApi.Application.Validators.Extensions;
using ReviewApi.Application.Validators.Review;
using ReviewApi.Domain.Dto;
using ReviewApi.Domain.Entities;
using ReviewApi.Domain.Exceptions;
using ReviewApi.Domain.Interfaces.Repositories;
using ReviewApi.Infra.Redis.Interfaces;
using ReviewApi.Shared.Interfaces;

namespace ReviewApi.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IFileUploadUtils _fileUploadUtils;
        private readonly ICacheDatabase _cacheDatabase;
        private readonly IJsonUtils _jsonUtils;
        private readonly IConverter _converter;
        private readonly string _webApplicationUrl;

        public ReviewService(IReviewRepository reviewRepository, IFileUploadUtils fileUploadUtils, ICacheDatabase cacheDatabase, IJsonUtils jsonUtils, IConverter converter, string webApplicationUrl)
        {
            _reviewRepository = reviewRepository;
            _fileUploadUtils = fileUploadUtils;
            _cacheDatabase = cacheDatabase;
            _jsonUtils = jsonUtils;
            _converter = converter;
            _webApplicationUrl = webApplicationUrl;
        }

        public async Task<IdResponseModel> Create(string userId, CreateOrUpdateReviewRequestModel model)
        {
            await new CreateOrUpdateReviewValidator().ValidateRequestModelAndThrow(model);

            Review review = new Review(model.Title, model.Text, model.Stars, Guid.Parse(userId));
            review.AddImage(await UploadImage(model.Image));

            await _reviewRepository.Create(review);
            await _reviewRepository.Save();

            return new IdResponseModel() { Id = review.Id };
        }

        public async Task Delete(string userId, string reviewId)
        {
            Review review = await _reviewRepository.GetById(Guid.Parse(reviewId));
            if (review == null)
            {
                throw new ResourceNotFoundException("review not found.");
            }
            ThrowIfAuthenticatedUserNotBeReviewCreator(review, Guid.Parse(userId));
            _reviewRepository.Delete(review);
            await _reviewRepository.Save();
            await _cacheDatabase.Remove(review.Id.ToString());
        }
        
        public async Task<PaginationResponseModel<ReviewResponseModel>> GetAll(PaginationDTO pagination)
        {
            int count = await _reviewRepository.Count();
            IEnumerable<Review> reviews = await _reviewRepository.GetAll(pagination);
            return CreatePaginationResult(reviews, count, pagination);
        }

        public async Task Update(string userId, string reviewId, CreateOrUpdateReviewRequestModel model)
        {
            await new CreateOrUpdateReviewValidator().ValidateRequestModelAndThrow(model);

            Review review = await _reviewRepository.GetById(Guid.Parse(reviewId));
            if (review == null)
            {
                throw new ResourceNotFoundException("review not found.");
            }
            ThrowIfAuthenticatedUserNotBeReviewCreator(review, Guid.Parse(userId));

            review.Update(model.Title, model.Text, model.Stars);
            _reviewRepository.Update(review);
            await _reviewRepository.Save();
            await _cacheDatabase.Set(review.Id.ToString(), _jsonUtils.Serialize(review));
        }

        public async Task<ReviewResponseModel> GetById(string reviewId)
        {
            string reviewRegisteredOnCacheJson = await _cacheDatabase.Get(reviewId);
            if (reviewRegisteredOnCacheJson != null)
            {
                Review reviewRegisteredOnCache = _jsonUtils.Deserialize<Review>(reviewRegisteredOnCacheJson);
                return _converter.ConvertReviewToReviewResponseModel(reviewRegisteredOnCache);
            }

            Review review = await _reviewRepository.GetById(Guid.Parse(reviewId));
            if (review == null)
            {
                throw new ResourceNotFoundException("review not found.");
            }
            await _cacheDatabase.Set(review.Id.ToString(), _jsonUtils.Serialize(review));
            return _converter.ConvertReviewToReviewResponseModel(review);
        }

        public async Task<IEnumerable<ReviewResponseModel>> Search(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }
            IEnumerable<Review> reviews = await _reviewRepository.Search(text);
            return reviews.ToList().Select(r => _converter.ConvertReviewToReviewResponseModel(r));
        }

        private PaginationResponseModel<ReviewResponseModel> CreatePaginationResult(IEnumerable<Review> reviews, int totalReviewsInserteds, PaginationDTO pagination)
        {
            int previousPage = pagination.Page - 1;
            string previousPageUrl = previousPage > 0 ? $"{_webApplicationUrl}/reviews?page={previousPage}&quantityPerPage={pagination.QuantityPerPage}" : null;
            return new PaginationResponseModel<ReviewResponseModel>()
            {
                Data = reviews.Select(r => _converter.ConvertReviewToReviewResponseModel(r)),
                NextPage = $"{_webApplicationUrl}/reviews?page={pagination.Page + 1}&quantityPerPage={pagination.QuantityPerPage}",
                PreviousPage = previousPageUrl,
                Total = totalReviewsInserteds
            };
        }

        private async Task<ReviewImage> UploadImage(Stream image)
        {
            FileDTO uploadedImage = await _fileUploadUtils.UploadImage(image);
            return new ReviewImage(uploadedImage.FileName, uploadedImage.FilePath);
        }

        private void ThrowIfAuthenticatedUserNotBeReviewCreator(Review review, Guid userId)
        {
            if (!review.WasCreatedBy(userId))
            {
                throw new ForbiddenException();
            }
        }
    }
}
