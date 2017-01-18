using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DynamicExpression.Core;

namespace TestExpression
{
    public class UserRepository
    {
        public string GetUser(Expression<Func<User, bool>> filter)
        {
            return DynamicExpressionHandler.GetDynamicQueryString((BinaryExpression)filter.Body);
        }
    }
}
