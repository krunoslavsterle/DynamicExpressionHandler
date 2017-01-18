using System;
using System.Linq.Expressions;

namespace DynamicExpression.Core
{
    public static class DynamicExpressionHandler
    {
        #region Fields

        private static string dynamicQueryString;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the query string from expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Query string from expression.</returns>
        public static string GetDynamicQueryString(BinaryExpression expression)
        {
            dynamicQueryString = String.Empty;
            HandleExpression(expression);
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
            var leftOperand = expression.Left;
            var rightOperand = expression.Right;

            var propertyName = leftOperand.ToString().Split('.')[1];
            var propertyValue = rightOperand.ToString().Replace("\"", "");

            return String.Format("{0}{1}{2}", propertyName, GetOperand(expression.NodeType), propertyValue);
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
