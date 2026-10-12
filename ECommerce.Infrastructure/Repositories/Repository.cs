using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ECommerce.Application.Interfaces;
using ECommerce.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();

        }

        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void  Delete(T entity)
        {

            _dbSet.Remove(entity);
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        private IQueryable<T> Query(Expression<Func<T,bool>>? expression =null,bool tracked = true,Expression<Func<T, object>>[]? includes = null)
        {
            var entities = _dbSet.AsQueryable();
            if (expression != null)
            {
                entities = entities.Where(expression);
            }
            if (!tracked)
            {
                entities = entities.AsNoTracking();
            }
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }
            return entities;
        }
        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? expression = null, bool tracked = true, Expression<Func<T, object>>[]? includes = null)
        {
            
             return await Query(expression,tracked, includes).ToListAsync();
        }

        public async Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null, 
            bool tracked = true,
            Expression<Func<T, object>>[]? includes = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null 
            )
        {
            if(orderBy != null)
            {
                var orderedQuery = orderBy.Compile()(Query(expression, tracked, includes));
                return await orderedQuery.FirstOrDefaultAsync();
            };

            return await Query(expression, tracked, includes).FirstOrDefaultAsync();
        }

      

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
                    Expression<Func<T, bool>>? expression,
                    int page,
                    int pageSize,
                    bool tracked = true,
                    Expression<Func<T, object>>[]? includes = null,
                    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)

        {
            var query = Query(expression, tracked, includes);

            var total = await query.CountAsync();

            IQueryable<T> ordered = orderBy is not null ? orderBy(query) : query;

            var items = await ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
    }
}
