using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class SecondCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "clients",
                columns: table => new
                {
                    ClientID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    IsOnline = table.Column<sbyte>(type: "tinyint", nullable: false, defaultValue: (sbyte)0),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    AskTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IpEndPointToString = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_pkey", x => x.ClientID);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    MessageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(type: "longtext", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UserIDFrom = table.Column<int>(type: "int", nullable: true),
                    UserIdTo = table.Column<int>(type: "int", nullable: true),
                    Discriminator = table.Column<string>(type: "varchar(21)", maxLength: 21, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("message_pkey", x => x.MessageID);
                    table.ForeignKey(
                        name: "FK_messages_clients_UserIDFrom",
                        column: x => x.UserIDFrom,
                        principalTable: "clients",
                        principalColumn: "ClientID");
                    table.ForeignKey(
                        name: "FK_messages_clients_UserIdTo",
                        column: x => x.UserIdTo,
                        principalTable: "clients",
                        principalColumn: "ClientID");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_clients_ClientID",
                table: "clients",
                column: "ClientID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clients_Name",
                table: "clients",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_messages_UserIDFrom",
                table: "messages",
                column: "UserIDFrom");

            migrationBuilder.CreateIndex(
                name: "IX_messages_UserIdTo",
                table: "messages",
                column: "UserIdTo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "clients");
        }
    }
}
