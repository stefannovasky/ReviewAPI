using System;

namespace ReviewApi.Domain.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; protected set; }

        public BaseEntity()
        {

        }

        public BaseEntity(Guid id)
        {
            Id = id; 
        }
    }
}
