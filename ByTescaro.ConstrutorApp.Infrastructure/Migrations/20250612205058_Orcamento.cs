using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByTescaro.ConstrutorApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Orcamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Insumo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Orcamento",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<long>(type: "bigint", nullable: false),
                    Responsavel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataReferencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalEstimado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UsuarioCadastro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orcamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orcamento_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrcamentoObra",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<long>(type: "bigint", nullable: false),
                    DataReferencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Responsavel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalEstimado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UsuarioCadastro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrcamentoObra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrcamentoObra_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrcamentoItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrcamentoObraId = table.Column<long>(type: "bigint", nullable: false),
                    InsumoId = table.Column<long>(type: "bigint", nullable: true),
                    ServicoId = table.Column<long>(type: "bigint", nullable: true),
                    FornecedorId = table.Column<long>(type: "bigint", nullable: true),
                    Quantidade = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UsuarioCadastro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrcamentoId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrcamentoItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrcamentoItem_Fornecedor_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrcamentoItem_Insumo_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrcamentoItem_OrcamentoObra_OrcamentoObraId",
                        column: x => x.OrcamentoObraId,
                        principalTable: "OrcamentoObra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrcamentoItem_Orcamento_OrcamentoId",
                        column: x => x.OrcamentoId,
                        principalTable: "Orcamento",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrcamentoItem_Servico_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servico",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orcamento_ObraId",
                table: "Orcamento",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_OrcamentoItem_FornecedorId",
                table: "OrcamentoItem",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrcamentoItem_InsumoId",
                table: "OrcamentoItem",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrcamentoItem_OrcamentoId",
                table: "OrcamentoItem",
                column: "OrcamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrcamentoItem_OrcamentoObraId",
                table: "OrcamentoItem",
                column: "OrcamentoObraId");

            migrationBuilder.CreateIndex(
                name: "IX_OrcamentoItem_ServicoId",
                table: "OrcamentoItem",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrcamentoObra_ObraId",
                table: "OrcamentoObra",
                column: "ObraId");

          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropTable(
                name: "OrcamentoItem");

            migrationBuilder.DropTable(
                name: "OrcamentoObra");

            migrationBuilder.DropTable(
                name: "Orcamento");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Insumo",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

       
        }
    }
}
