using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Domain.Common;
using TrainEase.Core.Domain.Contracts;
using TrainEase.Infrastructure.Presistance._Data;
using TrainEase.Infrastructure.Presistance.Repostories; 

namespace TrainEase.Infrastructure.Presistance.UnitOfWork
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly ConcurrentDictionary<string, object> _repositories;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            _repositories = new();
        }

        public async Task<int> CompeleteAsync() => await _context.SaveChangesAsync();

        public async ValueTask DisposeAsync() => await _context.DisposeAsync();

        public IGenericRepostorie<TEntity, Tkey> GetRepository<TEntity, Tkey>()
            where TEntity : BaseAuditableEntity<Tkey>
            where Tkey : IEquatable<Tkey>
        {
            var typeName = typeof(TEntity).Name;

            // GetOrAdd بتشوف لو الـ Repository موجود بترجعه، لو مش موجود بتعمل New وتضيفه
            return (IGenericRepostorie<TEntity, Tkey>)_repositories.GetOrAdd(typeName,
                _ => new GenericRepository<TEntity, Tkey>(_context));
        }
    }
}