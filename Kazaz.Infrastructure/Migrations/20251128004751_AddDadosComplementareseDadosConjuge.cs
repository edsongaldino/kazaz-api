using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDadosComplementareseDadosConjuge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "conjuges",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pessoa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    rg = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    orgao_expedidor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    data_nascimento = table.Column<DateOnly>(type: "date", nullable: true),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_conjuges", x => x.id);
                    table.ForeignKey(
                        name: "fk_conjuges_pessoas_pessoa_id",
                        column: x => x.pessoa_id,
                        principalTable: "pessoas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contatos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pessoa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    valor = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    principal = table.Column<bool>(type: "boolean", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contatos", x => x.id);
                    table.ForeignKey(
                        name: "fk_contatos_pessoas_pessoa_id",
                        column: x => x.pessoa_id,
                        principalTable: "pessoas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dados_complementares",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pessoa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    profissao = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    escolaridade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    renda_mensal = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    observacoes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dados_complementares", x => x.id);
                    table.ForeignKey(
                        name: "fk_dados_complementares_pessoas_pessoa_id",
                        column: x => x.pessoa_id,
                        principalTable: "pessoas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_conjuges_pessoa_id",
                table: "conjuges",
                column: "pessoa_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_contatos_pessoa_id",
                table: "contatos",
                column: "pessoa_id");

            migrationBuilder.CreateIndex(
                name: "ix_dados_complementares_pessoa_id",
                table: "dados_complementares",
                column: "pessoa_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "conjuges");

            migrationBuilder.DropTable(
                name: "contatos");

            migrationBuilder.DropTable(
                name: "dados_complementares");
        }
    }
}
