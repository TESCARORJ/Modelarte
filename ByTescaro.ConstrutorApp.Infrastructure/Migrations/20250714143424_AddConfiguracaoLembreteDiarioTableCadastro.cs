using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByTescaro.ConstrutorApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguracaoLembreteDiarioTableCadastro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataHoraCadastro",
                table: "ConfiguracoesLembreteDiario",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "UsuarioCadastroId",
                table: "ConfiguracoesLembreteDiario",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataHoraCadastro",
                table: "ConfiguracoesLembreteDiario");

            migrationBuilder.DropColumn(
                name: "UsuarioCadastroId",
                table: "ConfiguracoesLembreteDiario");
        }
    }
}
