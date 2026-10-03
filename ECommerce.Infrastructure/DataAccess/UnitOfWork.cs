using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.Interfaces;

namespace ECommerce.Infrastructure.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<int> CommitAsync(CancellationToken ct = default)
            => _context.SaveChangesAsync(ct);

    }
}
