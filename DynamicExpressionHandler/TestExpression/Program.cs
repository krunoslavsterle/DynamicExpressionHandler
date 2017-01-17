using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicExpressionHandler;

namespace TestExpression
{
    class Program
    {
        static void Main(string[] args)
        {
            var userRepository = new UserRepository();
            var query = userRepository.GetUser(p => p.Name == "Some name" && (p.Description == "dsafsdfsdfs" || p.Age == 6 || p.Id == Guid.Empty));

            Console.WriteLine(query);
            Console.Read();
        }
    }
}
