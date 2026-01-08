using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarUploadDeDocumentosConvites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "convites_cadastro_contrato",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contrato_id = table.Column<Guid>(type: "uuid", nullable: false),
                    papel = table.Column<int>(type: "integer", nullable: false),
                    token = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expira_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    pessoa_id = table.Column<Guid>(type: "uuid", nullable: true),
                    usado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_convites_cadastro_contrato", x => x.id);
                    table.ForeignKey(
                        name: "fk_convites_cadastro_contrato_contratos_contrato_id",
                        column: x => x.contrato_id,
                        principalTable: "contratos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_convites_cadastro_contrato_pessoas_pessoa_id",
                        column: x => x.pessoa_id,
                        principalTable: "pessoas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_convites_cadastro_contrato_contrato_id",
                table: "convites_cadastro_contrato",
                column: "contrato_id");

            migrationBuilder.CreateIndex(
                name: "ix_convites_cadastro_contrato_pessoa_id",
                table: "convites_cadastro_contrato",
                column: "pessoa_id");

            migrationBuilder.CreateIndex(
                name: "ix_convites_cadastro_contrato_token",
                table: "convites_cadastro_contrato",
                column: "token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "convites_cadastro_contrato");
        }
    }
}
