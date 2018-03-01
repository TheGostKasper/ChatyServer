using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace signalr_server.Migrations
{
    public partial class AddCreationDateToUsersAndMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Messages",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Messages");
        }
    }
}
