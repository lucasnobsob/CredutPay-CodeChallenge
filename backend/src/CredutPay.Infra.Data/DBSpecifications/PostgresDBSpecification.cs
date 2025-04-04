using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CredutPay.Infra.Data.DBSpecifications
{
    public static class PostgresDBSpecification
    {
        public static void ConfigureProperties(ModelBuilder modelBuilder)
        {
            var props = modelBuilder.Model.GetEntityTypes()
                            .SelectMany(e => e.GetProperties())
                            .Where(p => IsEligibleType(p.ClrType));

            foreach (var property in props)
                ConfigureProperty(property);
        }

        private static bool IsEligibleType(Type type) =>
            type == typeof(string) ||
            type == typeof(Guid) ||
            type == typeof(DateTime) ||
            type == typeof(bool);

        private static void ConfigureGuidProperty(IMutableProperty property)
        {
            property.SetColumnType("uuid");
            if (property.IsPrimaryKey())
                property.SetDefaultValueSql("gen_random_uuid()");
        }

        private static void ConfigureDateTimeProperty(IMutableProperty property)
        {
            property.SetColumnType("timestamp with time zone");
            property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                toDb => toDb.Kind == DateTimeKind.Local ? toDb.ToUniversalTime() : toDb,
                fromDb => fromDb
            ));
        }

        private static void ConfigureProperty(IMutableProperty property)
        {
            switch (property.ClrType)
            {
                case Type t when t == typeof(Guid):
                    ConfigureGuidProperty(property);
                    break;
                case Type t when t == typeof(DateTime):
                    ConfigureDateTimeProperty(property);
                    break;
                case Type t when t == typeof(bool):
                    property.SetColumnType("boolean");
                    break;
                default:
                    property.SetColumnType("varchar(255)");
                    break;
            }
        }

    }
}
