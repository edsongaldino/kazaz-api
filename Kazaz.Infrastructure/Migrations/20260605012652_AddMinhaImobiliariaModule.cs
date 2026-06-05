using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMinhaImobiliariaModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "colaboradores",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    cpf = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cargo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    telefone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    data_admissao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_colaboradores", x => x.id);
                    table.ForeignKey(
                        name: "fk_colaboradores_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "financeiro_lancamentos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    valor = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    data_vencimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_pagamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    categoria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    cliente_id = table.Column<Guid>(type: "uuid", nullable: true),
                    contrato_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_financeiro_lancamentos", x => x.id);
                    table.ForeignKey(
                        name: "fk_financeiro_lancamentos_contratos_contrato_id",
                        column: x => x.contrato_id,
                        principalTable: "contratos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_financeiro_lancamentos_pessoas_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "pessoas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "imobiliarias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    razao_social = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    nome_fantasia = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    cnpj = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    creci = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    data_fundacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    logo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    telefone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    endereco_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_imobiliarias", x => x.id);
                    table.ForeignKey(
                        name: "fk_imobiliarias_enderecos_endereco_id",
                        column: x => x.endereco_id,
                        principalTable: "enderecos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "prestadores_servicos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    especialidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    cpf_cnpj = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    telefone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    observacoes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    endereco_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_prestadores_servicos", x => x.id);
                    table.ForeignKey(
                        name: "fk_prestadores_servicos_enderecos_endereco_id",
                        column: x => x.endereco_id,
                        principalTable: "enderecos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_colaboradores_cpf",
                table: "colaboradores",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_colaboradores_usuario_id",
                table: "colaboradores",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_financeiro_lancamentos_cliente_id",
                table: "financeiro_lancamentos",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "ix_financeiro_lancamentos_contrato_id",
                table: "financeiro_lancamentos",
                column: "contrato_id");

            migrationBuilder.CreateIndex(
                name: "ix_financeiro_lancamentos_data_vencimento",
                table: "financeiro_lancamentos",
                column: "data_vencimento");

            migrationBuilder.CreateIndex(
                name: "ix_financeiro_lancamentos_status",
                table: "financeiro_lancamentos",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_financeiro_lancamentos_tipo",
                table: "financeiro_lancamentos",
                column: "tipo");

            migrationBuilder.CreateIndex(
                name: "ix_imobiliarias_cnpj",
                table: "imobiliarias",
                column: "cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_imobiliarias_endereco_id",
                table: "imobiliarias",
                column: "endereco_id");

            migrationBuilder.CreateIndex(
                name: "ix_prestadores_servicos_endereco_id",
                table: "prestadores_servicos",
                column: "endereco_id");

            migrationBuilder.CreateIndex(
                name: "ix_prestadores_servicos_especialidade",
                table: "prestadores_servicos",
                column: "especialidade");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "colaboradores");

            migrationBuilder.DropTable(
                name: "financeiro_lancamentos");

            migrationBuilder.DropTable(
                name: "imobiliarias");

            migrationBuilder.DropTable(
                name: "prestadores_servicos");
        }
    }
}
