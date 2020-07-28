using System.Collections.Generic;
using ReviewApi.Domain.Entities;
using ReviewApi.Infra.Context;

namespace ReviewApi.IntegrationTests.Initiliazers
{
    internal class UsersDbDataInitializer : DbDataInitializer<User>
    {
        public List<User> InsertedsConfirmedUsers { get; set; }

        public UsersDbDataInitializer(MainContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<User>();

            CreateConfirmedUsersInDatabase();
        }

        public void CreateConfirmedUsersInDatabase()
        {
            InsertedsConfirmedUsers = new List<User>()
            {
                new User("User Name 1", "user1@mail.com", "user password"),
                new User("User Name 2", "user2@mail.com", "user password"),
                new User("User Name 3", "user3@mail.com", "user password")
            };

            foreach (User user in InsertedsConfirmedUsers)
            {
                _dbSet.Add(user);
                _dbContext.SaveChanges();
                user.Confirm();
                _dbSet.Update(user);
                _dbContext.SaveChanges();
            }
        }
    }
}
