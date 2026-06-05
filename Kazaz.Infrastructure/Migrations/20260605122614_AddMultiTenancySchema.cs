using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenancySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "imobiliaria_id",
                table: "usuarios",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "imobiliaria_id",
                table: "prestadores_servicos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "imobiliaria_id",
                table: "pessoas",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "imobiliaria_id",
                table: "leads",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "imobiliaria_id",
                table: "imoveis",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "imobiliaria_id",
                table: "financeiro_lancamentos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "imobiliaria_id",
                table: "convites_cadastro_contrato",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "imobiliaria_id",
                table: "contratos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "imobiliaria_id",
                table: "colaboradores",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_imobiliaria_id",
                table: "usuarios",
                column: "imobiliaria_id");

            migrationBuilder.CreateIndex(
                name: "ix_prestadores_servicos_imobiliaria_id",
                table: "prestadores_servicos",
                column: "imobiliaria_id");

            migrationBuilder.CreateIndex(
                name: "ix_pessoas_imobiliaria_id",
                table: "pessoas",
                column: "imobiliaria_id");

            migrationBuilder.CreateIndex(
                name: "ix_leads_imobiliaria_id",
                table: "leads",
                column: "imobiliaria_id");

            migrationBuilder.CreateIndex(
                name: "ix_imoveis_imobiliaria_id",
                table: "imoveis",
                column: "imobiliaria_id");

            migrationBuilder.CreateIndex(
                name: "ix_financeiro_lancamentos_imobiliaria_id",
                table: "financeiro_lancamentos",
                column: "imobiliaria_id");

            migrationBuilder.CreateIndex(
                name: "ix_convites_cadastro_contrato_imobiliaria_id",
                table: "convites_cadastro_contrato",
                column: "imobiliaria_id");

            migrationBuilder.CreateIndex(
                name: "ix_contratos_imobiliaria_id",
                table: "contratos",
                column: "imobiliaria_id");

            migrationBuilder.CreateIndex(
                name: "ix_colaboradores_imobiliaria_id",
                table: "colaboradores",
                column: "imobiliaria_id");

            migrationBuilder.AddForeignKey(
                name: "fk_colaboradores_imobiliarias_imobiliaria_id",
                table: "colaboradores",
                column: "imobiliaria_id",
                principalTable: "imobiliarias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_contratos_imobiliarias_imobiliaria_id",
                table: "contratos",
                column: "imobiliaria_id",
                principalTable: "imobiliarias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_convites_cadastro_contrato_imobiliarias_imobiliaria_id",
                table: "convites_cadastro_contrato",
                column: "imobiliaria_id",
                principalTable: "imobiliarias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_financeiro_lancamentos_imobiliarias_imobiliaria_id",
                table: "financeiro_lancamentos",
                column: "imobiliaria_id",
                principalTable: "imobiliarias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_imoveis_imobiliarias_imobiliaria_id",
                table: "imoveis",
                column: "imobiliaria_id",
                principalTable: "imobiliarias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_leads_imobiliarias_imobiliaria_id",
                table: "leads",
                column: "imobiliaria_id",
                principalTable: "imobiliarias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pessoas_imobiliarias_imobiliaria_id",
                table: "pessoas",
                column: "imobiliaria_id",
                principalTable: "imobiliarias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_prestadores_servicos_imobiliarias_imobiliaria_id",
                table: "prestadores_servicos",
                column: "imobiliaria_id",
                principalTable: "imobiliarias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_usuarios_imobiliarias_imobiliaria_id",
                table: "usuarios",
                column: "imobiliaria_id",
                principalTable: "imobiliarias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_colaboradores_imobiliarias_imobiliaria_id",
                table: "colaboradores");

            migrationBuilder.DropForeignKey(
                name: "fk_contratos_imobiliarias_imobiliaria_id",
                table: "contratos");

            migrationBuilder.DropForeignKey(
                name: "fk_convites_cadastro_contrato_imobiliarias_imobiliaria_id",
                table: "convites_cadastro_contrato");

            migrationBuilder.DropForeignKey(
                name: "fk_financeiro_lancamentos_imobiliarias_imobiliaria_id",
                table: "financeiro_lancamentos");

            migrationBuilder.DropForeignKey(
                name: "fk_imoveis_imobiliarias_imobiliaria_id",
                table: "imoveis");

            migrationBuilder.DropForeignKey(
                name: "fk_leads_imobiliarias_imobiliaria_id",
                table: "leads");

            migrationBuilder.DropForeignKey(
                name: "fk_pessoas_imobiliarias_imobiliaria_id",
                table: "pessoas");

            migrationBuilder.DropForeignKey(
                name: "fk_prestadores_servicos_imobiliarias_imobiliaria_id",
                table: "prestadores_servicos");

            migrationBuilder.DropForeignKey(
                name: "fk_usuarios_imobiliarias_imobiliaria_id",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "ix_usuarios_imobiliaria_id",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "ix_prestadores_servicos_imobiliaria_id",
                table: "prestadores_servicos");

            migrationBuilder.DropIndex(
                name: "ix_pessoas_imobiliaria_id",
                table: "pessoas");

            migrationBuilder.DropIndex(
                name: "ix_leads_imobiliaria_id",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "ix_imoveis_imobiliaria_id",
                table: "imoveis");

            migrationBuilder.DropIndex(
                name: "ix_financeiro_lancamentos_imobiliaria_id",
                table: "financeiro_lancamentos");

            migrationBuilder.DropIndex(
                name: "ix_convites_cadastro_contrato_imobiliaria_id",
                table: "convites_cadastro_contrato");

            migrationBuilder.DropIndex(
                name: "ix_contratos_imobiliaria_id",
                table: "contratos");

            migrationBuilder.DropIndex(
                name: "ix_colaboradores_imobiliaria_id",
                table: "colaboradores");

            migrationBuilder.DropColumn(
                name: "imobiliaria_id",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "imobiliaria_id",
                table: "prestadores_servicos");

            migrationBuilder.DropColumn(
                name: "imobiliaria_id",
                table: "pessoas");

            migrationBuilder.DropColumn(
                name: "imobiliaria_id",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "imobiliaria_id",
                table: "imoveis");

            migrationBuilder.DropColumn(
                name: "imobiliaria_id",
                table: "financeiro_lancamentos");

            migrationBuilder.DropColumn(
                name: "imobiliaria_id",
                table: "convites_cadastro_contrato");

            migrationBuilder.DropColumn(
                name: "imobiliaria_id",
                table: "contratos");

            migrationBuilder.DropColumn(
                name: "imobiliaria_id",
                table: "colaboradores");
        }
    }
}
