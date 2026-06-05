using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenantChecklistGlobal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "imobiliaria_id",
                table: "checklist_etapas_globais",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_checklist_etapas_globais_imobiliaria_id",
                table: "checklist_etapas_globais",
                column: "imobiliaria_id");

            migrationBuilder.AddForeignKey(
                name: "fk_checklist_etapas_globais_imobiliarias_imobiliaria_id",
                table: "checklist_etapas_globais",
                column: "imobiliaria_id",
                principalTable: "imobiliarias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_checklist_etapas_globais_imobiliarias_imobiliaria_id",
                table: "checklist_etapas_globais");

            migrationBuilder.DropIndex(
                name: "ix_checklist_etapas_globais_imobiliaria_id",
                table: "checklist_etapas_globais");

            migrationBuilder.DropColumn(
                name: "imobiliaria_id",
                table: "checklist_etapas_globais");
        }
    }
}
