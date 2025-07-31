using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByTescaro.ConstrutorApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Personalizacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Personalizacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    NomeEmpresa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LogotipoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FaviconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EnderecoEmpresa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TelefoneEmpresa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EmailEmpresa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TextoBoasVindas = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TextoFooter = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImagemFundoLoginUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CorHeader = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CorSecundaria = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CorDestaque = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UsuarioCadastroId = table.Column<long>(type: "bigint", nullable: true),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personalizacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personalizacao_Usuario_UsuarioCadastroId",
                        column: x => x.UsuarioCadastroId,
                        principalTable: "Usuario",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Personalizacao_UsuarioCadastroId",
                table: "Personalizacao",
                column: "UsuarioCadastroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Personalizacao");
        }
    }
}
