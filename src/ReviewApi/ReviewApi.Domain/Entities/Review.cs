using System;

namespace ReviewApi.Domain.Entities
{
    public class Review : BaseEntity
    {
        public string Title { get; protected set; }
        public string Text { get; protected set; }
        public int Stars { get; protected set; }
        public Guid CreatorId { get; protected set; }
        public User Creator { get; protected set; }
        public ReviewImage Image { get; protected set; }

        public Review()
        {

        }

        public Review(string title, string text, int stars, Guid creatorId)
        {
            Title = title;
            Text = text;
            Stars = stars;
            CreatorId = creatorId;
        }

        public void AddImage(ReviewImage image)
        {
            Image = image;
        }

        public void Update(string title, string text, int stars)
        {
            Title = title;
            Text = text;
            Stars = stars;
        }
    }
}
