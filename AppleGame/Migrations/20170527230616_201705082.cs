using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppleGame.Migrations
{
    public partial class _201705082 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Players",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Notices",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Messages",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Accounts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Notices");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Accounts");
        }
    }
}
