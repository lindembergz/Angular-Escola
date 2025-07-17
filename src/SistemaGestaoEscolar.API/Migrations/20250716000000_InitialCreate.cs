using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGestaoEscolar.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tabela Escolas
            migrationBuilder.CreateTable(
                name: "Escolas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    CNPJ = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: false),
                    TipoEscola = table.Column<int>(type: "int", nullable: false),
                    Endereco_Logradouro = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Endereco_Numero = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    Endereco_Complemento = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Endereco_Bairro = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Endereco_Cidade = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Endereco_Estado = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    Endereco_CEP = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false),
                    RedeEscolarId = table.Column<Guid>(type: "char(36)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Escolas", x => x.Id);
                });

            // Tabela RedesEscolares
            migrationBuilder.CreateTable(
                name: "RedesEscolares",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    CNPJ = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedesEscolares", x => x.Id);
                });

            // Índices
            migrationBuilder.CreateIndex(
                name: "IX_Escolas_CNPJ",
                table: "Escolas",
                column: "CNPJ",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Escolas_RedeEscolarId",
                table: "Escolas",
                column: "RedeEscolarId");

            migrationBuilder.CreateIndex(
                name: "IX_RedesEscolares_CNPJ",
                table: "RedesEscolares",
                column: "CNPJ",
                unique: true);

            // Foreign Keys
            migrationBuilder.AddForeignKey(
                name: "FK_Escolas_RedesEscolares_RedeEscolarId",
                table: "Escolas",
                column: "RedeEscolarId",
                principalTable: "RedesEscolares",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Escolas_RedesEscolares_RedeEscolarId",
                table: "Escolas");

            migrationBuilder.DropTable(
                name: "Escolas");

            migrationBuilder.DropTable(
                name: "RedesEscolares");
        }
    }
}