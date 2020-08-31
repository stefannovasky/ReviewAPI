using System;
using System.Threading.Tasks;
using ReviewApi.Application.Models;

namespace ReviewApi.Application.Interfaces
{
    public interface IFavoriteService
    {
        /// <summary>
        ///     Create favorite if favorite not exists.
        ///     Delete favorite if already exists. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        Task CreateOrDelete(string userId, string reviewId);
    }
}
