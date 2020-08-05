using System.IO;

namespace ReviewApi.Application.Models.Review
{
    public class CreateOrUpdateReviewRequestModel
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public int Stars { get; set; }
        public Stream Image { get; set; }
    }
}
