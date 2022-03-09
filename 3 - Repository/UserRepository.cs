using System.Collections.Generic;
using System.Linq;
using Domain;

namespace Repository
{
    public static class UserRepository
    {
        public static User? Get(string username, string password) 
        {
            var users = new List<User>
            {
                new User(1, "batman", "batman", "manager"),
                new User(2, "robin", "robin", "employee"),
                new User(3, "batgirl", "batgirl", "trainee")
            };

            return users.Where(u => u.Username == username && u.Password == password).FirstOrDefault();
        }
    }
}