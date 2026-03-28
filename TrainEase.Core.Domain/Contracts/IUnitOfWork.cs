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
