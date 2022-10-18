using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using online_store_api.Data;
using online_store_api.Helpers;

#nullable disable

namespace online_store_api.Migrations
{
    public partial class Products : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordSalt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

                var hashResult = PasswordHelper.HashPassword("root");

                migrationBuilder.InsertData(
                    table: "users",
                    columns: new[] { 
                    nameof(User.Id), 
                    nameof(User.Email), 
                    nameof(User.FullName), 
                    nameof(User.PasswordHash),
                    nameof(User.PasswordSalt) , 
                    nameof(User.Role),
                    nameof(User.IsActive)},
                    values: new object[] {
                        Guid.NewGuid().ToString(),
                        "Admin@Admin.com",
                        "Admin",
                        hashResult.PasswordHash,
                        hashResult.PasswordSalt,
                        "Admin",
                        true
                    }
                );
              
        }

         protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
