using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DataAccess.Interfaces;

namespace DataAccess.Classes
{
    public static class Extensions
    {
        // ..................................................................................Order By.....................................................................
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, IQueryableOrderBy<T> obj) where T : class
        {
            return obj.OrderBy(source);
        }


        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, IEnumerableOrderBy<T> obj) where T : class
        {
            return obj.OrderBy(source);
        }


        // ..................................................................................Where.....................................................................
        public static IQueryable<T> Where<T>(this IQueryable<T> source, IWhere<T> obj) where T : class
        {
            return obj.SetWhere(source);
        }



        
        // ..................................................................................Select.....................................................................
        public static IQueryable<TOut> Select<T, TOut>(this IQueryable<T> source) where T : class where TOut : class, new()
        {
            IQueryableSelect<T, TOut> obj = (IQueryableSelect<T, TOut>)new TOut();
            return obj.Select(source);
        }


        public static IEnumerable<TOut> Select<T, TOut>(this IEnumerable<T> source, IEnumerableSelect<T, TOut> obj) where T : class where TOut : class
        {
            return obj.Select(source);
        }



        // ...............................................................................Where Any.....................................................................
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
    }
}
