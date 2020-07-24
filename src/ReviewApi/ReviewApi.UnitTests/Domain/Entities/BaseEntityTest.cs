using System;
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

            Assert.Equal(Guid.Empty, baseEntity.Id);
        }

        [Fact]
        public void ShouldConstructBaseEntityWithIdConstructor()
        {
            Guid id = Guid.Empty;
            BaseEntity baseEntity = new BaseEntity(id);

            Assert.Equal(id, baseEntity.Id);
        }
    }
}
