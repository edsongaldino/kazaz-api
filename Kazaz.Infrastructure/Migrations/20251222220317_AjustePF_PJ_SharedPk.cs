using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjustePF_PJ_SharedPk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "inscricao_estadual",
                table: "dados_pessoa_juridica",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "nome_fantasia",
                table: "dados_pessoa_juridica",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "estado_civil",
                table: "dados_pessoa_fisica",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "nacionalidade",
                table: "dados_pessoa_fisica",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "inscricao_estadual",
                table: "dados_pessoa_juridica");

            migrationBuilder.DropColumn(
                name: "nome_fantasia",
                table: "dados_pessoa_juridica");

            migrationBuilder.DropColumn(
                name: "estado_civil",
                table: "dados_pessoa_fisica");

            migrationBuilder.DropColumn(
                name: "nacionalidade",
                table: "dados_pessoa_fisica");
        }
    }
}
