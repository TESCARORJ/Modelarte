using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByTescaro.ConstrutorApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class servico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObraServicoLista",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<long>(type: "bigint", nullable: false),
                    ResponsavelId = table.Column<long>(type: "bigint", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioCadastro = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraServicoLista", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraServicoLista_Funcionario_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "Funcionario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObraServicoLista_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Servico",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioCadastro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servico", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FornecedorServico",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FornecedorId = table.Column<long>(type: "bigint", nullable: false),
                    ServicoId = table.Column<long>(type: "bigint", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrazoEntregaDias = table.Column<int>(type: "int", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioCadastro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FornecedorServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FornecedorServico_Fornecedor_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FornecedorServico_Servico_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ObraServico",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraServicoListaId = table.Column<long>(type: "bigint", nullable: false),
                    ServicoId = table.Column<long>(type: "bigint", nullable: false),
                    Quantidade = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioCadastro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ObraId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraServico_ObraServicoLista_ObraServicoListaId",
                        column: x => x.ObraServicoListaId,
                        principalTable: "ObraServicoLista",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObraServico_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ObraServico_Servico_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FornecedorServico_FornecedorId",
                table: "FornecedorServico",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_FornecedorServico_ServicoId",
                table: "FornecedorServico",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraServico_ObraId",
                table: "ObraServico",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraServico_ObraServicoListaId",
                table: "ObraServico",
                column: "ObraServicoListaId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraServico_ServicoId",
                table: "ObraServico",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraServicoLista_ObraId",
                table: "ObraServicoLista",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraServicoLista_ResponsavelId",
                table: "ObraServicoLista",
                column: "ResponsavelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FornecedorServico");

            migrationBuilder.DropTable(
                name: "ObraServico");

            migrationBuilder.DropTable(
                name: "ObraServicoLista");

            migrationBuilder.DropTable(
                name: "Servico");
        }
    }
}
