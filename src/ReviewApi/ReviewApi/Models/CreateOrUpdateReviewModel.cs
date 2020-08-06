using Microsoft.AspNetCore.Http;

namespace ReviewApi.Models
{
    public class CreateOrUpdateReviewModel
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public int Stars { get; set; }
        public IFormFile Image { get; set; }
    }
}
