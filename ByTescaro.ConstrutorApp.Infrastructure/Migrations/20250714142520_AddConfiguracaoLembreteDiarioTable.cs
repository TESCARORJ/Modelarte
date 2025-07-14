using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByTescaro.ConstrutorApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguracaoLembreteDiarioTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracoesLembreteDiario",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoraDoDia = table.Column<TimeSpan>(type: "time", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracoesLembreteDiario", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracoesLembreteDiario");
        }
    }
}
