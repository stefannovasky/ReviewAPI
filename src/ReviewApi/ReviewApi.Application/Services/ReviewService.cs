using System;
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

        private readonly User _fakeInsertedConfirmedUser;

        public ReviewService(IReviewRepository reviewRepository, IUserRepository userRepository, IFileUploadUtils fileUploadUtils)
        {
            _reviewRepository = reviewRepository;
            _userRepository = userRepository; 
            _fileUploadUtils = fileUploadUtils;

            _fakeInsertedConfirmedUser = new User(Guid.NewGuid(), "User Name", "user@mail.com", "password");
        }

        public async Task<IdResponseModel> Create(string userId, CreateOrUpdateReviewRequestModel model)
        {
            await new CreateOrUpdateReviewValidator().ValidateRequestModelAndThrow(model);
            await VerifyIfUserNotExistsOrNotConfirmedAndThrow(Guid.Parse(userId));

            Review review = new Review(model.Title, model.Text, model.Stars, Guid.Parse(userId));
            FileDTO uploadedImage = await _fileUploadUtils.UploadImage(model.Image);
            ReviewImage image = new ReviewImage(uploadedImage.FileName, uploadedImage.FilePath);
            review.AddImage(image);

            await _reviewRepository.Create(review);
            await _reviewRepository.Save();

            return new IdResponseModel() { Id = review.Id };
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
