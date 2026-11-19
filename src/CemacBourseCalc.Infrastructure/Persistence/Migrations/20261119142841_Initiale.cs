using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CemacBourseCalc.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initiale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "titre",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    isin = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    nom = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    emetteur = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    nominal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    type_titre = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre_titres = table.Column<long>(type: "bigint", nullable: true),
                    taux_facial = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    periodicite = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    date_jouissance = table.Column<DateOnly>(type: "date", nullable: true),
                    date_echeance = table.Column<DateOnly>(type: "date", nullable: true),
                    mode_remboursement = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    differe_echeances = table.Column<int>(type: "integer", nullable: true),
                    code_convention = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_titre", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_titre_isin",
                table: "titre",
                column: "isin",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "titre");
        }
    }
}
