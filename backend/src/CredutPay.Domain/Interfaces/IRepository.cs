namespace CredutPay.Domain.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        Task Add(TEntity obj);
        Task<TEntity> GetById(Guid id);
        Task<IEnumerable<TEntity>> GetAll();
        Task<IEnumerable<TEntity>> GetAll(ISpecification<TEntity> spec);
        Task<IEnumerable<TEntity>> GetAllSoftDeleted();
        void Update(TEntity obj);
        Task Remove(Guid id);
        Task<int> SaveChanges();
    }
}
