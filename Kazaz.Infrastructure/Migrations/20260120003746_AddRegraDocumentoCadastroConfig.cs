using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRegraDocumentoCadastroConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "regras_documento_cadastro",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_pessoa = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    tipo_contrato = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    papel_contrato = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    tipo_documento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    obrigatorio = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ordem = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    multiplicidade = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    rotulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_regras_documento_cadastro", x => x.id);
                    table.CheckConstraint("CK_regras_documento_cadastro_multiplicidade", "multiplicidade >= 1");
                    table.ForeignKey(
                        name: "fk_regras_documento_cadastro_tipos_documento_tipo_documento_id",
                        column: x => x.tipo_documento_id,
                        principalTable: "tipos_documento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_regras_documento_cadastro_tipo_documento_id",
                table: "regras_documento_cadastro",
                column: "tipo_documento_id");

            migrationBuilder.CreateIndex(
                name: "ix_regras_documento_cadastro_tipo_documento_id_tipo_pessoa_tip",
                table: "regras_documento_cadastro",
                columns: new[] { "tipo_documento_id", "tipo_pessoa", "tipo_contrato", "papel_contrato", "rotulo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_regras_documento_cadastro_tipo_pessoa_tipo_contrato_papel_c",
                table: "regras_documento_cadastro",
                columns: new[] { "tipo_pessoa", "tipo_contrato", "papel_contrato", "ativo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "regras_documento_cadastro");
        }
    }
}
