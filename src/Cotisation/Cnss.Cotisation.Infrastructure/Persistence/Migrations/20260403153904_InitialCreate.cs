using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cnss.Cotisation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cotisation");

            migrationBuilder.CreateTable(
                name: "cot_declarations",
                schema: "cotisation",
                columns: table => new
                {
                    cot_declaration_identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cot_declaration_employer_identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cot_declaration_year = table.Column<int>(type: "integer", nullable: false),
                    cot_declaration_month = table.Column<int>(type: "integer", nullable: false),
                    cot_declaration_is_published = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cot_declarations", x => x.cot_declaration_identifier);
                });

            migrationBuilder.CreateTable(
                name: "cot_declaration_items",
                schema: "cotisation",
                columns: table => new
                {
                    cot_declaration_item_identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cot_declaration_item_declaration_identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cot_declaration_item_employee_identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cot_declaration_item_gross_salary = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    cot_declaration_item_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cot_declaration_items", x => x.cot_declaration_item_identifier);
                    table.ForeignKey(
                        name: "FK_cot_declaration_items_cot_declarations_cot_declaration_item~",
                        column: x => x.cot_declaration_item_declaration_identifier,
                        principalSchema: "cotisation",
                        principalTable: "cot_declarations",
                        principalColumn: "cot_declaration_identifier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "cotisation",
                table: "cot_declarations",
                columns: new[] { "cot_declaration_identifier", "cot_declaration_employer_identifier", "cot_declaration_is_published", "cot_declaration_month", "cot_declaration_year" },
                values: new object[] { "DEC-0001", "EMP-0001", true, 3, 2026 });

            migrationBuilder.InsertData(
                schema: "cotisation",
                table: "cot_declaration_items",
                columns: new[] { "cot_declaration_item_identifier", "cot_declaration_item_amount", "cot_declaration_item_declaration_identifier", "cot_declaration_item_employee_identifier", "cot_declaration_item_gross_salary" },
                values: new object[,]
                {
                    { "DIT-0001", 75m, "DEC-0001", "SAL-0001", 1500m },
                    { "DIT-0002", 100m, "DEC-0001", "SAL-0002", 2000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_cot_declaration_items_cot_declaration_item_declaration_iden~",
                schema: "cotisation",
                table: "cot_declaration_items",
                column: "cot_declaration_item_declaration_identifier");

            migrationBuilder.CreateIndex(
                name: "IX_cot_declarations_cot_declaration_employer_identifier_cot_de~",
                schema: "cotisation",
                table: "cot_declarations",
                columns: new[] { "cot_declaration_employer_identifier", "cot_declaration_year", "cot_declaration_month" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cot_declaration_items",
                schema: "cotisation");

            migrationBuilder.DropTable(
                name: "cot_declarations",
                schema: "cotisation");
        }
    }
}
