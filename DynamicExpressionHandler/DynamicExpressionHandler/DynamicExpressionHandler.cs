using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DynamicExpression.Core
{
    /// <summary>
    /// Dynamic expression handler.
    /// </summary>
    public static class DynamicExpressionHandler
    {
        #region Fields

        private static string dynamicQueryString;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the query string from expression.
        /// </summary>
        /// <param name="expressionBody">The expression body.</param>
        /// <returns>Query string from expression.</returns>
        public static string GetDynamicQueryString(Expression expressionBody)
        {
            dynamicQueryString = String.Empty;
            HandleExpression((BinaryExpression)expressionBody);
            return dynamicQueryString;
        }
        
        /// <summary>
        /// Handles the expression and generates query string.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <exception cref="NullReferenceException">The specific NodeType is not supported: " + expression.NodeType.ToString()</exception>
        private static void HandleExpression(BinaryExpression expression, ExpressionType? expressionType = null)
        {
            if (CheckIsSingleExpression(expression.NodeType))
            {
                dynamicQueryString += GetExpressionQueryString(expression);
            }
            else if (CheckIsMultiExpression(expression.NodeType))
            {
                HandleExpression((BinaryExpression)expression.Left);
                dynamicQueryString += String.Format(" {0} ", GetOperand(expression.NodeType));
                HandleExpression((BinaryExpression)expression.Right);
            }
            else
            {
                throw new NullReferenceException("The specific NodeType is not supported: " + expression.NodeType.ToString());
            }
        }

        /// <summary>
        /// Gets the query string form the provided expression.  
        /// </summary>
        /// <remarks>
        /// Use on the 'property' - 'value' expression, when there are no logical operators for concatenating two expressions (eg. 'AndAlso').
        /// </remarks>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        private static string GetExpressionQueryString(BinaryExpression expression)
        {
            // TODO: GetExpressionQueryString() - Handle more complex types.

            var leftOperand = expression.Left;
            var rightOperand = expression.Right;

            var propertyName = leftOperand.ToString().Split('.')[1];
            var propertyValue = GetExpressionPropertyValue(rightOperand);

            // TODO: GetExpressionQueryString() - this is temp solution, need to handle more complex types.
            if (rightOperand.Type == String.Empty.GetType())
                return String.Format("{0} {1} \"{2}\"", propertyName, GetOperand(expression.NodeType), propertyValue);
            else if (rightOperand.Type == Guid.Empty.GetType())
            {
                return String.Format("({0}.Equals({1}))", propertyName, propertyValue);
            }
            else
                throw new Exception("Type not supported: " + rightOperand.Type.ToString());
        }
        
        /// <summary>
        /// Gets the property value from the expression.
        /// </summary>
        /// <param name="memberExpression">The member expression.</param>
        /// <returns>Value.</returns>
        private static string GetExpressionPropertyValue(Expression exp)
        {
            MemberExpression memberExpression = null;
            ConstantExpression constantExpression = null;

            if (exp.NodeType == ExpressionType.Constant)
            {
                constantExpression = (ConstantExpression)exp;
                return constantExpression.Value.ToString();
            }
            
            else if (exp.NodeType == ExpressionType.Convert)
            {
                memberExpression = (MemberExpression)((UnaryExpression)exp).Operand;
            }
            else if (exp.NodeType == ExpressionType.MemberAccess)
                memberExpression = (MemberExpression)exp;

            try
            {
                // TODO: GetExpressionPropertyValue() - in memory product caching.
                if (memberExpression.Expression.NodeType == ExpressionType.Constant)
                {
                    // If expression is a constant by itself, get the value from it. 
                    object val = null;
                    ConstantExpression cc = (ConstantExpression)memberExpression.Expression;

                    if (memberExpression.Member.MemberType == MemberTypes.Field)
                        val = ((FieldInfo)memberExpression.Member).GetValue(cc.Value);
                    else if (memberExpression.Member.MemberType == MemberTypes.Property)
                        val = ((PropertyInfo)memberExpression.Member).GetValue(cc.Value);
                    else
                        throw new Exception("Member expression type not handeled. Type: " + memberExpression.Member.MemberType.ToString());

                    return GetFormatedString(memberExpression, val.ToString());
                }
                else
                {
                    MemberExpression objectExpression = (MemberExpression)memberExpression.Expression;
                    ConstantExpression objectConstant = (ConstantExpression)objectExpression.Expression;

                    object product = ((FieldInfo)objectExpression.Member).GetValue(objectConstant.Value);
                    object value = ((PropertyInfo)memberExpression.Member).GetValue(product, null);

                    return GetFormatedString(memberExpression, value);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Get formated string for provided value based on expression type.
        /// </summary>
        /// <param name="memberExpression">Expression.</param>
        /// <param name="value">Value.</param>
        /// <returns>Formated value.</returns>
        private static string GetFormatedString(MemberExpression memberExpression, object value) 
        {
            if (memberExpression.Type == Guid.Empty.GetType())
                return String.Format("Guid(\"{0}\")", value.ToString());
            else if (memberExpression.Type == String.Empty.GetType())
                return value.ToString();

            throw new Exception("Type not supported: " + memberExpression.Type);
        }

        /// <summary>
        /// Gets the logical operand for query string.
        /// </summary>
        /// <param name="expType">Expression type.</param>
        /// <returns>Logical operand for query string.</returns>
        /// <exception cref="NullReferenceException">Operand not exists: " + expType.ToString()</exception>
        private static string GetOperand(ExpressionType expType)
        {
            switch (expType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return "AND";

                case ExpressionType.Equal:
                    return "=";

                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";

                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";

                case ExpressionType.Not:
                case ExpressionType.NotEqual:
                    return "NOT";

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return "OR";

                default:
                    throw new NullReferenceException("Operand not exists: " + expType.ToString());
            }
        }

        /// <summary>
        /// Checks if provided expression type is for single expression.
        /// </summary>
        /// <remarks>
        /// Single expression is when there are no logical operators for concatenating two expressions (eg. 'AndAlso').
        /// </remarks>
        /// <param name="expressionType">Type of the expression.</param>
        /// <returns>True if provided expression type is for single expression.</returns>
        private static bool CheckIsSingleExpression(ExpressionType expressionType)
        {
            if (expressionType == ExpressionType.Equal
                || expressionType == ExpressionType.GreaterThan
                || expressionType == ExpressionType.GreaterThanOrEqual
                || expressionType == ExpressionType.LessThan
                || expressionType == ExpressionType.LessThanOrEqual
                || expressionType == ExpressionType.Not
                || expressionType == ExpressionType.NotEqual)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if provided expression type is for multi expression.
        /// </summary>
        /// <remarks>
        /// Multi expression is when there are logical operators for concatenating two expressions (eg. 'AndAlso').
        /// </remarks>
        /// <param name="expressionType">Type of the expression.</param>
        /// <returns>True if provided expression type is for multi expression.</returns>
        private static bool CheckIsMultiExpression(ExpressionType expressionType)
        {
            if (expressionType == ExpressionType.And
                || expressionType == ExpressionType.AndAlso
                || expressionType == ExpressionType.Or
                || expressionType == ExpressionType.OrElse)
            {
                return true;
            }
            return false;
        }

        #endregion Methods
    }
}
