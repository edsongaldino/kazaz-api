using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImovelProprietarioAndContratoGarantia2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "administrado_pelo_proprietario",
                table: "contratos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "forma_garantia",
                table: "contratos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "imovel_proprietarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    imovel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pessoa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    percentual = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_imovel_proprietarios", x => x.id);
                    table.ForeignKey(
                        name: "fk_imovel_proprietarios_imoveis_imovel_id",
                        column: x => x.imovel_id,
                        principalTable: "imoveis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_imovel_proprietarios_pessoas_pessoa_id",
                        column: x => x.pessoa_id,
                        principalTable: "pessoas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_imovel_proprietarios_imovel_id",
                table: "imovel_proprietarios",
                column: "imovel_id");

            migrationBuilder.CreateIndex(
                name: "ix_imovel_proprietarios_pessoa_id",
                table: "imovel_proprietarios",
                column: "pessoa_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "imovel_proprietarios");

            migrationBuilder.DropColumn(
                name: "administrado_pelo_proprietario",
                table: "contratos");

            migrationBuilder.DropColumn(
                name: "forma_garantia",
                table: "contratos");
        }
    }
}
