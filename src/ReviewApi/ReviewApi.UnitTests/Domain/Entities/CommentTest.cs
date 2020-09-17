using System;
using ReviewApi.Domain.Entities;
using Xunit;

namespace ReviewApi.UnitTests.Domain.Entities
{
    public class CommentTest
    {
        [Fact]
        public void ShouldConstructWithParameterlessConstructor() 
        {
            Comment comment = new Comment();

            Assert.Null(comment.Text);
            Assert.Null(comment.Review);
            Assert.Null(comment.User);
            Assert.Equal(Guid.Empty, comment.ReviewId);
            Assert.Equal(Guid.Empty, comment.UserId);
            Assert.Equal(Guid.Empty, comment.Id);
        }

        [Fact]
        public void ShouldConstructWithOnlyIdConstructor()
        {
            Guid id = Guid.NewGuid();
            Comment comment = new Comment(id);

            Assert.Null(comment.Text);
            Assert.Null(comment.Review);
            Assert.Null(comment.User);
            Assert.Equal(Guid.Empty, comment.ReviewId);
            Assert.Equal(Guid.Empty, comment.UserId);
            Assert.Equal(id, comment.Id);
        }

        [Fact]
        public void ShouldConstructWithReviewIdAndUserIdConstructor()
        {
            Guid userId = Guid.NewGuid();
            Guid reviewId = Guid.NewGuid();
            string text = "TEXT";
            Comment comment = new Comment(text, userId, reviewId);

            Assert.Null(comment.Review);
            Assert.Null(comment.User);
            Assert.Equal(text, comment.Text);
            Assert.Equal(reviewId, comment.ReviewId);
            Assert.Equal(userId, comment.UserId);
            Assert.Equal(Guid.Empty, comment.Id);
        }
    }
}
