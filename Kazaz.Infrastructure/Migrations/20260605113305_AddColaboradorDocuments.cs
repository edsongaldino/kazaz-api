using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddColaboradorDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "colaboradores_documentos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    colaborador_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    documento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_anexo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_colaboradores_documentos", x => x.id);
                    table.ForeignKey(
                        name: "fk_colaboradores_documentos_colaboradores_colaborador_id",
                        column: x => x.colaborador_id,
                        principalTable: "colaboradores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_colaboradores_documentos_documentos_documento_id",
                        column: x => x.documento_id,
                        principalTable: "documentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_colaboradores_documentos_colaborador_id",
                table: "colaboradores_documentos",
                column: "colaborador_id");

            migrationBuilder.CreateIndex(
                name: "ix_colaboradores_documentos_documento_id",
                table: "colaboradores_documentos",
                column: "documento_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "colaboradores_documentos");
        }
    }
}
