using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Contracts
{
    public interface IGenericRepository<TEntity, TKey>
        where TEntity : BaseAuditableEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        public IQueryable<TEntity> GetQueryable();
        Task<IEnumerable<TEntity>> GetAllAsync(bool withTracking = false);
        Task<TEntity?> GetAsync(TKey id);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
