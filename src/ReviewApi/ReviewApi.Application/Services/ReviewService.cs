using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly string _webApplicationUrl;

        public ReviewService(IReviewRepository reviewRepository, IFileUploadUtils fileUploadUtils, ICacheDatabase cacheDatabase, IJsonUtils jsonUtils, string webApplicationUrl)
        {
            _reviewRepository = reviewRepository;
            _fileUploadUtils = fileUploadUtils;
            _cacheDatabase = cacheDatabase;
            _jsonUtils = jsonUtils;
            _webApplicationUrl = webApplicationUrl;
        }

        public async Task<IdResponseModel> Create(string userId, CreateOrUpdateReviewRequestModel model)
        {
            await new CreateOrUpdateReviewValidator().ValidateRequestModelAndThrow(model);

            Review review = new Review(model.Title, model.Text, model.Stars, Guid.Parse(userId));
            review.AddImage(await UploadImage(model.Image));

            await _reviewRepository.Create(review);
            await _reviewRepository.Save();
            await _cacheDatabase.Set(review.Id.ToString(), _jsonUtils.Serialize(review));

            return new IdResponseModel() { Id = review.Id };
        }

        public async Task Delete(string userId, string reviewId)
        {
            Review review = await _reviewRepository.GetById(Guid.Parse(reviewId));
            if (review == null)
            {
                throw new ResourceNotFoundException("review not found.");
            }
            VerifyIfAuthenticatedIsReviewCreatorAndThrow(review, Guid.Parse(userId));
            _reviewRepository.Delete(review);
            await _reviewRepository.Save();
            await _cacheDatabase.Remove(review.Id.ToString());
        }

        public async Task<PaginationResponseModel<ReviewResponseModel>> GetAll(int page = 1, int quantityPerPage = 14)
        {
            int count = await _reviewRepository.Count();
            IEnumerable<Review> reviews = await _reviewRepository.GetAll(page, quantityPerPage);
            return CreatePaginationResult(reviews, count, page, quantityPerPage);
        }

        public async Task Update(string userId, string reviewId, CreateOrUpdateReviewRequestModel model)
        {
            await new CreateOrUpdateReviewValidator().ValidateRequestModelAndThrow(model);

            Review review = await _reviewRepository.GetById(Guid.Parse(reviewId));
            if (review == null)
            {
                throw new ResourceNotFoundException("review not found.");
            }
            VerifyIfAuthenticatedIsReviewCreatorAndThrow(review, Guid.Parse(userId));

            review.Update(model.Title, model.Text, model.Stars);
            _reviewRepository.Update(review);
            await _reviewRepository.Save();
            await _cacheDatabase.Set(review.Id.ToString(), _jsonUtils.Serialize(review));
        }

        public async Task<ReviewResponseModel> GetById(string userId, string reviewId)
        {
            string reviewRegisteredOnCacheJson = await _cacheDatabase.Get(reviewId);
            if (reviewRegisteredOnCacheJson == null)
            {
                Review review = await _reviewRepository.GetById(Guid.Parse(reviewId));
                if (review == null)
                {
                    throw new ResourceNotFoundException("review not found.");
                }
                await _cacheDatabase.Set(review.Id.ToString(), _jsonUtils.Serialize(review));
                return ConvertReviewToReviewResponseModel(review);
            }
            Review reviewRegisteredOnCache = _jsonUtils.Deserialize<Review>(reviewRegisteredOnCacheJson);
            return ConvertReviewToReviewResponseModel(reviewRegisteredOnCache);
        }

        private PaginationResponseModel<ReviewResponseModel> CreatePaginationResult(IEnumerable<Review> reviews, int totalReviewsInserteds, int actualPage, int quantityPerPage)
        {
            int previousPage = actualPage - 1;
            if (previousPage == 0)
            {
                previousPage = 1;
            }
            return new PaginationResponseModel<ReviewResponseModel>()
            {
                Data = reviews.Select(r => ConvertReviewToReviewResponseModel(r)),
                NextPage = $"{_webApplicationUrl}/reviews?page={actualPage + 1}&quantity={quantityPerPage}",
                PreviousPage = $"{_webApplicationUrl}/reviews?page={previousPage}&quantity={quantityPerPage}",
                Total = totalReviewsInserteds
            };
        }

        private ReviewResponseModel ConvertReviewToReviewResponseModel(Review review)
        {
            string reviewImageUrl = _fileUploadUtils.GenerateImageUrl(review.Image.FileName);
            return new ReviewResponseModel()
            {
                Id = review.Id,
                Image = reviewImageUrl,
                Creator = review.Creator.Name,
                Stars = review.Stars,
                Text = review.Text,
                Title = review.Title
            };
        }

        private async Task<ReviewImage> UploadImage(Stream image)
        {
            FileDTO uploadedImage = await _fileUploadUtils.UploadImage(image);
            return new ReviewImage(uploadedImage.FileName, uploadedImage.FilePath);
        }

        private void VerifyIfAuthenticatedIsReviewCreatorAndThrow(Review review, Guid userId)
        {
            if (!review.WasCreatedAt(userId))
            {
                throw new ForbiddenException();
            }
        }
    }
}
