using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cnss.Affiliation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "affiliation");

            migrationBuilder.CreateTable(
                name: "aff_employees",
                schema: "affiliation",
                columns: table => new
                {
                    aff_employee_identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    aff_employee_registration_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    aff_employee_first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    aff_employee_last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    aff_employee_employer_identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aff_employees", x => x.aff_employee_identifier);
                });

            migrationBuilder.CreateTable(
                name: "aff_employers",
                schema: "affiliation",
                columns: table => new
                {
                    aff_employer_identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    aff_employer_registration_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    aff_employer_company_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    aff_employer_employee_identifiers = table.Column<string[]>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aff_employers", x => x.aff_employer_identifier);
                });

            migrationBuilder.InsertData(
                schema: "affiliation",
                table: "aff_employees",
                columns: new[] { "aff_employee_identifier", "aff_employee_employer_identifier", "aff_employee_first_name", "aff_employee_last_name", "aff_employee_registration_number" },
                values: new object[,]
                {
                    { "SAL-0001", "EMP-0001", "John", "Doe", "MAT-001" },
                    { "SAL-0002", "EMP-0001", "Jane", "Doe", "MAT-002" }
                });

            migrationBuilder.InsertData(
                schema: "affiliation",
                table: "aff_employers",
                columns: new[] { "aff_employer_identifier", "aff_employer_company_name", "aff_employer_employee_identifiers", "aff_employer_registration_number" },
                values: new object[] { "EMP-0001", "ACME SARL", new[] { "SAL-0001", "SAL-0002" }, "RCCM-001" });

            migrationBuilder.CreateIndex(
                name: "IX_aff_employees_aff_employee_employer_identifier",
                schema: "affiliation",
                table: "aff_employees",
                column: "aff_employee_employer_identifier");

            migrationBuilder.CreateIndex(
                name: "IX_aff_employees_aff_employee_registration_number",
                schema: "affiliation",
                table: "aff_employees",
                column: "aff_employee_registration_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_aff_employers_aff_employer_registration_number",
                schema: "affiliation",
                table: "aff_employers",
                column: "aff_employer_registration_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aff_employees",
                schema: "affiliation");

            migrationBuilder.DropTable(
                name: "aff_employers",
                schema: "affiliation");
        }
    }
}
