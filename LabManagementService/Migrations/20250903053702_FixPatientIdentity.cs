using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabManagementService.Migrations
{
    /// <inheritdoc />
    public partial class FixPatientIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create a backup table
            migrationBuilder.Sql(@"
            SELECT * INTO Patients_backup FROM Patients;
        ");

            // 2. Drop foreign keys if any (though you said there are none)
            // 3. Drop the original table
            migrationBuilder.DropTable(name: "Patients");

            // 4. Recreate table without identity
            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Complement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            // 5. Copy data back
            migrationBuilder.Sql(@"
            INSERT INTO Patients (Id, Complement, FirstName, LastName, Gender)
            SELECT Id, Complement, FirstName, LastName, Gender FROM Patients_backup;
        ");

            // 6. Drop backup table
            migrationBuilder.Sql(@"
            DROP TABLE Patients_backup;
        ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the process
            migrationBuilder.Sql(@"
            SELECT * INTO Patients_backup FROM Patients;
        ");

            migrationBuilder.DropTable(name: "Patients");

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Complement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.Sql(@"
            SET IDENTITY_INSERT Patients ON;
            INSERT INTO Patients (Id, Complement, FirstName, LastName, Gender)
            SELECT Id, Complement, FirstName, LastName, Gender FROM Patients_backup;
            SET IDENTITY_INSERT Patients OFF;
        ");

            migrationBuilder.Sql(@"
            DROP TABLE Patients_backup;
        ");
        }
    }
}
