using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "documentos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    caminho = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    content_type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    tamanho_bytes = table.Column<long>(type: "bigint", nullable: true),
                    data_upload = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_documentos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "estados",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    uf = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_estados", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "origens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_origens", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "perfis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_perfis", x => x.id);
                });

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
                name: "tipos_documento",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    alvo = table.Column<int>(type: "integer", nullable: false),
                    obrigatorio = table.Column<bool>(type: "boolean", nullable: false),
                    ordem = table.Column<int>(type: "integer", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tipos_documento", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cidades",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    ibge = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    estado_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cidades", x => x.id);
                    table.ForeignKey(
                        name: "fk_cidades_estados_estado_id",
                        column: x => x.estado_id,
                        principalTable: "estados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    senha = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    perfil_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuarios", x => x.id);
                    table.ForeignKey(
                        name: "fk_usuarios_perfis_perfil_id",
                        column: x => x.perfil_id,
                        principalTable: "perfis",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "enderecos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cep = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    logradouro = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    complemento = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    bairro = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    cidade_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_enderecos", x => x.id);
                    table.ForeignKey(
                        name: "fk_enderecos_cidades_cidade_id",
                        column: x => x.cidade_id,
                        principalTable: "cidades",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "imoveis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    endereco_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_imoveis", x => x.id);
                    table.ForeignKey(
                        name: "fk_imoveis_enderecos_endereco_id",
                        column: x => x.endereco_id,
                        principalTable: "enderecos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pessoas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    endereco_id = table.Column<Guid>(type: "uuid", nullable: true),
                    origem_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pessoas", x => x.id);
                    table.ForeignKey(
                        name: "fk_pessoas_enderecos_endereco_id",
                        column: x => x.endereco_id,
                        principalTable: "enderecos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_pessoas_origens_origem_id",
                        column: x => x.origem_id,
                        principalTable: "origens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fotos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    imovel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    caminho = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    ordem = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fotos", x => x.id);
                    table.ForeignKey(
                        name: "fk_fotos_imoveis_imovel_id",
                        column: x => x.imovel_id,
                        principalTable: "imoveis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "imoveis_documentos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    imovel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_documento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    documento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_anexo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    observacao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_imoveis_documentos", x => x.id);
                    table.ForeignKey(
                        name: "fk_imoveis_documentos_documentos_documento_id",
                        column: x => x.documento_id,
                        principalTable: "documentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_imoveis_documentos_imoveis_imovel_id",
                        column: x => x.imovel_id,
                        principalTable: "imoveis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_imoveis_documentos_tipos_documento_tipo_documento_id",
                        column: x => x.tipo_documento_id,
                        principalTable: "tipos_documento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "dados_pessoa_fisica",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    data_nascimento = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dados_pessoa_fisica", x => x.id);
                    table.ForeignKey(
                        name: "fk_dados_pessoa_fisica_pessoas_id",
                        column: x => x.id,
                        principalTable: "pessoas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dados_pessoa_juridica",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    razao_social = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    data_abertura = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dados_pessoa_juridica", x => x.id);
                    table.ForeignKey(
                        name: "fk_dados_pessoa_juridica_pessoas_id",
                        column: x => x.id,
                        principalTable: "pessoas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pessoas_documentos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pessoa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_documento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    documento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_anexo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    observacao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pessoas_documentos", x => x.id);
                    table.ForeignKey(
                        name: "fk_pessoas_documentos_documentos_documento_id",
                        column: x => x.documento_id,
                        principalTable: "documentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pessoas_documentos_pessoas_pessoa_id",
                        column: x => x.pessoa_id,
                        principalTable: "pessoas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pessoas_documentos_tipos_documento_tipo_documento_id",
                        column: x => x.tipo_documento_id,
                        principalTable: "tipos_documento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vinculos_pessoa_imovel",
                columns: table => new
                {
                    pessoa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    imovel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    perfil_vinculo_imovel_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vinculos_pessoa_imovel", x => new { x.pessoa_id, x.imovel_id, x.perfil_vinculo_imovel_id });
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "socios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pessoa_fisica_id = table.Column<Guid>(type: "uuid", nullable: false),
                    percentual = table.Column<decimal>(type: "numeric(9,2)", precision: 9, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_socios", x => x.id);
                    table.ForeignKey(
                        name: "fk_socios_dados_pessoas_fisicas_pessoa_fisica_id",
                        column: x => x.pessoa_fisica_id,
                        principalTable: "dados_pessoa_fisica",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_socios_dados_pessoas_juridicas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "dados_pessoa_juridica",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cidades_estado_id_nome",
                table: "cidades",
                columns: new[] { "estado_id", "nome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cidades_ibge",
                table: "cidades",
                column: "ibge");

            migrationBuilder.CreateIndex(
                name: "ix_pessoas_cpf_1",
                table: "dados_pessoa_fisica",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_dados_pessoa_juridica_cnpj",
                table: "dados_pessoa_juridica",
                column: "cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_enderecos_cep",
                table: "enderecos",
                column: "cep");

            migrationBuilder.CreateIndex(
                name: "ix_enderecos_cidade_id",
                table: "enderecos",
                column: "cidade_id");

            migrationBuilder.CreateIndex(
                name: "ix_enderecos_logradouro_numero_bairro_cidade_id",
                table: "enderecos",
                columns: new[] { "logradouro", "numero", "bairro", "cidade_id" });

            migrationBuilder.CreateIndex(
                name: "ix_estados_uf",
                table: "estados",
                column: "uf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_fotos_imovel_id_ordem",
                table: "fotos",
                columns: new[] { "imovel_id", "ordem" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_imoveis_codigo",
                table: "imoveis",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_imoveis_endereco_id",
                table: "imoveis",
                column: "endereco_id");

            migrationBuilder.CreateIndex(
                name: "ix_imoveis_documentos_documento_id",
                table: "imoveis_documentos",
                column: "documento_id");

            migrationBuilder.CreateIndex(
                name: "ix_imoveis_documentos_tipo_documento_id",
                table: "imoveis_documentos",
                column: "tipo_documento_id");

            migrationBuilder.CreateIndex(
                name: "UX_imoveis_documentos_unique",
                table: "imoveis_documentos",
                columns: new[] { "imovel_id", "tipo_documento_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_origens_nome",
                table: "origens",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "ix_perfis_vinculo_imovel_nome",
                table: "perfis_vinculo_imovel",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pessoas_endereco_id",
                table: "pessoas",
                column: "endereco_id");

            migrationBuilder.CreateIndex(
                name: "ix_pessoas_origem_id",
                table: "pessoas",
                column: "origem_id");

            migrationBuilder.CreateIndex(
                name: "ix_pessoas_documentos_documento_id",
                table: "pessoas_documentos",
                column: "documento_id");

            migrationBuilder.CreateIndex(
                name: "ix_pessoas_documentos_tipo_documento_id",
                table: "pessoas_documentos",
                column: "tipo_documento_id");

            migrationBuilder.CreateIndex(
                name: "UX_pessoas_documentos_unique",
                table: "pessoas_documentos",
                columns: new[] { "pessoa_id", "tipo_documento_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_socios_empresa_id_pessoa_fisica_id",
                table: "socios",
                columns: new[] { "empresa_id", "pessoa_fisica_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_socios_pessoa_fisica_id",
                table: "socios",
                column: "pessoa_fisica_id");

            migrationBuilder.CreateIndex(
                name: "ix_tipos_documento_alvo_nome",
                table: "tipos_documento",
                columns: new[] { "alvo", "nome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_perfil_id",
                table: "usuarios",
                column: "perfil_id");

            migrationBuilder.CreateIndex(
                name: "ix_vinculos_pessoa_imovel_imovel_id",
                table: "vinculos_pessoa_imovel",
                column: "imovel_id");

            migrationBuilder.CreateIndex(
                name: "ix_vinculos_pessoa_imovel_perfil_vinculo_imovel_id",
                table: "vinculos_pessoa_imovel",
                column: "perfil_vinculo_imovel_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fotos");

            migrationBuilder.DropTable(
                name: "imoveis_documentos");

            migrationBuilder.DropTable(
                name: "pessoas_documentos");

            migrationBuilder.DropTable(
                name: "socios");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "vinculos_pessoa_imovel");

            migrationBuilder.DropTable(
                name: "documentos");

            migrationBuilder.DropTable(
                name: "tipos_documento");

            migrationBuilder.DropTable(
                name: "dados_pessoa_fisica");

            migrationBuilder.DropTable(
                name: "dados_pessoa_juridica");

            migrationBuilder.DropTable(
                name: "perfis");

            migrationBuilder.DropTable(
                name: "imoveis");

            migrationBuilder.DropTable(
                name: "perfis_vinculo_imovel");

            migrationBuilder.DropTable(
                name: "pessoas");

            migrationBuilder.DropTable(
                name: "enderecos");

            migrationBuilder.DropTable(
                name: "origens");

            migrationBuilder.DropTable(
                name: "cidades");

            migrationBuilder.DropTable(
                name: "estados");
        }
    }
}
