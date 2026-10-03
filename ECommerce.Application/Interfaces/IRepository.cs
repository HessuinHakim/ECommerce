using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ECommerce.Application.Interfaces
{
    public interface IRepository <T> where T : class
    {
        public  Task CreateAsync(T entity);

        public void Delete(T entity);

        public void Update(T entity);

        public Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? expression = null, bool tracked = true, Expression<Func<T, object>>[]? includes = null);

        public Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null, 
            bool tracked = true,
            Expression<Func<T, object>>[]? includes = null
            , Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null);



        public  Task CommitAsync();

    }
}
