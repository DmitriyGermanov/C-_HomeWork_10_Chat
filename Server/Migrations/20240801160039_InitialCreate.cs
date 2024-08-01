using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    IsOnline = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AskTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    ClientType = table.Column<string>(type: "varchar(21)", maxLength: 21, nullable: false),
                    IpEndPointToString = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientID", x => x.ClientID);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(type: "longtext", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UserIDFrom = table.Column<int>(type: "int", nullable: true),
                    UserIdTo = table.Column<int>(type: "int", nullable: true),
                    Discriminator = table.Column<string>(type: "varchar(21)", maxLength: 21, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageID", x => x.MessageID);
                    table.ForeignKey(
                        name: "FK_Messages_Clients_UserIDFrom",
                        column: x => x.UserIDFrom,
                        principalTable: "Clients",
                        principalColumn: "ClientID");
                    table.ForeignKey(
                        name: "FK_Messages_Clients_UserIdTo",
                        column: x => x.UserIdTo,
                        principalTable: "Clients",
                        principalColumn: "ClientID");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ClientID",
                table: "Clients",
                column: "ClientID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserIDFrom",
                table: "Messages",
                column: "UserIDFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserIdTo",
                table: "Messages",
                column: "UserIdTo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
