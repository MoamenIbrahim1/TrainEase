using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Domain.Common;
using TrainEase.Core.Domain.Contracts;

public interface IUnitOfWork : IAsyncDisposable
{
    IGenericRepostorie<TEntity, Tkey> GetRepository<TEntity, Tkey>()
     where TEntity : BaseAuditableEntity<Tkey>
     where Tkey : IEquatable<Tkey>;
    Task<int> CompeleteAsync();
}