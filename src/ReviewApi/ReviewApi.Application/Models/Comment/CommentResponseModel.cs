using System;
using ReviewApi.Application.Models.User;

namespace ReviewApi.Application.Models.Comment
{
    public class CommentResponseModel
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public UserProfileResponseModel User { get; set; }
    }
}
