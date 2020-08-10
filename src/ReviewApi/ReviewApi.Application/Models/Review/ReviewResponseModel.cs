using System;

namespace ReviewApi.Application.Models.Review
{
    public class ReviewResponseModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public int Stars { get; set; }
        public string Creator { get; set; }
    }
}
