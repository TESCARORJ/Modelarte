using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByTescaro.ConstrutorApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddObraItemEtapaInsumo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObraItemEtapaPadraoInsumos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraItemEtapaPadraoId = table.Column<long>(type: "bigint", nullable: false),
                    InsumoId = table.Column<long>(type: "bigint", nullable: false),
                    QuantidadeSugerida = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DataHoraCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioCadastro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraItemEtapaPadraoInsumos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraItemEtapaPadraoInsumos_Insumo_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObraItemEtapaPadraoInsumos_ObraItemEtapaPadrao_ObraItemEtapaPadraoId",
                        column: x => x.ObraItemEtapaPadraoId,
                        principalTable: "ObraItemEtapaPadrao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObraItemEtapaPadraoInsumos_InsumoId",
                table: "ObraItemEtapaPadraoInsumos",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraItemEtapaPadraoInsumos_ObraItemEtapaPadraoId",
                table: "ObraItemEtapaPadraoInsumos",
                column: "ObraItemEtapaPadraoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObraItemEtapaPadraoInsumos");
        }
    }
}
