using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImoveisEstruturaCompleta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_vinculos_pessoa_imovel_pessoas_pessoa_id",
                table: "vinculos_pessoa_imovel");

            migrationBuilder.DropPrimaryKey(
                name: "pk_vinculos_pessoa_imovel",
                table: "vinculos_pessoa_imovel");

            migrationBuilder.RenameIndex(
                name: "ix_vinculos_pessoa_imovel_perfil_vinculo_imovel_id",
                table: "vinculos_pessoa_imovel",
                newName: "ix_vinculos_perfil_id");

            migrationBuilder.RenameIndex(
                name: "ix_vinculos_pessoa_imovel_imovel_id",
                table: "vinculos_pessoa_imovel",
                newName: "ix_vinculos_imovel_id");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "vinculos_pessoa_imovel",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "imovel_id1",
                table: "imoveis_documentos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "finalidade",
                table: "imoveis",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "observacoes",
                table: "imoveis",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "imoveis",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "tipo_imovel_id",
                table: "imoveis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "titulo",
                table: "imoveis",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_vinculos_pessoa_imovel",
                table: "vinculos_pessoa_imovel",
                column: "id");

            migrationBuilder.CreateTable(
                name: "caracteristicas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    tipo_valor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    unidade = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    grupo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ordem = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_caracteristicas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tipo_imovel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "text", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    ordem = table.Column<int>(type: "integer", nullable: false),
                    categoria = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tipo_imovel", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "imoveis_caracteristicas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    imovel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    caracteristica_id = table.Column<Guid>(type: "uuid", nullable: false),
                    valor_bool = table.Column<bool>(type: "boolean", nullable: true),
                    valor_int = table.Column<int>(type: "integer", nullable: true),
                    valor_decimal = table.Column<decimal>(type: "numeric(14,2)", precision: 14, scale: 2, nullable: true),
                    valor_texto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    valor_data = table.Column<DateOnly>(type: "date", nullable: true),
                    observacao = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_imoveis_caracteristicas", x => x.id);
                    table.ForeignKey(
                        name: "fk_imoveis_caracteristicas_caracteristicas_caracteristica_id",
                        column: x => x.caracteristica_id,
                        principalTable: "caracteristicas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_imoveis_caracteristicas_imoveis_imovel_id",
                        column: x => x.imovel_id,
                        principalTable: "imoveis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_vinculos_pessoa_id",
                table: "vinculos_pessoa_imovel",
                column: "pessoa_id");

            migrationBuilder.CreateIndex(
                name: "ux_vinculos_pessoa_imovel_unico",
                table: "vinculos_pessoa_imovel",
                columns: new[] { "pessoa_id", "imovel_id", "perfil_vinculo_imovel_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_imoveis_documentos_imovel_id1",
                table: "imoveis_documentos",
                column: "imovel_id1");

            migrationBuilder.CreateIndex(
                name: "ix_imoveis_tipo_imovel_id",
                table: "imoveis",
                column: "tipo_imovel_id");

            migrationBuilder.CreateIndex(
                name: "ux_caracteristicas_nome",
                table: "caracteristicas",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_imoveis_caracteristicas_caracteristica_id",
                table: "imoveis_caracteristicas",
                column: "caracteristica_id");

            migrationBuilder.CreateIndex(
                name: "ux_ic_imovel_caracteristica",
                table: "imoveis_caracteristicas",
                columns: new[] { "imovel_id", "caracteristica_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_imoveis_tipo_imovel_tipo_imovel_id",
                table: "imoveis",
                column: "tipo_imovel_id",
                principalTable: "tipo_imovel",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_imoveis_documentos_imoveis_imovel_id1",
                table: "imoveis_documentos",
                column: "imovel_id1",
                principalTable: "imoveis",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_vinculos_pessoa_imovel_pessoas_pessoa_id",
                table: "vinculos_pessoa_imovel",
                column: "pessoa_id",
                principalTable: "pessoas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_imoveis_tipo_imovel_tipo_imovel_id",
                table: "imoveis");

            migrationBuilder.DropForeignKey(
                name: "fk_imoveis_documentos_imoveis_imovel_id1",
                table: "imoveis_documentos");

            migrationBuilder.DropForeignKey(
                name: "fk_vinculos_pessoa_imovel_pessoas_pessoa_id",
                table: "vinculos_pessoa_imovel");

            migrationBuilder.DropTable(
                name: "imoveis_caracteristicas");

            migrationBuilder.DropTable(
                name: "tipo_imovel");

            migrationBuilder.DropTable(
                name: "caracteristicas");

            migrationBuilder.DropPrimaryKey(
                name: "pk_vinculos_pessoa_imovel",
                table: "vinculos_pessoa_imovel");

            migrationBuilder.DropIndex(
                name: "ix_vinculos_pessoa_id",
                table: "vinculos_pessoa_imovel");

            migrationBuilder.DropIndex(
                name: "ux_vinculos_pessoa_imovel_unico",
                table: "vinculos_pessoa_imovel");

            migrationBuilder.DropIndex(
                name: "ix_imoveis_documentos_imovel_id1",
                table: "imoveis_documentos");

            migrationBuilder.DropIndex(
                name: "ix_imoveis_tipo_imovel_id",
                table: "imoveis");

            migrationBuilder.DropColumn(
                name: "id",
                table: "vinculos_pessoa_imovel");

            migrationBuilder.DropColumn(
                name: "imovel_id1",
                table: "imoveis_documentos");

            migrationBuilder.DropColumn(
                name: "finalidade",
                table: "imoveis");

            migrationBuilder.DropColumn(
                name: "observacoes",
                table: "imoveis");

            migrationBuilder.DropColumn(
                name: "status",
                table: "imoveis");

            migrationBuilder.DropColumn(
                name: "tipo_imovel_id",
                table: "imoveis");

            migrationBuilder.DropColumn(
                name: "titulo",
                table: "imoveis");

            migrationBuilder.RenameIndex(
                name: "ix_vinculos_perfil_id",
                table: "vinculos_pessoa_imovel",
                newName: "ix_vinculos_pessoa_imovel_perfil_vinculo_imovel_id");

            migrationBuilder.RenameIndex(
                name: "ix_vinculos_imovel_id",
                table: "vinculos_pessoa_imovel",
                newName: "ix_vinculos_pessoa_imovel_imovel_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_vinculos_pessoa_imovel",
                table: "vinculos_pessoa_imovel",
                columns: new[] { "pessoa_id", "imovel_id", "perfil_vinculo_imovel_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_vinculos_pessoa_imovel_pessoas_pessoa_id",
                table: "vinculos_pessoa_imovel",
                column: "pessoa_id",
                principalTable: "pessoas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
