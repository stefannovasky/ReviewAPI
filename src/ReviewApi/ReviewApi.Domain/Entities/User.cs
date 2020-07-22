using System;

namespace ReviewApi.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; protected set; }
        public string Email { get; protected set; }
        public string Password { get; protected set; }

        public User()
        {

        }

        public User(Guid id) : base(id)
        {

        }

        public User(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;
        }

        public User(Guid id, string name, string email, string password) : base(id)
        {
            Name = name;
            Email = email;
            Password = password; 
        }

        public void UpdatePassword(string password)
        {
            Password = password; 
        }
    }
}
