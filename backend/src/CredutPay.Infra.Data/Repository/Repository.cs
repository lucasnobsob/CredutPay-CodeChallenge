using CredutPay.Infra.Data.Context;
using CredutPay.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CredutPay.Infra.Data.Repository
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext Db;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(ApplicationDbContext context)
        {
            Db = context;
            DbSet = Db.Set<TEntity>();
        }

        public virtual async Task Add(TEntity obj)
        {
            await DbSet.AddAsync(obj);
        }

        public virtual async Task<TEntity> GetById(Guid id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll(ISpecification<TEntity> spec)
        {
            return await ApplySpecification(spec);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllSoftDeleted()
        {
            return await DbSet.IgnoreQueryFilters()
                .Where(e => EF.Property<bool>(e, "IsDeleted") == true).ToListAsync();
        }

        public virtual void Update(TEntity obj)
        {
            DbSet.Update(obj);
        }

        public virtual async Task Remove(Guid id)
        {
            var entity = await DbSet.FindAsync(id);
            if (entity is null) return;
            DbSet.Remove(entity);
        }

        public async Task<int> SaveChanges()
        {
            return await Db.SaveChangesAsync();
        }

        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }

        private async Task<IEnumerable<TEntity>> ApplySpecification(ISpecification<TEntity> spec)
        {
            return await SpecificationEvaluator<TEntity>.GetQuery(DbSet.AsQueryable(), spec).ToListAsync();
        }
    }
}
