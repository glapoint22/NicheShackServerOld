using System;
using System.Linq;
using System.Linq.Expressions;
using Website.Interfaces;

namespace Website.Classes
{
    public static class Extensions
    {
        // ..................................................................................Sort By.....................................................................
        public static IOrderedQueryable<T> SortBy<T>(this IQueryable<T> source, ISort<T> dto) where T : class
        {
            return dto.SetSortOption(source);
        }


        // ..................................................................................Where.....................................................................
        public static IQueryable<T> Where<T>(this IQueryable<T> source, IWhere<T> dto) where T : class
        {
            return dto.SetWhere(source);
        }



        // ..................................................................................Select.....................................................................
        public static IQueryable<TOut> Select<T, TOut>(this IQueryable<T> source, ISelect<T, TOut> dto) where T : class where TOut : class
        {
            return dto.SetSelect(source);
        }



        public static IQueryable<T> WhereAny<T>(this IQueryable<T> queryable, params Expression<Func<T, bool>>[] predicates)
        {
            var parameter = Expression.Parameter(typeof(T));
            return queryable.Where(Expression.Lambda<Func<T, bool>>(predicates.Aggregate<Expression<Func<T, bool>>, Expression>(null,
                (current, predicate) =>
                {
                    var visitor = new ParameterSubstitutionVisitor(predicate.Parameters[0], parameter);
                    return current != null ? Expression.OrElse(current, visitor.Visit(predicate.Body)) : visitor.Visit(predicate.Body);
                }), parameter));
        }

        private class ParameterSubstitutionVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _destination;
            private readonly ParameterExpression _source;

            public ParameterSubstitutionVisitor(ParameterExpression source, ParameterExpression destination)
            {
                _source = source;
                _destination = destination;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return ReferenceEquals(node, _source) ? _destination : base.VisitParameter(node);
            }
        }


    }
}
