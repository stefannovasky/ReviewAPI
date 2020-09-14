using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Models;
using ReviewApi.Application.Models.Review;
using ReviewApi.Application.Models.User;
using ReviewApi.Domain.Entities;
using ReviewApi.Domain.Exceptions;
using ReviewApi.Domain.Interfaces.Repositories;
using ReviewApi.Shared.Interfaces;

namespace ReviewApi.Application.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFileUploadUtils _fileUploadUtils;
        private readonly string _webApplicationUrl;

        public FavoriteService(IFavoriteRepository favoriteRepository, IReviewRepository reviewRepository, IUserRepository userRepository, IFileUploadUtils fileUploadUtils, string webApplicationUrl)
        {
            _favoriteRepository = favoriteRepository;
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _fileUploadUtils = fileUploadUtils;
            _webApplicationUrl = webApplicationUrl;
        }

        public async Task CreateOrDelete(string userId, string reviewId)
        {
            Guid userIdGuid = Guid.Parse(userId);
            Guid reviewIdGuid = Guid.Parse(reviewId);

            Favorite insertedFavorite = await _favoriteRepository.GetByUserIdAndReviewId(userIdGuid, reviewIdGuid);
            if (insertedFavorite == null)
            {
                await ThrowIfReviewNotExists(reviewIdGuid);

                Favorite favorite = new Favorite(userIdGuid, reviewIdGuid);
                await _favoriteRepository.Create(favorite);
                await _favoriteRepository.Save();
                return;
            }

            _favoriteRepository.Delete(insertedFavorite);
            await _favoriteRepository.Save();
        }

        public async Task<PaginationResponseModel<UserProfileResponseModel>> GetAllFromReview(string reviewId, int page = 1, int quantityPerPage = 14)
        {
            if (page < 1)
            {
                page = 1;
            }
            if (quantityPerPage < 1)
            {
                quantityPerPage = 1;
            }

            Guid guidReviewId = Guid.Parse(reviewId);
            int previousPage = page - 1;
            string previousPageUrl = previousPage > 0 ? $"{_webApplicationUrl}/reviews?page={previousPage}&quantity={quantityPerPage}" : null;

            int totalReviewFavorites = await _favoriteRepository.CountByReviewId(guidReviewId);
            IEnumerable<Favorite> favorites = await _favoriteRepository.GetAllByReviewId(guidReviewId, page, quantityPerPage);

            return new PaginationResponseModel<UserProfileResponseModel>()
            {
                Data = favorites.Select(favorite => ConvertUserToUserProfileResponseModel(favorite.User)),
                NextPage = $"{_webApplicationUrl}/reviews/${reviewId}/favorites?page={page + 1}&quantity={quantityPerPage}",
                PreviousPage = previousPageUrl,
                Total = totalReviewFavorites
            };
        }

        public async Task<PaginationResponseModel<ReviewResponseModel>> GetAllFromUser(string userName, int page = 1, int quantityPerPage = 14)
        {
            if (page < 1)
            {
                page = 1;
            }
            if (quantityPerPage < 1)
            {
                quantityPerPage = 1;
            }
            int previousPage = page - 1;
            string previousPageUrl = previousPage > 0 ? $"{_webApplicationUrl}/reviews?page={previousPage}&quantity={quantityPerPage}" : null;

            User registeredUser = await _userRepository.GetByName(userName);
            if (registeredUser == null)
            {
                throw new ResourceNotFoundException("User not found.");
            }

            int totalUserFavorites = await _favoriteRepository.CountByUserId(registeredUser.Id);
            IEnumerable<Favorite> favorites = await _favoriteRepository.GetAllByUserIdIncludingReview(registeredUser.Id, page, quantityPerPage);

            return new PaginationResponseModel<ReviewResponseModel>()
            {
                Data = favorites.Select(favorite => ConvertReviewToReviewResponseModel(favorite.Review, registeredUser.Name)),
                NextPage = $"{_webApplicationUrl}/reviews/${registeredUser.Name}/favorites?page={page + 1}&quantity={quantityPerPage}",
                PreviousPage = previousPageUrl,
                Total = totalUserFavorites
            };
        }

        private UserProfileResponseModel ConvertUserToUserProfileResponseModel(User user)
        {
            return new UserProfileResponseModel()
            {
                Email = user.Email,
                Name = user.Name,
                Image = _fileUploadUtils.GenerateImageUrl(user.ProfileImage.FileName)
            };
        }

        private ReviewResponseModel ConvertReviewToReviewResponseModel(Review review, string userName) 
        {
            return new ReviewResponseModel()
            {
                Creator = userName,
                Id = review.Id,
                Image = _fileUploadUtils.GenerateImageUrl(review.Image.FileName),
                Stars = review.Stars,
                Text = review.Text,
                Title = review.Title
            };
        }

        private async Task ThrowIfReviewNotExists(Guid id) 
        {
            if (!await _reviewRepository.AlreadyExists(id))
            {
                throw new ResourceNotFoundException("review not found.");
            }
        }

        private async Task<bool> FavoriteAlreadyExists(Guid userId, Guid reviewId)
        {
            if (await _favoriteRepository.AlreadyExists(userId, reviewId))
            {
                return true;
            }
            return false; 
        }
    }
}
