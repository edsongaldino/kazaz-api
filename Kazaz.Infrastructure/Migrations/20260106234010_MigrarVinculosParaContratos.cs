using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrarVinculosParaContratos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vinculos_pessoa_imovel");

            migrationBuilder.DropTable(
                name: "perfis_vinculo_imovel");

            migrationBuilder.CreateTable(
                name: "contratos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    imovel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    inicio_vigencia = table.Column<DateOnly>(type: "date", nullable: false),
                    fim_vigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    numero = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contratos", x => x.id);
                    table.ForeignKey(
                        name: "fk_contratos_imoveis_imovel_id",
                        column: x => x.imovel_id,
                        principalTable: "imoveis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contrato_partes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contrato_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pessoa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    papel = table.Column<int>(type: "integer", nullable: false),
                    percentual = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contrato_partes", x => x.id);
                    table.ForeignKey(
                        name: "fk_contrato_partes_contratos_contrato_id",
                        column: x => x.contrato_id,
                        principalTable: "contratos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_contrato_partes_pessoas_pessoa_id",
                        column: x => x.pessoa_id,
                        principalTable: "pessoas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_contrato_partes_contrato_id_pessoa_id_papel",
                table: "contrato_partes",
                columns: new[] { "contrato_id", "pessoa_id", "papel" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_contrato_partes_pessoa_id",
                table: "contrato_partes",
                column: "pessoa_id");

            migrationBuilder.CreateIndex(
                name: "ix_contratos_imovel_id",
                table: "contratos",
                column: "imovel_id");

            migrationBuilder.CreateIndex(
                name: "ix_contratos_numero",
                table: "contratos",
                column: "numero",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contrato_partes");

            migrationBuilder.DropTable(
                name: "contratos");

            migrationBuilder.CreateTable(
                name: "perfis_vinculo_imovel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_perfis_vinculo_imovel", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vinculos_pessoa_imovel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    imovel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    perfil_vinculo_imovel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pessoa_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vinculos_pessoa_imovel", x => x.id);
                    table.ForeignKey(
                        name: "fk_vinculos_pessoa_imovel_imoveis_imovel_id",
                        column: x => x.imovel_id,
                        principalTable: "imoveis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_vinculos_pessoa_imovel_perfis_vinculo_imovel_perfil_vinculo",
                        column: x => x.perfil_vinculo_imovel_id,
                        principalTable: "perfis_vinculo_imovel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_vinculos_pessoa_imovel_pessoas_pessoa_id",
                        column: x => x.pessoa_id,
                        principalTable: "pessoas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_perfis_vinculo_imovel_nome",
                table: "perfis_vinculo_imovel",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_vinculos_imovel_id",
                table: "vinculos_pessoa_imovel",
                column: "imovel_id");

            migrationBuilder.CreateIndex(
                name: "ix_vinculos_perfil_id",
                table: "vinculos_pessoa_imovel",
                column: "perfil_vinculo_imovel_id");

            migrationBuilder.CreateIndex(
                name: "ix_vinculos_pessoa_id",
                table: "vinculos_pessoa_imovel",
                column: "pessoa_id");

            migrationBuilder.CreateIndex(
                name: "ux_vinculos_pessoa_imovel_unico",
                table: "vinculos_pessoa_imovel",
                columns: new[] { "pessoa_id", "imovel_id", "perfil_vinculo_imovel_id" },
                unique: true);
        }
    }
}
