﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DataAccess.Classes;

namespace DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        // Set the context
        private readonly DbContext context;
        public Repository(DbContext context)
        {
            this.context = context;
        }



        // Get overloads
        public async Task<T> Get(int id)
        {
            return await context.Set<T>()
                .FindAsync(id);
        }

        public async Task<T> Get(string id)
        {
            return await context.Set<T>()
                .FindAsync(id);
        }

        public async Task<T> Get(Expression<Func<T, bool>> predicate)
        {
            return await context.Set<T>()
                .AsNoTracking()
                .Where(predicate)
                .FirstOrDefaultAsync();
        }

        public async Task<TOut> Get<TOut>(Expression<Func<T, bool>> predicate, Expression<Func<T, TOut>> select)
        {
            return await context.Set<T>()
                .AsNoTracking()
                .Where(predicate)
                .Select(select)
                .FirstOrDefaultAsync();
        }

        public async Task<TOut> Get<TOut>(Expression<Func<T, bool>> predicate) where TOut : class, new()
        {
            return await context.Set<T>()
                .AsNoTracking()
                .Where(predicate)
                .Select<T, TOut>()
                .FirstOrDefaultAsync();
        }





        // GetCollection overloads
        public async Task<IEnumerable<T>> GetCollection()
        {
            return await context.Set<T>().ToListAsync();

        }



        public async Task<IEnumerable<T>> GetCollection(Expression<Func<T, bool>> predicate)
        {
            return await context.Set<T>()
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync();
        }





        public async Task<IEnumerable<TOut>> GetCollection<TOut>(Expression<Func<T, bool>> predicate, Expression<Func<T, TOut>> select)
        {
            return await context.Set<T>()
                .AsNoTracking()
                .Where(predicate)
                .Select(select)
                .ToListAsync();
        }


        public async Task<IEnumerable<TOut>> GetCollection<TOut>(Expression<Func<T, TOut>> select)
        {
            return await context.Set<T>()
                .AsNoTracking()
                .Select(select)
                .ToListAsync();
        }



        public async Task<IEnumerable<TOut>> GetCollection<TOut>() where TOut : class, new()
        {
            return await context.Set<T>()
                .AsNoTracking()
                .Select<T, TOut>()
                .ToListAsync();
        }





        public async Task<IEnumerable<TOut>> GetCollection<TOut>(Expression<Func<T, bool>> predicate) where TOut : class, new()
        {
            return await context.Set<T>()
                .AsNoTracking()
                .Where(predicate)
                .Select<T, TOut>()
                .ToListAsync();
        }







        public async Task<IEnumerable<TOut>> GetCollection<TOut>(Expression<Func<T, int>> keySelector, Expression<Func<T, bool>> predicate, Expression<Func<T, TOut>> select)
        {
            return await context.Set<T>()
                .AsNoTracking()
                .OrderBy(keySelector)
                .Where(predicate)
                .Select(select)
                .ToListAsync();
        }




        // Count
        public async Task<int> GetCount(Expression<Func<T, bool>> predicate)
        {
            return await context.Set<T>()
                .AsNoTracking()
                .CountAsync(predicate);
        }







        // Add
        public void Add(T entity)
        {
            context.Set<T>().Add(entity);
        }



        // Update
        public void Update(T entity)
        {
            context.Set<T>().Update(entity);
        }



        // Remove
        public void Remove(T entity)
        {
            context.Set<T>().Remove(entity);
        }


        // Remove Range
        public void RemoveRange(IEnumerable<T> entities)
        {
            context.Set<T>().RemoveRange(entities);
        }



        // Add Range
        public void AddRange(IEnumerable<T> entities)
        {
            context.Set<T>().AddRange(entities);
        }



        // Update Range
        public void UpdateRange(IEnumerable<T> entities)
        {
            context.Set<T>().UpdateRange(entities);
        }



        // Any
        public async Task<bool> Any(Expression<Func<T, bool>> predicate)
        {
            return await context.Set<T>().AnyAsync(predicate);
        }
    }
}