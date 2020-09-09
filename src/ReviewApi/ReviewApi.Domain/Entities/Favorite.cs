using System;

namespace ReviewApi.Domain.Entities
{
    public class Favorite : BaseEntity
    {
        public Guid UserId { get; protected set; }
        public User User { get; protected set; }
        public Guid ReviewId { get; protected set; }
        public Review Review { get; protected set; }

        public Favorite()
        {

        }

        public Favorite(Guid userId, Guid reviewId)
        {
            UserId = userId;
            ReviewId = reviewId;
        }
    }
}
