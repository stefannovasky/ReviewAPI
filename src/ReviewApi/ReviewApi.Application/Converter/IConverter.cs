using System;
using System.Collections.Generic;
using System.Text;
using ReviewApi.Application.Models.Review;
using ReviewApi.Application.Models.User;
using ReviewApi.Domain.Entities;

namespace ReviewApi.Application.Converter
{
    public interface IConverter
    {
        ReviewResponseModel ConvertReviewToReviewResponseModel(Review review);
        ReviewResponseModel ConvertReviewToReviewResponseModel(Review review, string userName);
        UserProfileResponseModel ConvertUserToUserProfileResponseModel(User user);
    }
}
