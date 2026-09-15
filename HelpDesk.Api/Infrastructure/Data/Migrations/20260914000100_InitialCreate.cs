using HelpDesk.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDesk.API.Infrastructure.Data.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20260914000100_InitialCreate")]
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TECNICO",
                columns: table => new
                {
                    ID_TECNICO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NOME = table.Column<string>(type: "VARCHAR2(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "VARCHAR2(100)", maxLength: 100, nullable: false),
                    ESPECIALIDADE = table.Column<string>(type: "VARCHAR2(50)", maxLength: 50, nullable: false),
                    ATIVO = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TECNICO", x => x.ID_TECNICO);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO",
                columns: table => new
                {
                    ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NOME = table.Column<string>(type: "VARCHAR2(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "VARCHAR2(100)", maxLength: 100, nullable: false),
                    DEPARTAMENTO = table.Column<string>(type: "VARCHAR2(100)", maxLength: 100, nullable: false),
                    ATIVO = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO", x => x.ID_USUARIO);
                });

            migrationBuilder.CreateTable(
                name: "CHAMADO",
                columns: table => new
                {
                    ID_CHAMADO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_TECNICO = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    TITULO = table.Column<string>(type: "VARCHAR2(150)", maxLength: 150, nullable: false),
                    DESCRICAO = table.Column<string>(type: "VARCHAR2(1000)", maxLength: 1000, nullable: false),
                    PRIORIDADE = table.Column<string>(type: "VARCHAR2(20)", maxLength: 20, nullable: false),
                    STATUS = table.Column<string>(type: "VARCHAR2(30)", maxLength: 30, nullable: false),
                    DATA_ABERTURA = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    DATA_ATUALIZACAO = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    DATA_FECHAMENTO = table.Column<DateTime>(type: "TIMESTAMP", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHAMADO", x => x.ID_CHAMADO);
                    table.ForeignKey(
                        name: "FK_CHAMADO_TECNICO",
                        column: x => x.ID_TECNICO,
                        principalTable: "TECNICO",
                        principalColumn: "ID_TECNICO");
                    table.ForeignKey(
                        name: "FK_CHAMADO_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "USUARIO",
                        principalColumn: "ID_USUARIO");
                });

            migrationBuilder.CreateTable(
                name: "COMENTARIO",
                columns: table => new
                {
                    ID_COMENTARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_CHAMADO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    AUTOR = table.Column<string>(type: "VARCHAR2(100)", maxLength: 100, nullable: false),
                    TEXTO = table.Column<string>(type: "VARCHAR2(1000)", maxLength: 1000, nullable: false),
                    DATA_CRIACAO = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COMENTARIO", x => x.ID_COMENTARIO);
                    table.ForeignKey(
                        name: "FK_COMENTARIO_CHAMADO",
                        column: x => x.ID_CHAMADO,
                        principalTable: "CHAMADO",
                        principalColumn: "ID_CHAMADO");
                });

            migrationBuilder.CreateIndex(name: "UX_TECNICO_EMAIL", table: "TECNICO", column: "EMAIL", unique: true);
            migrationBuilder.CreateIndex(name: "IX_TECNICO_ESPECIALIDADE", table: "TECNICO", column: "ESPECIALIDADE");
            migrationBuilder.CreateIndex(name: "UX_USUARIO_EMAIL", table: "USUARIO", column: "EMAIL", unique: true);
            migrationBuilder.CreateIndex(name: "IX_CHAMADO_STATUS", table: "CHAMADO", column: "STATUS");
            migrationBuilder.CreateIndex(name: "IX_CHAMADO_PRIORIDADE", table: "CHAMADO", column: "PRIORIDADE");
            migrationBuilder.CreateIndex(name: "IX_CHAMADO_USUARIO", table: "CHAMADO", column: "ID_USUARIO");
            migrationBuilder.CreateIndex(name: "IX_CHAMADO_TECNICO", table: "CHAMADO", column: "ID_TECNICO");
            migrationBuilder.CreateIndex(name: "IX_CHAMADO_DATA_ABERTURA", table: "CHAMADO", column: "DATA_ABERTURA");
            migrationBuilder.CreateIndex(name: "IX_CHAMADO_STATUS_PRIORIDADE", table: "CHAMADO", columns: new[] { "STATUS", "PRIORIDADE" });
            migrationBuilder.CreateIndex(name: "IX_COMENTARIO_CHAMADO", table: "COMENTARIO", column: "ID_CHAMADO");
            migrationBuilder.CreateIndex(name: "IX_COMENTARIO_DATA_CRIACAO", table: "COMENTARIO", column: "DATA_CRIACAO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "COMENTARIO");
            migrationBuilder.DropTable(name: "CHAMADO");
            migrationBuilder.DropTable(name: "TECNICO");
            migrationBuilder.DropTable(name: "USUARIO");
        }
    }
}
