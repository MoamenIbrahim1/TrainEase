using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Domain.Common;
using TrainEase.Core.Domain.Contracts;
using TrainEase.Infrastructure.Presistance._Data;

namespace TrainEase.Infrastructure.Presistance.Repostories
{
    internal class GenericRepository<TEntity, TKey>(AppDbContext _dbContext) : IGenericRepostorie<TEntity, TKey>
        where TEntity : BaseAuditableEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        
        public async Task<IEnumerable<TEntity>> GetAllAsync(bool withTracking = false)
        {
            if (withTracking)
            {
                return await _dbContext.Set<TEntity>().ToListAsync();
            }

            // AsNoTracking بيحسن الأداء لو مش محتاج تعدل على البيانات دي
            return await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public async Task<TEntity?> GetAsync(TKey id)
        {
          
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }
    }
}

