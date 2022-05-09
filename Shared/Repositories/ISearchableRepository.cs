using DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface ISearchableRepository<T> : IRepository<T> where T : class, IItem
    {
        Task<IEnumerable<TOut>> GetCollection<TOut>(string searchWords) where TOut : class, new();

        Task<IEnumerable<TOut>> GetCollection<TOut>(string searchWords, Expression<Func<T, TOut>> select);

        Task<IEnumerable<TOut>> GetCollection<TOut>(Expression<Func<T, bool>> predicate, string searchWords) where TOut : class, new();

        Task<IEnumerable<TOut>> GetCollection<TOut>(Expression<Func<T, bool>> predicate, string searchWords, Expression<Func<T, TOut>> select) where TOut : class, new();
    }
}