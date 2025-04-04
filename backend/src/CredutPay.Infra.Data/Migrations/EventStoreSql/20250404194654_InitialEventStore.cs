using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CredutPay.Infra.Data.Migrations.EventStoreSql
{
    /// <inheritdoc />
    public partial class InitialEventStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoredEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Data = table.Column<string>(type: "varchar(255)", nullable: false),
                    User = table.Column<string>(type: "varchar(255)", nullable: false),
                    Action = table.Column<string>(type: "varchar(100)", nullable: false),
                    AggregateId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredEvent", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoredEvent");
        }
    }
}
