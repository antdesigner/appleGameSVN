using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppleGame.Migrations
{
    public partial class installDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    ModifyTime = table.Column<DateTime>(nullable: true)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    WeixinName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Content = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    ModifyTime = table.Column<DateTime>(nullable: true)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayOrders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClientIp = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    ModifyTime = table.Column<DateTime>(nullable: true)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    Out_trade_no = table.Column<string>(nullable: true),
                    Prepay_id = table.Column<string>(nullable: true),
                    Success = table.Column<bool>(nullable: false),
                    Transaction_id = table.Column<string>(nullable: true),
                    WeixinName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RedPacks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    Err_code = table.Column<string>(nullable: true),
                    Mch_billno = table.Column<string>(nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: true)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    Send_listid = table.Column<string>(nullable: true),
                    WeixinName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedPacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountDeTails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    AccountId = table.Column<int>(nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    Explain = table.Column<string>(nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: true)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountDeTails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountDeTails_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    AccountId = table.Column<int>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    Discriminator = table.Column<string>(nullable: false),
                    IntroducerWeixinName = table.Column<string>(nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: true)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    OnlineState = table.Column<bool>(nullable: false),
                    WeixinName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Content = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    ModifyTime = table.Column<DateTime>(nullable: true)
                        .Annotation("MySql:ValueGeneratedOnAddOrUpdate", true),
                    ReceiverId = table.Column<int>(nullable: true),
                    ReplyContent = table.Column<string>(nullable: true),
                    SenderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Players_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Players_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountDeTails_AccountId",
                table: "AccountDeTails",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverId",
                table: "Messages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_AccountId",
                table: "Players",
                column: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountDeTails");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Notices");

            migrationBuilder.DropTable(
                name: "PayOrders");

            migrationBuilder.DropTable(
                name: "RedPacks");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
