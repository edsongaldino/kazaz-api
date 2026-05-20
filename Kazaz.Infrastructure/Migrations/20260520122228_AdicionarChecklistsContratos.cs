using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarChecklistsContratos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contratos_checklist_entrada",
                columns: table => new
                {
                    contrato_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assinado_em = table.Column<DateOnly>(type: "date", nullable: true),
                    seguro_incendio = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    chaves = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    energia = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    agua = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    gas = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    condominio = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    iptu_garagem = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    iptu = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    vistoria_entrada_em = table.Column<DateOnly>(type: "date", nullable: true),
                    manutencao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    observacoes_finais = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    bonus_locacao = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    data_pagamento_bonus = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contratos_checklist_entrada", x => x.contrato_id);
                    table.ForeignKey(
                        name: "fk_contratos_checklist_entrada_contratos_contrato_id",
                        column: x => x.contrato_id,
                        principalTable: "contratos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contratos_checklist_saida",
                columns: table => new
                {
                    contrato_id = table.Column<Guid>(type: "uuid", nullable: false),
                    motivo_saida = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    aluguel = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    multa_contratual = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    aviso_saida_em = table.Column<DateOnly>(type: "date", nullable: true),
                    chaves = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    aviso_proprietario = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    energia = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    gas = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    agua = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    condominio = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    iptu = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    vistoria_saida_em = table.Column<DateOnly>(type: "date", nullable: true),
                    pintura_manutencao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    reativar_imovel_no_site = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    cancelamento_seguro_fianca = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contratos_checklist_saida", x => x.contrato_id);
                    table.ForeignKey(
                        name: "fk_contratos_checklist_saida_contratos_contrato_id",
                        column: x => x.contrato_id,
                        principalTable: "contratos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contratos_checklist_entrada");

            migrationBuilder.DropTable(
                name: "contratos_checklist_saida");
        }
    }
}
