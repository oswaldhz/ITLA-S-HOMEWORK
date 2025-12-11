using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAuthentication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Usuarios",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Email", "Nombre", "PasswordHash", "Rol" },
                values: new object[,]
                {
                    { 1, "admin@lab.com", "Administrador", "PrP+ZrMeO00Q+nC1ytSccRIpSvauTkdqHEBRVdRaoSE=", "Admin" },
                    { 2, "asistente@lab.com", "Asistente", "byXtkMMD/6uPKOxneb4A6x3pWl6X2Jwl2uIPDXdHgm0=", "Asistente" },
                    { 3, "estudiante@lab.com", "Estudiante", "4Qrs5Ak+mcBVzb6uVWIW+0HMth2wP9/5gZYQyrS+VY8=", "Estudiante" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Usuarios");
        }
    }
}
