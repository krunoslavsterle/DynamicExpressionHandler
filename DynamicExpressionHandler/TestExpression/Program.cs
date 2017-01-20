using System;
using System.Linq.Expressions;
using System.Reflection;

namespace TestExpression
{
    class Program
    {
        static void Main(string[] args)
        {
            var userRepository = new UserRepository();
            var user = new User() { Age = 33, DateCreated = DateTime.Now, Description = "Description", Id = Guid.Empty, Name = "Namee", OwnerId = Guid.NewGuid() };
            
            var query = userRepository.GetUser(p => p.Name == user.Name && p.Age == user.Age || p.Description == user.Description);

            Console.WriteLine(query);
            Console.Read();
        }
    }
}
