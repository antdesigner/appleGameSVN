using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using AntDesigner.GameCityBase.EF;

namespace AppleGame.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20170504093509_installDb")]
    partial class installDb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("AntDesigner.AppleGame.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Balance")
                        .IsConcurrencyToken()
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime?>("ModifyTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("WeixinName");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("AntDesigner.AppleGame.AccountDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AccountId");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Explain");

                    b.Property<DateTime?>("ModifyTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("AccountDeTails");
                });

            modelBuilder.Entity("AntDesigner.AppleGame.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime?>("ModifyTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int?>("ReceiverId");

                    b.Property<string>("ReplyContent");

                    b.Property<int?>("SenderId");

                    b.HasKey("Id");

                    b.HasIndex("ReceiverId");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("AntDesigner.AppleGame.Notice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime?>("ModifyTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.ToTable("Notices");
                });

            modelBuilder.Entity("AntDesigner.AppleGame.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AccountId");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("IntroducerWeixinName");

                    b.Property<DateTime?>("ModifyTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<bool>("OnlineState");

                    b.Property<string>("WeixinName")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Players");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Player");
                });

            modelBuilder.Entity("AntDesigner.weiXinPay.PayOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ClientIp");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime?>("ModifyTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Out_trade_no");

                    b.Property<string>("Prepay_id");

                    b.Property<bool>("Success");

                    b.Property<string>("Transaction_id");

                    b.Property<string>("WeixinName");

                    b.HasKey("Id");

                    b.ToTable("PayOrders");
                });

            modelBuilder.Entity("AntDesigner.weiXinPay.RedPack", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Err_code");

                    b.Property<string>("Mch_billno");

                    b.Property<DateTime?>("ModifyTime")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Send_listid");

                    b.Property<string>("WeixinName");

                    b.HasKey("Id");

                    b.ToTable("RedPacks");
                });

            modelBuilder.Entity("AntDesigner.AppleGame.ManagePlayer", b =>
                {
                    b.HasBaseType("AntDesigner.AppleGame.Player");


                    b.ToTable("ManagePlayer");

                    b.HasDiscriminator().HasValue("ManagePlayer");
                });

            modelBuilder.Entity("AntDesigner.AppleGame.AccountDetail", b =>
                {
                    b.HasOne("AntDesigner.AppleGame.Account", "Account")
                        .WithMany("AccountDetails")
                        .HasForeignKey("AccountId");
                });

            modelBuilder.Entity("AntDesigner.AppleGame.Message", b =>
                {
                    b.HasOne("AntDesigner.AppleGame.Player", "Receiver")
                        .WithMany()
                        .HasForeignKey("ReceiverId");

                    b.HasOne("AntDesigner.AppleGame.Player", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId");
                });

            modelBuilder.Entity("AntDesigner.AppleGame.Player", b =>
                {
                    b.HasOne("AntDesigner.AppleGame.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId");
                });
        }
    }
}
