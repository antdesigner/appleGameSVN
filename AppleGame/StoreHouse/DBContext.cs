using AntDesigner.weiXinPay;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntDesigner.GameCityBase.EF
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }
        public DbSet<Player> Players { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Notice> Notices { get; set; }
        public DbSet<Message> Messages { get;set;}
        public DbSet<AccountDetail> AccountDeTails { get; set; }
        public DbSet<ManagePlayer> ManagePlayers { get; set; }
        public DbSet<PayOrder> PayOrders { get; set; }
        public DbSet<RedPack> RedPacks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().Property(p => p.Balance).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<AccountDetail>().Property(accountDetail => accountDetail.Amount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<PayOrder>().Property(payOrder => payOrder.Amount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<RedPack>().Property(redPack => redPack.Amount).HasColumnType("decimal(18,2)");
        }
  
    }
      
}
