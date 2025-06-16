﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByTescaro.ConstrutorApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ObraInsumoListaCadsastro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataHoraCadastro",
                table: "ObraInsumoLista",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UsuarioCadastro",
                table: "ObraInsumoLista",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataHoraCadastro",
                table: "ObraInsumoLista");

            migrationBuilder.DropColumn(
                name: "UsuarioCadastro",
                table: "ObraInsumoLista");
        }
    }
}
