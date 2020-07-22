using System;

namespace ReviewApi.Domain.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; protected set; }
        public bool Deleted { get; protected set; }

        public void SetAsDeleted() 
        {
            Deleted = true; 
        }

        public BaseEntity()
        {

        }

        public BaseEntity(Guid id)
        {
            Id = id; 
        }
    }
}
