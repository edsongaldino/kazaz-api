using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjustesAnaliseConvites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "preenchido_em",
                table: "convites_cadastro_contrato",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "analise_convite",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    convite_id = table.Column<Guid>(type: "uuid", nullable: false),
                    resultado = table.Column<int>(type: "integer", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    comentario = table.Column<string>(type: "text", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_analise_convite", x => x.id);
                    table.ForeignKey(
                        name: "fk_analise_convite_convite_cadastro_contrato_convite_id",
                        column: x => x.convite_id,
                        principalTable: "convites_cadastro_contrato",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_analise_convite_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_analise_convite_convite_id",
                table: "analise_convite",
                column: "convite_id");

            migrationBuilder.CreateIndex(
                name: "ix_analise_convite_usuario_id",
                table: "analise_convite",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "analise_convite");

            migrationBuilder.DropColumn(
                name: "preenchido_em",
                table: "convites_cadastro_contrato");
        }
    }
}
