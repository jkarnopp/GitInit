using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace GitInitTest.Common.Helpers
{
    public enum ExpressionType
    {
        Equal = 1,
        LessThanOrEqual = 2,
        GreaterThanOrEqual = 3,
        StartsWith = 4,
        EndsWith = 5,
        Contains = 6
    }

    public static class ExpressionBuilder<T>
    {
        public static Expression<Func<T, bool>> GetExpression(object dto)
        {
            if (dto == null) return null;

            Expression combinedExpression = null;

            ParameterExpression parameterExpression = Expression.Parameter(typeof(T));

            foreach (PropertyInfo field in dto.GetType().GetProperties())
            {
                var fieldName = field.Name;
                var fieldValue = field.GetValue(dto, null)?.ToString();

                if (fieldName.StartsWith("lt_"))
                {
                    fieldName = fieldName.Replace("lt_", "");
                    if (fieldValue != null) fieldValue = "<" + fieldValue;
                }

                if (fieldName.StartsWith("gt_"))
                {
                    fieldName = fieldName.Replace("gt_", "");
                    if (fieldValue != null) fieldValue = ">" + fieldValue;
                }

                if (typeof(T).GetProperty(fieldName) == null) continue;

                if (fieldValue != null)
                    combinedExpression = combinedExpression == null
                        ? GetStringExpression(parameterExpression, fieldName, fieldValue)
                        : Expression.And(combinedExpression, GetStringExpression(parameterExpression, fieldName, fieldValue));
            }

            if (combinedExpression != null)
                return Expression.Lambda<Func<T, Boolean>>(combinedExpression, parameterExpression);

            return null;
        }

        private static Expression GetStringExpression(ParameterExpression pe, string property, string dtoProperty)
        {
            if (dtoProperty.StartsWith("*") && dtoProperty.EndsWith("*"))
                return GetExpression(pe, property, dtoProperty.Replace("*", ""), ExpressionType.Contains);
            if (dtoProperty.StartsWith("*"))
                return GetExpression(pe, property, dtoProperty.Replace("*", ""), ExpressionType.EndsWith);
            if (dtoProperty.EndsWith("*"))
                return GetExpression(pe, property, dtoProperty.Replace("*", ""), ExpressionType.StartsWith);
            if (dtoProperty.StartsWith(">"))
                return GetExpression(pe, property, dtoProperty.Replace(">", ""), ExpressionType.GreaterThanOrEqual);
            if (dtoProperty.StartsWith("<"))
                return GetExpression(pe, property, dtoProperty.Replace("<", ""), ExpressionType.LessThanOrEqual);

            return GetExpression(pe, property, dtoProperty, ExpressionType.Equal);
        }

        public static Expression GetExpression(ParameterExpression pe, string propertyName, string dtoPropertyValue,
            ExpressionType type)
        {
            //Expression for accessing Entity Framework property
            var member = Expression.Property(pe, propertyName);
            var propertyType = ((PropertyInfo)member.Member).PropertyType;
            var converter = TypeDescriptor.GetConverter(propertyType);

            if (!converter.CanConvertFrom(typeof(string)))
                throw new NotSupportedException();

            var propertyValue = converter.ConvertFromInvariantString(dtoPropertyValue);
            var constant = Expression.Constant(propertyValue);
            var valueExpression = Expression.Convert(constant, propertyType);

            switch (type)
            {
                case ExpressionType.Equal:
                    return Expression.Equal(member, valueExpression);

                case ExpressionType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(member, valueExpression);

                case ExpressionType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(member, valueExpression);

                case ExpressionType.StartsWith:
                    MethodInfo stringStartsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                    return Expression.Call(member, stringStartsWithMethod, valueExpression);

                case ExpressionType.EndsWith:
                    MethodInfo stringEndsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                    return Expression.Call(member, stringEndsWithMethod, valueExpression);

                case ExpressionType.Contains:
                    MethodInfo stringContainsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    return Expression.Call(member, stringContainsMethod, valueExpression);

                default:
                    return Expression.Equal(member, valueExpression);
            }
        }
    }
}