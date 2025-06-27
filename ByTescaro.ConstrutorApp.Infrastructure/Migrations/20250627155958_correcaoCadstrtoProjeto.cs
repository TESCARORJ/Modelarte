using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByTescaro.ConstrutorApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class correcaoCadstrtoProjeto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Obra_Funcionario_ResponsavelObraId",
                table: "Obra");

            migrationBuilder.AlterColumn<long>(
                name: "ResponsavelObraId",
                table: "Obra",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Obra_Funcionario_ResponsavelObraId",
                table: "Obra",
                column: "ResponsavelObraId",
                principalTable: "Funcionario",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Obra_Funcionario_ResponsavelObraId",
                table: "Obra");

            migrationBuilder.AlterColumn<long>(
                name: "ResponsavelObraId",
                table: "Obra",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Obra_Funcionario_ResponsavelObraId",
                table: "Obra",
                column: "ResponsavelObraId",
                principalTable: "Funcionario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
