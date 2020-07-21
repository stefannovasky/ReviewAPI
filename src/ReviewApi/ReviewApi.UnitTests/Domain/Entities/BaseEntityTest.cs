using ReviewApi.Domain.Entities;
using Xunit;

namespace ReviewApi.UnitTests.Domain.Entities
{
    public class BaseEntityTest
    {
        [Fact]
        public void ShouldConstructBaseEntityWithEmptyConstructor()
        {
            BaseEntity baseEntity = new BaseEntity();

            Assert.Equal(0, baseEntity.Id);
            Assert.False(baseEntity.Deleted);
        }

        [Fact]
        public void ShouldConstructBaseEntityWithIdConstructor()
        {
            int id = 1;
            BaseEntity baseEntity = new BaseEntity(id);

            Assert.Equal(id, baseEntity.Id);
            Assert.False(baseEntity.Deleted);
        }

        [Fact]
        public void ShouldDelete()
        {
            BaseEntity baseEntity = new BaseEntity();

            baseEntity.SetAsDeleted();

            Assert.True(baseEntity.Deleted);
        }
    }
}
