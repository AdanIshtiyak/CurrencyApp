using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyApp.Entity.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyAndCurrencyDataModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrencyData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    InternalId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Id = table.Column<string>(type: "TEXT", nullable: true),
                    CurrencyDataId = table.Column<int>(type: "INTEGER", nullable: false),
                    NumCode = table.Column<string>(type: "TEXT", nullable: false),
                    CharCode = table.Column<string>(type: "TEXT", nullable: false),
                    Nominal = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<decimal>(type: "TEXT", nullable: false),
                    Previous = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.InternalId);
                    table.ForeignKey(
                        name: "FK_Currencies_CurrencyData_CurrencyDataId",
                        column: x => x.CurrencyDataId,
                        principalTable: "CurrencyData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CurrencyDataId",
                table: "Currencies",
                column: "CurrencyDataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "CurrencyData");
        }
    }
}
