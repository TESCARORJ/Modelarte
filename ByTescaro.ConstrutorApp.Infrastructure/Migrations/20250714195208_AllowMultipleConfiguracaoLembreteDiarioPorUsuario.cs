using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByTescaro.ConstrutorApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AllowMultipleConfiguracaoLembreteDiarioPorUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConfiguracoesLembreteDiario_UsuarioCadastroId",
                table: "ConfiguracoesLembreteDiario");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracoesLembreteDiario_UsuarioCadastroId",
                table: "ConfiguracoesLembreteDiario",
                column: "UsuarioCadastroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConfiguracoesLembreteDiario_UsuarioCadastroId",
                table: "ConfiguracoesLembreteDiario");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracoesLembreteDiario_UsuarioCadastroId",
                table: "ConfiguracoesLembreteDiario",
                column: "UsuarioCadastroId",
                unique: true);
        }
    }
}
