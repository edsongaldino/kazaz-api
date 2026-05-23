using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEtapasPersonalizadasChecklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "etapas_personalizadas_json",
                table: "contratos_checklist_saida",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "etapas_personalizadas_json",
                table: "contratos_checklist_entrada",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "etapas_personalizadas_json",
                table: "contratos_checklist_saida");

            migrationBuilder.DropColumn(
                name: "etapas_personalizadas_json",
                table: "contratos_checklist_entrada");
        }
    }
}
