using CredutPay.Domain.Core.Events;
using CredutPay.Infra.Data.DBSpecifications;
using CredutPay.Infra.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace CredutPay.Infra.Data.Context
{
    public class EventStoreSqlContext : DbContext
    {
        public EventStoreSqlContext(DbContextOptions<EventStoreSqlContext> options) : base(options)
        {
        }

        public DbSet<StoredEvent> StoredEvent { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            PostgresDBSpecification.ConfigureProperties(modelBuilder);

            modelBuilder.ApplyConfiguration(new StoredEventMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}
