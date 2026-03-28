using System.Collections.Concurrent;
using TrainEase.Core.Domain.Common;
using TrainEase.Core.Domain.Contracts;
using TrainEase.Infrastructure.Presistance._Data;
using TrainEase.Infrastructure.Presistance.Repostories;

namespace TrainEase.Infrastructure.Presistance.UnitOfWork
{
    internal class AppUnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly ConcurrentDictionary<string, object> _repositories;

        public AppUnitOfWork(AppDbContext context)
        {
            _context = context;
            _repositories = new();
        }

        public async Task<int> CompeleteAsync() => await _context.SaveChangesAsync();

        public async ValueTask DisposeAsync() => await _context.DisposeAsync();

        public IGenericRepository<TEntity, Tkey> GetRepository<TEntity, Tkey>()
            where TEntity : BaseAuditableEntity<Tkey>
            where Tkey : IEquatable<Tkey>
        {
            var typeName = typeof(TEntity).Name;

            // GetOrAdd بتشوف لو الـ Repository موجود بترجعه، لو مش موجود بتعمل New وتضيفه
            return (IGenericRepository<TEntity, Tkey>)_repositories.GetOrAdd(typeName,
                _ => new GenericRepository<TEntity, Tkey>(_context));
        }


    }
}