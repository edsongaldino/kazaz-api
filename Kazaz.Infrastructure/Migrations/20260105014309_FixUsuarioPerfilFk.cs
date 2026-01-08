using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixUsuarioPerfilFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_usuarios_perfis_perfil_id1",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "ix_usuarios_perfil_id1",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "perfil_id1",
                table: "usuarios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "perfil_id1",
                table: "usuarios",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_perfil_id1",
                table: "usuarios",
                column: "perfil_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_usuarios_perfis_perfil_id1",
                table: "usuarios",
                column: "perfil_id1",
                principalTable: "perfis",
                principalColumn: "id");
        }
    }
}
