using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByTescaro.ConstrutorApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlteraOnDeleteObraFuncionarioParaCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObraFuncionario_Obra_ObraId",
                table: "ObraFuncionario");

            migrationBuilder.AddForeignKey(
                name: "FK_ObraFuncionario_Obra_ObraId",
                table: "ObraFuncionario",
                column: "ObraId",
                principalTable: "Obra",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObraFuncionario_Obra_ObraId",
                table: "ObraFuncionario");

            migrationBuilder.AddForeignKey(
                name: "FK_ObraFuncionario_Obra_ObraId",
                table: "ObraFuncionario",
                column: "ObraId",
                principalTable: "Obra",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
