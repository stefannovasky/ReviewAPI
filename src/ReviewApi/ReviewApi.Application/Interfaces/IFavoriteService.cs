using System;
using System.Threading.Tasks;
using ReviewApi.Application.Models;

namespace ReviewApi.Application.Interfaces
{
    public interface IFavoriteService
    {
        Task<IdResponseModel> Create(string userId, string reviewId);
    }
}
