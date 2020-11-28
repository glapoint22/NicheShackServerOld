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
                .OrderBy(x => x.Name.ToLower().StartsWith(searchWords.ToLower()) ? (x.Name.ToLower() == searchWords.ToLower() ? 0 : 1) :
                    EF.Functions.Like(x.Name, searchWords + " %") ||
                    EF.Functions.Like(x.Name, "% " + searchWords + " %") ||
                    EF.Functions.Like(x.Name, "% " + searchWords)
                    ? 2 : 3)
                .WhereAny(searchWordsArray.Select(w => (Expression<Func<T, bool>>)(x =>
                    EF.Functions.Like(x.Name, w + "[^a-z]%") ||
                    EF.Functions.Like(x.Name, "%[^a-z]" + w + "[^a-z]%") ||
                    EF.Functions.Like(x.Name, "%[^a-z]" + w))).ToArray())
                .Select<T, TOut>()
                .ToListAsync();


            
        }






        public async Task<IEnumerable<TOut>> GetCollection<TOut>(Expression<Func<T, bool>> predicate, string searchWords) where TOut : class, new()
        {
            string[] searchWordsArray = searchWords.Split(' ').Select(x => "%" + x + "%").ToArray();

            return await context.Set<T>()
                .AsNoTracking()
                .OrderBy(x => x.Name.ToLower().StartsWith(searchWords.ToLower()) ? (x.Name.ToLower() == searchWords.ToLower() ? 0 : 1) :
                    EF.Functions.Like(x.Name, searchWords + " %") ||
                    EF.Functions.Like(x.Name, "% " + searchWords + " %") ||
                    EF.Functions.Like(x.Name, "% " + searchWords)
                    ? 2 : 3)
                .Where(predicate)
                .WhereAny(searchWordsArray.Select(w => (Expression<Func<T, bool>>)(x =>
                    EF.Functions.Like(x.Name, w + "[^a-z]%") ||
                    EF.Functions.Like(x.Name, "%[^a-z]" + w + "[^a-z]%") ||
                    EF.Functions.Like(x.Name, "%[^a-z]" + w))).ToArray())
                .Select<T, TOut>()
                .ToListAsync();
        }
    }
}