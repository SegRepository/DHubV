using DHubV.Core.Dtos.BaseFilter;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DHubV.Application.Helper
{
    public static class FilterHelper
    {
        public static Expression<Func<TModel, bool>> GetConditions<TModel, TFilter>(
            TFilter searchCriteria,
            params string[] searchableFields
        )
        {
            var parameter = Expression.Parameter(typeof(TModel), "model");
            Expression predicate = Expression.Constant(true);

            var searchKeywordProperty = typeof(TFilter).GetProperty("SearchKeyword");
            var searchKeyword = searchKeywordProperty?.GetValue(searchCriteria) as string;

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                Expression keywordPredicate = Expression.Constant(false);
                foreach (var field in searchableFields)
                {
                    var modelProperty = typeof(TModel).GetProperty(field);
                    if (modelProperty != null && modelProperty.PropertyType == typeof(string))
                    {
                        var propertyExpression = Expression.Property(parameter, modelProperty);
                        var containsMethod = typeof(string).GetMethod(
                            "Contains",
                            new[] { typeof(string) }
                        );
                        var containsExpression = Expression.Call(
                            propertyExpression,
                            containsMethod,
                            Expression.Constant(searchKeyword)
                        );
                        keywordPredicate = Expression.OrElse(keywordPredicate, containsExpression);
                    }
                }
                predicate = Expression.AndAlso(predicate, keywordPredicate);
            }

            foreach (var property in typeof(TFilter).GetProperties())
            {
                if (property.Name == "SearchKeyword")
                    continue;

                var value = property.GetValue(searchCriteria);
                if (IsDefaultValue(value) && (property.Name != "Id" || value == null))
                    continue;

                var modelProperty = typeof(TModel).GetProperty(property.Name);
                if (modelProperty == null)
                    continue;

                var left = Expression.Property(parameter, modelProperty);
                var right = Expression.Constant(value);

                Expression condition;
                if (property.PropertyType == typeof(string))
                {
                    var containsMethod = typeof(string).GetMethod(
                        "Contains",
                        new[] { typeof(string) }
                    );
                    condition = Expression.Call(left, containsMethod, right);
                }
                else
                {
                    // Check if the model property type is nullable
                    if (Nullable.GetUnderlyingType(modelProperty.PropertyType) != null)
                    {
                        // Convert the constant value to the nullable type
                        var converted = Expression.Convert(right, modelProperty.PropertyType);
                        condition = Expression.Equal(left, converted);
                    }
                    else
                    {
                        condition = Expression.Equal(left, right);
                    }
                }

                predicate = Expression.AndAlso(predicate, condition);
            }

            return Expression.Lambda<Func<TModel, bool>>(predicate, parameter);
        }

        private static bool IsDefaultValue(object value)
        {
            if (value == null)
                return true;
            if (
                value is string str
                && (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
            )
            {
                return true;
            }
            else if (value is string || typeof(string) == value.GetType())
            {
                return false;
            }
            else if (value is bool && value != null)
            {
                return false;
            }
            else if (value is Enum && value != null)
            {
                return false;
            }
            if (Equals(value, Activator.CreateInstance(value.GetType())))
                return true;

            return false;
        }

        public static T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> a,
            Expression<Func<T, bool>> b
        )
        {
            ParameterExpression p = a.Parameters[0];

            SubstExpressionVisitor visitor = new();
            visitor.subst[b.Parameters[0]] = p;

            Expression body = Expression.AndAlso(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> a,
            Expression<Func<T, bool>> b
        )
        {
            ParameterExpression p = a.Parameters[0];

            SubstExpressionVisitor visitor = new();
            visitor.subst[b.Parameters[0]] = p;

            Expression body = Expression.OrElse(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        public static IQueryable<TModel> ApplySorting<TModel>(
            this IQueryable<TModel> query,
            BaseFilterDTO<int> searchCriteria
        )
        {
            if (
                !string.IsNullOrEmpty(searchCriteria.SortField)
                && !string.IsNullOrEmpty(searchCriteria.SortDirection)
            )
            {
                var sortingExpression = GetSortingExpression<TModel>(searchCriteria.SortField);
                query =
                    searchCriteria.SortDirection.ToLower() == "asc"
                        ? query.OrderBy(sortingExpression)
                        : query.OrderByDescending(sortingExpression);
            }
            else
            {
                query = query.OrderByDescending(c => EF.Property<DateTime>(c, "CreatedAt"));
            }

            return query;
        }

        public static IQueryable<TModel> ApplyPagination<TModel>(
            this IQueryable<TModel> query,
            int? pageIndex,
            int? pageSize
        )
        {
            pageIndex ??= 1;
            pageSize ??= 10;

            var offset = (pageIndex.Value - 1) * pageSize.Value;
            query = query.Skip(offset).Take(pageSize.Value);

            return query;
        }

        public static T? DeepClone<T>(this T? obj)
        {
            var json = JsonSerializer.Serialize(obj);
            return JsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Checks if string is null, empty or whitespace
        /// </summary>
        /// <param name="str">string to check</param>
        /// <returns>true if input string is null, empty or whitespace, false otherwise</returns>
        public static bool IsNull(this string? str)
        {
            return (str is null || string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str));
        }

        public static Expression<Func<TModel, object>> GetSortingExpression<TModel>(
            string sortField
        )
        {
            var parameter = Expression.Parameter(typeof(TModel), "model");
            var property = Expression.Property(parameter, sortField);
            var converted = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<TModel, object>>(converted, parameter);
        }
    }

    internal class SubstExpressionVisitor : ExpressionVisitor
    {
        public Dictionary<Expression, Expression> subst = new();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (subst.TryGetValue(node, out var newValue))
            {
                return newValue;
            }
            return node;
        }
    }
}
