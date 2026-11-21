using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CemacBourseCalc.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IndexCoursDateSeance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_cours_date_seance",
                table: "cours",
                column: "date_seance");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_cours_date_seance",
                table: "cours");
        }
    }
}
