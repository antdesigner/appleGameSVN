using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppleGame.Migrations
{
    public partial class _20170528 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifyTime",
                table: "RedPacks");

            migrationBuilder.DropColumn(
                name: "ModifyTime",
                table: "PayOrders");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ModifyTime",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Notices");

            migrationBuilder.DropColumn(
                name: "ModifyTime",
                table: "Notices");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ModifyTime",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ModifyTime",
                table: "AccountDeTails");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ModifyTime",
                table: "Accounts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyTime",
                table: "RedPacks",
                nullable: true)
                .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyTime",
                table: "PayOrders",
                nullable: true)
                .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Players",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyTime",
                table: "Players",
                nullable: true)
                .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Notices",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyTime",
                table: "Notices",
                nullable: true)
                .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Messages",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyTime",
                table: "Messages",
                nullable: true)
                .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyTime",
                table: "AccountDeTails",
                nullable: true)
                .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Accounts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyTime",
                table: "Accounts",
                nullable: true)
                .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true);
        }
    }
}
