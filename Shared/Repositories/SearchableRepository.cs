using DataAccess.Classes;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class SearchableRepository<T> : Repository<T>, ISearchableRepository<T> where T : class, IItem
    {
        private readonly DbContext context;

        public SearchableRepository(DbContext context) : base(context)
        {
            this.context = context;
        }


        public async Task<IEnumerable<TOut>> GetCollection<TOut>(string searchWords) where TOut : class, new()
        {
            string[] searchWordsArray = searchWords.Split(' ').Select(x => "%" + x + "%").ToArray();

            return await context.Set<T>()
                .AsNoTracking()
                .WhereAny(searchWordsArray.Select(w => (Expression<Func<T, bool>>)(x => EF.Functions.Like(x.Name, w))).ToArray())
                .ExtensionSelect<T, TOut>()
                .ToListAsync();
        }






        public async Task<IEnumerable<TOut>> GetCollection<TOut>(Expression<Func<T, bool>> predicate, string searchWords) where TOut : class, new()
        {
            string[] searchWordsArray = searchWords.Split(' ').Select(x => "%" + x + "%").ToArray();

            return await context.Set<T>()
                .AsNoTracking()
                .Where(predicate)
                .WhereAny(searchWordsArray.Select(w => (Expression<Func<T, bool>>)(x => EF.Functions.Like(x.Name, w))).ToArray())
                .ExtensionSelect<T, TOut>()
                .ToListAsync();
        }
    }
}