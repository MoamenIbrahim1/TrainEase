using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Contracts
{
    public interface IGenericRepostorie<TEntity,TKey> 
        where TEntity: BaseAuditableEntity<TKey>
        where TKey : IEnumerable<TKey>
    {
      Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(int id);



    }
}
