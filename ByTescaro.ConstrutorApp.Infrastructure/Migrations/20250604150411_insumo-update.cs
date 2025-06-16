using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByTescaro.ConstrutorApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class insumoupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ControlaEstoque",
                table: "Insumo");

            migrationBuilder.DropColumn(
                name: "ControlaValidade",
                table: "Insumo");

            migrationBuilder.DropColumn(
                name: "CustoUnitario",
                table: "Insumo");

            migrationBuilder.DropColumn(
                name: "DataValidade",
                table: "Insumo");

            migrationBuilder.DropColumn(
                name: "EstoqueMaximo",
                table: "Insumo");

            migrationBuilder.DropColumn(
                name: "EstoqueMinimo",
                table: "Insumo");

            migrationBuilder.DropColumn(
                name: "Marca",
                table: "Insumo");

            migrationBuilder.DropColumn(
                name: "Modelo",
                table: "Insumo");

            migrationBuilder.DropColumn(
                name: "PrecoVenda",
                table: "Insumo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ControlaEstoque",
                table: "Insumo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ControlaValidade",
                table: "Insumo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "CustoUnitario",
                table: "Insumo",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataValidade",
                table: "Insumo",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstoqueMaximo",
                table: "Insumo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstoqueMinimo",
                table: "Insumo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Marca",
                table: "Insumo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Modelo",
                table: "Insumo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecoVenda",
                table: "Insumo",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }
    }
}
