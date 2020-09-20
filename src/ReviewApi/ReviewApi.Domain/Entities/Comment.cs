using System;

namespace ReviewApi.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string Text { get; protected set; }
        public Guid ReviewId { get; protected set; }
        public Review Review { get; protected set; }
        public Guid UserId { get; protected set; }
        public User User { get; protected set; }

        public Comment() : base()
        {

        }

        public Comment(Guid id) : base(id)
        {

        }

        public Comment(string text, Guid userId, Guid reviewId)
        {
            Text = text;
            UserId = userId;
            ReviewId = reviewId;
        }

        public void UpdateText(string text)
        {
            Text = text; 
        }
    }
}
