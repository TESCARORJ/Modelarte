using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByTescaro.ConstrutorApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ObraInsumoLista : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObraInsumo_Insumo_InsumoId",
                table: "ObraInsumo");

            migrationBuilder.DropForeignKey(
                name: "FK_ObraInsumo_Obra_ObraId",
                table: "ObraInsumo");

            migrationBuilder.AlterColumn<long>(
                name: "ObraId",
                table: "ObraInsumo",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "ObraInsumoListaId",
                table: "ObraInsumo",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ObraInsumoLista",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<long>(type: "bigint", nullable: false),
                    ResponsavelId = table.Column<long>(type: "bigint", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraInsumoLista", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObraInsumoLista_Funcionario_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "Funcionario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObraInsumoLista_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObraInsumo_ObraInsumoListaId",
                table: "ObraInsumo",
                column: "ObraInsumoListaId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraInsumoLista_ObraId",
                table: "ObraInsumoLista",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_ObraInsumoLista_ResponsavelId",
                table: "ObraInsumoLista",
                column: "ResponsavelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ObraInsumo_Insumo_InsumoId",
                table: "ObraInsumo",
                column: "InsumoId",
                principalTable: "Insumo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ObraInsumo_ObraInsumoLista_ObraInsumoListaId",
                table: "ObraInsumo",
                column: "ObraInsumoListaId",
                principalTable: "ObraInsumoLista",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ObraInsumo_Obra_ObraId",
                table: "ObraInsumo",
                column: "ObraId",
                principalTable: "Obra",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObraInsumo_Insumo_InsumoId",
                table: "ObraInsumo");

            migrationBuilder.DropForeignKey(
                name: "FK_ObraInsumo_ObraInsumoLista_ObraInsumoListaId",
                table: "ObraInsumo");

            migrationBuilder.DropForeignKey(
                name: "FK_ObraInsumo_Obra_ObraId",
                table: "ObraInsumo");

            migrationBuilder.DropTable(
                name: "ObraInsumoLista");

            migrationBuilder.DropIndex(
                name: "IX_ObraInsumo_ObraInsumoListaId",
                table: "ObraInsumo");

            migrationBuilder.DropColumn(
                name: "ObraInsumoListaId",
                table: "ObraInsumo");

            migrationBuilder.AlterColumn<long>(
                name: "ObraId",
                table: "ObraInsumo",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ObraInsumo_Insumo_InsumoId",
                table: "ObraInsumo",
                column: "InsumoId",
                principalTable: "Insumo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ObraInsumo_Obra_ObraId",
                table: "ObraInsumo",
                column: "ObraId",
                principalTable: "Obra",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
