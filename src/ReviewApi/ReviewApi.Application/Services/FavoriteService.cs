using System;
using System.Threading.Tasks;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Models;
using ReviewApi.Domain.Entities;
using ReviewApi.Domain.Exceptions;
using ReviewApi.Domain.Interfaces.Repositories;

namespace ReviewApi.Application.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IReviewRepository _reviewRepository; 

        public FavoriteService(IFavoriteRepository favoriteRepository, IReviewRepository reviewRepository)
        {
            _favoriteRepository = favoriteRepository;
            _reviewRepository = reviewRepository; 
        }

        public async Task<IdResponseModel> Create(string userId, string reviewId)
        {
            Guid userIdGuid = Guid.Parse(userId);
            Guid reviewIdGuid = Guid.Parse(reviewId);

            await ThrowIfReviewNotExists(reviewIdGuid);
            await ThrowIfReviewAlreadyExists(userIdGuid, reviewIdGuid);
          
            Favorite favorite = new Favorite(Guid.Parse(userId), Guid.Parse(reviewId));
            await _favoriteRepository.Create(favorite);
            await _favoriteRepository.Save();

            return new IdResponseModel() { Id = favorite.Id }; 
        }

        private async Task ThrowIfReviewNotExists(Guid id) 
        {
            if (!await _reviewRepository.AlreadyExists(id))
            {
                throw new ResourceNotFoundException("review not found.");
            }
        }

        private async Task ThrowIfReviewAlreadyExists(Guid userId, Guid reviewId)
        {
            if (await _favoriteRepository.AlreadyExists(userId, reviewId))
            {
                throw new AlreadyExistsException("favorite");
            }
        }
    }
}
