using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kazaz.Infrastructure.Migrations
{
    public partial class AlterarEstadoCivilParaInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) cria coluna temporária int
            migrationBuilder.AddColumn<int>(
                name: "estado_civil_tmp",
                table: "dados_pessoa_fisica",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // 2) converte string -> int
            migrationBuilder.Sql(@"
                UPDATE dados_pessoa_fisica
                SET estado_civil_tmp = CASE LOWER(TRIM(estado_civil))
                  WHEN 'naoinformado' THEN 0
                  WHEN 'não informado' THEN 0
                  WHEN 'nao informado' THEN 0
                  WHEN 'solteiro' THEN 1
                  WHEN 'casado' THEN 2
                  WHEN 'divorciado' THEN 3
                  WHEN 'viuvo' THEN 4
                  WHEN 'viúvo' THEN 4
                  WHEN 'uniaoestavel' THEN 5
                  WHEN 'união estável' THEN 5
                  WHEN 'uniao estavel' THEN 5
                  WHEN 'separado' THEN 6
                  ELSE 0
                END;
            ");

            // 3) remove coluna antiga (varchar)
            migrationBuilder.DropColumn(
                name: "estado_civil",
                table: "dados_pessoa_fisica");

            // 4) renomeia tmp -> estado_civil
            migrationBuilder.RenameColumn(
                name: "estado_civil_tmp",
                table: "dados_pessoa_fisica",
                newName: "estado_civil");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // volta para varchar(30) gravando o NOME do enum

            // 1) cria coluna temporária varchar
            migrationBuilder.AddColumn<string>(
                name: "estado_civil_tmp",
                table: "dados_pessoa_fisica",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "NaoInformado");

            // 2) converte int -> string
            migrationBuilder.Sql(@"
                UPDATE dados_pessoa_fisica
                SET estado_civil_tmp = CASE estado_civil
                  WHEN 0 THEN 'NaoInformado'
                  WHEN 1 THEN 'Solteiro'
                  WHEN 2 THEN 'Casado'
                  WHEN 3 THEN 'Divorciado'
                  WHEN 4 THEN 'Viuvo'
                  WHEN 5 THEN 'UniaoEstavel'
                  WHEN 6 THEN 'Separado'
                  ELSE 'NaoInformado'
                END;
            ");

            // 3) remove coluna int
            migrationBuilder.DropColumn(
                name: "estado_civil",
                table: "dados_pessoa_fisica");

            // 4) renomeia tmp -> estado_civil
            migrationBuilder.RenameColumn(
                name: "estado_civil_tmp",
                table: "dados_pessoa_fisica",
                newName: "estado_civil");
        }
    }
}