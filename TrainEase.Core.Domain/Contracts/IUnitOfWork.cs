using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Contracts
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<TEntity, Tkey> GetRepository<TEntity, Tkey>()
         where TEntity : BaseAuditableEntity<Tkey>
         where Tkey : IEquatable<Tkey>;
        Task<int> CompeleteAsync();
    }
}
