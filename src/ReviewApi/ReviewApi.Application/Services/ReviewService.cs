﻿using System;
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
using ReviewApi.Shared.Interfaces;

namespace ReviewApi.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IFileUploadUtils _fileUploadUtils;
        private readonly IUserRepository _userRepository;

        public ReviewService(IReviewRepository reviewRepository, IUserRepository userRepository, IFileUploadUtils fileUploadUtils)
        {
            _reviewRepository = reviewRepository;
            _userRepository = userRepository; 
            _fileUploadUtils = fileUploadUtils;
        }

        public async Task<IdResponseModel> Create(string userId, CreateOrUpdateReviewRequestModel model)
        {
            await new CreateOrUpdateReviewValidator().ValidateRequestModelAndThrow(model);
            await VerifyIfUserNotExistsOrNotConfirmedAndThrow(Guid.Parse(userId));

            Review review = new Review(model.Title, model.Text, model.Stars, Guid.Parse(userId));
            review.AddImage(await UploadImage(model.Image));

            await _reviewRepository.Create(review);
            await _reviewRepository.Save();

            return new IdResponseModel() { Id = review.Id };
        }

        public async Task<IEnumerable<ReviewResponseModel>> GetAll(int page = 1)
        {
            IEnumerable<Review> reviews = await _reviewRepository.GetAll(page);
            return reviews.Select(r => ConvertReviewToReviewResponseModel(r));
        }

        private ReviewResponseModel ConvertReviewToReviewResponseModel(Review review)
        {
            string reviewImageUrl = _fileUploadUtils.GenerateImageUrl(review.Image.FileName);
            return new ReviewResponseModel()
            {
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

        private async Task VerifyIfUserNotExistsOrNotConfirmedAndThrow(Guid userId)
        {
            User user = await _userRepository.GetById(userId);
            if (user == null)
            {
                throw new ResourceNotFoundException("user not found.");
            }
            if (!user.Confirmed)
            {
                throw new UserNotConfirmedException();
            }
        }
    }
}
