using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MottuGestor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePatioEndereco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ENDERECO",
                table: "Patio",
                newName: "Endereco");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Endereco",
                table: "Patio",
                newName: "ENDERECO");
        }
    }
}
