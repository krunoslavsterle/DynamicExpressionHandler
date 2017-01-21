using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using DynamicExpression.Core;

namespace TestExpression
{
    public class UserRepository
    {
        public static List<User> Users { get; set; }

        public UserRepository()
        {
            Users = new List<User>();

            for(int i = 0; i < 50; i ++)
            {
                var user = new User();
                user.Age = i * 2;
                user.DateCreated = DateTime.Now;
                user.Description = String.Format("{0}_user desctiption", i.ToString());
                user.Id = Guid.NewGuid();
                user.Name = String.Format("{0} user", i.ToString());
                user.OwnerId = Guid.Empty;

                Users.Add(user);
            }
        }

        public string GetUser(Expression<Func<User, bool>> filter)
        {
            //return Users.Where("(OwnerId = Guid(\"00000000-0000-0000-0000-000000000000\"))").FirstOrDefault();
           // return System.Linq.Dynamic.
           return DynamicExpressionHandler.GetDynamicQueryString(filter.Body);
        }

        
    }
}
