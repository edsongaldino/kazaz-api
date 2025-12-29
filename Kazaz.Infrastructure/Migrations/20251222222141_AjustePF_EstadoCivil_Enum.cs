using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjustePF_EstadoCivil_Enum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "estado_civil",
                table: "dados_pessoa_fisica",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "estado_civil",
                table: "dados_pessoa_fisica",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);
        }
    }
}
