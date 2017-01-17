using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TestExpression
{
    public class UserRepository
    {
        public string GetUser(Expression<Func<User, bool>> filter)
        {
            return DynamicExpressionHandler.DynamicExpressionHandler.GetDynamicQueryString((BinaryExpression)filter.Body);
        }
    }
}
