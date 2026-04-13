using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjustePessoaDocumentoContrato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_pessoas_documentos_unique",
                table: "pessoas_documentos");

            migrationBuilder.AddColumn<Guid>(
                name: "contrato_id",
                table: "pessoas_documentos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "UX_pessoas_documentos_unique",
                table: "pessoas_documentos",
                columns: new[] { "pessoa_id", "contrato_id", "tipo_documento_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_pessoas_documentos_unique",
                table: "pessoas_documentos");

            migrationBuilder.DropColumn(
                name: "contrato_id",
                table: "pessoas_documentos");

            migrationBuilder.CreateIndex(
                name: "UX_pessoas_documentos_unique",
                table: "pessoas_documentos",
                columns: new[] { "pessoa_id", "tipo_documento_id" },
                unique: true);
        }
    }
}
