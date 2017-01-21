using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace TestExpression
{
    class Program
    {
        static void Main(string[] args)
        {
            var userRepository = new UserRepository();
            var user = new User() { Age = 33, DateCreated = DateTime.Now, Description = "Description", Id = Guid.Empty, Name = "Namee", OwnerId = Guid.NewGuid() };

            var usr = UserRepository.Users.First();
            var query = userRepository.GetUser(p => p.Id == usr.Id);

            Console.WriteLine(query);
            Console.Read();
        }
    }
}
