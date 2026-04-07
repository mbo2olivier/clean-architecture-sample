using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cnss.Affiliation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorToDomainEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "aff_employer_employee_identifiers",
                schema: "affiliation",
                table: "aff_employers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "aff_employer_employee_identifiers",
                schema: "affiliation",
                table: "aff_employers",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.UpdateData(
                schema: "affiliation",
                table: "aff_employers",
                keyColumn: "aff_employer_identifier",
                keyValue: "EMP-0001",
                column: "aff_employer_employee_identifiers",
                value: new[] { "SAL-0001", "SAL-0002" });
        }
    }
}
