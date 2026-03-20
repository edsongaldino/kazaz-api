using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjustarVinculoPF_PJPorPessoaId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_dados_pessoa_fisica_pessoas_id",
                table: "dados_pessoa_fisica");

            migrationBuilder.DropForeignKey(
                name: "fk_dados_pessoa_juridica_pessoas_id",
                table: "dados_pessoa_juridica");

            migrationBuilder.DropForeignKey(
                name: "fk_socios_dados_pessoas_fisicas_pessoa_fisica_id",
                table: "socios");

            migrationBuilder.DropForeignKey(
                name: "fk_socios_dados_pessoas_juridicas_empresa_id",
                table: "socios");

            // ✅ NÃO mexe na PK de pessoas (era isso que quebrava com FKs dependentes)
            // migrationBuilder.DropPrimaryKey(name: "PK_pessoas", table: "pessoas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dados_pessoa_juridica",
                table: "dados_pessoa_juridica");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dados_pessoa_fisica",
                table: "dados_pessoa_fisica");

            // ✅ Nome sai da tabela base pessoas (você decidiu manter nome só em PF)
            migrationBuilder.DropColumn(
                name: "nome",
                table: "pessoas");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "dados_pessoa_juridica",
                newName: "pessoa_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "dados_pessoa_fisica",
                newName: "pessoa_id");

            // ✅ Nome agora fica em PF
            migrationBuilder.AddColumn<string>(
                name: "nome",
                table: "dados_pessoa_fisica",
                type: "text",
                nullable: false,
                defaultValue: "");

            // ✅ NÃO recria PK de pessoas (não precisa)
            // migrationBuilder.AddPrimaryKey(name: "pk_pessoas", table: "pessoas", column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_dados_pessoa_juridica",
                table: "dados_pessoa_juridica",
                column: "pessoa_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_dados_pessoa_fisica",
                table: "dados_pessoa_fisica",
                column: "pessoa_id");

            migrationBuilder.AddForeignKey(
                name: "fk_dados_pessoa_fisica_pessoas_pessoa_id",
                table: "dados_pessoa_fisica",
                column: "pessoa_id",
                principalTable: "pessoas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_dados_pessoa_juridica_pessoas_pessoa_id",
                table: "dados_pessoa_juridica",
                column: "pessoa_id",
                principalTable: "pessoas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_socios_dados_pessoa_fisica_pessoa_fisica_id",
                table: "socios",
                column: "pessoa_fisica_id",
                principalTable: "dados_pessoa_fisica",
                principalColumn: "pessoa_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_socios_dados_pessoa_juridica_empresa_id",
                table: "socios",
                column: "empresa_id",
                principalTable: "dados_pessoa_juridica",
                principalColumn: "pessoa_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_dados_pessoa_fisica_pessoas_pessoa_id",
                table: "dados_pessoa_fisica");

            migrationBuilder.DropForeignKey(
                name: "fk_dados_pessoa_juridica_pessoas_pessoa_id",
                table: "dados_pessoa_juridica");

            migrationBuilder.DropForeignKey(
                name: "fk_socios_dados_pessoa_fisica_pessoa_fisica_id",
                table: "socios");

            migrationBuilder.DropForeignKey(
                name: "fk_socios_dados_pessoa_juridica_empresa_id",
                table: "socios");

            // ✅ NÃO mexe na PK de pessoas
            // migrationBuilder.DropPrimaryKey(name: "pk_pessoas", table: "pessoas");

            migrationBuilder.DropPrimaryKey(
                name: "pk_dados_pessoa_juridica",
                table: "dados_pessoa_juridica");

            migrationBuilder.DropPrimaryKey(
                name: "pk_dados_pessoa_fisica",
                table: "dados_pessoa_fisica");

            migrationBuilder.DropColumn(
                name: "nome",
                table: "dados_pessoa_fisica");

            migrationBuilder.RenameColumn(
                name: "pessoa_id",
                table: "dados_pessoa_juridica",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "pessoa_id",
                table: "dados_pessoa_fisica",
                newName: "id");

            // ✅ Recoloca Nome em pessoas como era (NOT NULL com default)
            migrationBuilder.AddColumn<string>(
                name: "nome",
                table: "pessoas",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            // ✅ NÃO recria PK de pessoas (já existe)
            // migrationBuilder.AddPrimaryKey(name: "PK_pessoas", table: "pessoas", column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dados_pessoa_juridica",
                table: "dados_pessoa_juridica",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dados_pessoa_fisica",
                table: "dados_pessoa_fisica",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_dados_pessoa_fisica_pessoas_id",
                table: "dados_pessoa_fisica",
                column: "id",
                principalTable: "pessoas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_dados_pessoa_juridica_pessoas_id",
                table: "dados_pessoa_juridica",
                column: "id",
                principalTable: "pessoas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_socios_dados_pessoas_fisicas_pessoa_fisica_id",
                table: "socios",
                column: "pessoa_fisica_id",
                principalTable: "dados_pessoa_fisica",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_socios_dados_pessoas_juridicas_empresa_id",
                table: "socios",
                column: "empresa_id",
                principalTable: "dados_pessoa_juridica",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}