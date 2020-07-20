namespace ReviewApi.Domain.Entities
{
    internal class BaseEntity
    {
        public int Id { get; protected set; }
        public bool Deleted { get; protected set; }

        public void SetAsDeleted() 
        {
            Deleted = true; 
        }

        public BaseEntity()
        {

        }

        public BaseEntity(int id)
        {
            Id = id; 
        }
    }
}
