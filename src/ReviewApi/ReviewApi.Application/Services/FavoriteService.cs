﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReviewApi.Application.Converter;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Models;
using ReviewApi.Application.Models.Review;
using ReviewApi.Application.Models.User;
using ReviewApi.Domain.Dto;
using ReviewApi.Domain.Entities;
using ReviewApi.Domain.Exceptions;
using ReviewApi.Domain.Interfaces.Repositories;

namespace ReviewApi.Application.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConverter _converter;
        private readonly string _webApplicationUrl;

        public FavoriteService(IFavoriteRepository favoriteRepository, IReviewRepository reviewRepository, IUserRepository userRepository, IConverter converter, string webApplicationUrl)
        {
            _favoriteRepository = favoriteRepository;
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _converter = converter;
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

        public async Task<PaginationResponseModel<UserProfileResponseModel>> GetAllFromReview(string reviewId, PaginationDTO pagination)
        {
            Guid guidReviewId = Guid.Parse(reviewId);
            int previousPage = pagination.Page - 1;
            string previousPageUrl = previousPage > 0 ? $"{_webApplicationUrl}/reviews?page={previousPage}&quantityPerPage={pagination.QuantityPerPage}" : null;

            int totalReviewFavorites = await _favoriteRepository.CountByReviewId(guidReviewId);
            IEnumerable<Favorite> favorites = await _favoriteRepository.GetAllByReviewId(guidReviewId, pagination);

            return new PaginationResponseModel<UserProfileResponseModel>()
            {
                Data = favorites.Select(favorite => _converter.ConvertUserToUserProfileResponseModel(favorite.User)),
                NextPage = $"{_webApplicationUrl}/reviews/${reviewId}/favorites?page={pagination.Page + 1}&quantityPerPage={pagination.QuantityPerPage}",
                PreviousPage = previousPageUrl,
                Total = totalReviewFavorites
            };
        }

        public async Task<PaginationResponseModel<ReviewResponseModel>> GetAllFromUser(string userName, PaginationDTO pagination)
        {
            int previousPage = pagination.Page - 1;
            string previousPageUrl = previousPage > 0 ? $"{_webApplicationUrl}/reviews?page={previousPage}&quantityPerPage={pagination.QuantityPerPage}" : null;

            User registeredUser = await _userRepository.GetByName(userName);
            if (registeredUser == null)
            {
                throw new ResourceNotFoundException("User not found.");
            }

            int totalUserFavorites = await _favoriteRepository.CountByUserId(registeredUser.Id);
            IEnumerable<Favorite> favorites = await _favoriteRepository.GetAllByUserIdIncludingReview(registeredUser.Id, pagination);

            return new PaginationResponseModel<ReviewResponseModel>()
            {
                Data = favorites.Select(favorite => _converter.ConvertReviewToReviewResponseModel(favorite.Review, registeredUser.Name)),
                NextPage = $"{_webApplicationUrl}/reviews/${registeredUser.Name}/favorites?page={pagination.Page + 1}&quantityPerPage={pagination.QuantityPerPage}",
                PreviousPage = previousPageUrl,
                Total = totalUserFavorites
            };
        }

        private async Task ThrowIfReviewNotExists(Guid id)
        {
            if (!await _reviewRepository.AlreadyExists(id))
            {
                throw new ResourceNotFoundException("review not found.");
            }
        }
    }
}
