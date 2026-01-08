using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateUsuariosPerfis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_usuarios_perfis_perfil_id",
                table: "usuarios");

            migrationBuilder.AlterColumn<string>(
                name: "senha",
                table: "usuarios",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<Guid>(
                name: "perfil_id",
                table: "usuarios",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "usuarios",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "nome",
                table: "usuarios",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "perfil_id1",
                table: "usuarios",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "nome",
                table: "perfis",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_perfil_id1",
                table: "usuarios",
                column: "perfil_id1");

            migrationBuilder.CreateIndex(
                name: "ix_perfis_nome",
                table: "perfis",
                column: "nome",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_usuarios_perfis_perfil_id",
                table: "usuarios",
                column: "perfil_id",
                principalTable: "perfis",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_usuarios_perfis_perfil_id1",
                table: "usuarios",
                column: "perfil_id1",
                principalTable: "perfis",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_usuarios_perfis_perfil_id",
                table: "usuarios");

            migrationBuilder.DropForeignKey(
                name: "fk_usuarios_perfis_perfil_id1",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "ix_usuarios_email",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "ix_usuarios_perfil_id1",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "ix_perfis_nome",
                table: "perfis");

            migrationBuilder.DropColumn(
                name: "ativo",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "nome",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "perfil_id1",
                table: "usuarios");

            migrationBuilder.AlterColumn<string>(
                name: "senha",
                table: "usuarios",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<Guid>(
                name: "perfil_id",
                table: "usuarios",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "usuarios",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "nome",
                table: "perfis",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AddForeignKey(
                name: "fk_usuarios_perfis_perfil_id",
                table: "usuarios",
                column: "perfil_id",
                principalTable: "perfis",
                principalColumn: "id");
        }
    }
}
