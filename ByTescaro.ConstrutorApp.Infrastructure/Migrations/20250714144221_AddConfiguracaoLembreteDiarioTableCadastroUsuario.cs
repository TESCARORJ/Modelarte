using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByTescaro.ConstrutorApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguracaoLembreteDiarioTableCadastroUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracoesLembreteDiario_UsuarioCadastroId",
                table: "ConfiguracoesLembreteDiario",
                column: "UsuarioCadastroId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ConfiguracoesLembreteDiario_Usuario_UsuarioCadastroId",
                table: "ConfiguracoesLembreteDiario",
                column: "UsuarioCadastroId",
                principalTable: "Usuario",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfiguracoesLembreteDiario_Usuario_UsuarioCadastroId",
                table: "ConfiguracoesLembreteDiario");

            migrationBuilder.DropIndex(
                name: "IX_ConfiguracoesLembreteDiario_UsuarioCadastroId",
                table: "ConfiguracoesLembreteDiario");
        }
    }
}
