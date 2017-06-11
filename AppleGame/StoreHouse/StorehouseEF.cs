using AntDesigner.GameCityBase.EF;
using AntDesigner.GameCityBase.interFace;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AntDesigner.weiXinPay;
using WxPayAPI;
using GameCitys.EF;

namespace AntDesigner.GameCityBase
{
    public class StorehouseEF : IStorehouse, IStoreHoseForWeixin
    {

    protected DataContext Db { get ; set; }

        public StorehouseEF(DataContext db_)
        {
            Db = db_;
        }
       

        /// <summary>
        /// 添加EF实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="t">对象</param>
        /// <returns></returns>
        public T AddEntity<T>(T t) where T : class
        {
            Db.Add<T>(t);
            return t;
        }
        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="t">实体对象</param>
        public void RemoveEntity<T>(T t) where T : class
        {
            Db.Remove<T>(t);
            Db.SaveChanges();
        }
        /// <summary>
        /// 通过Id找到实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="id">实体对象ID</param>
        /// <returns>实体对象</returns>
        public T GetEntityById<T>(int id) where T : class
        {
            return Db.Find<T>(id);
        }
        /// <summary>
        /// 返回前几个实体集合
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="n">返回实体数量</param>
        /// <returns></returns>
        public IList<T> GetEntitys<T>(int n) where T : class
        {
            Type t = typeof(T);
            string typeName = t.Name;
            PropertyInfo propertyInfo = Db.GetType().GetProperty(typeName + "s");
            DbSet<T> entitys = (DbSet<T>)propertyInfo.GetValue(Db);
            return (entitys.Take<T>(n)).ToList();

        }
        /// <summary>
        /// 保存上下文
        /// </summary>
        public void SaveChanges()
        {
            try
            {
                Db.SaveChanges();
            }

            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {

                    if (entry.Entity is Account)
                    {
                        int accountId = (int)entry.Property("id").CurrentValue;
                        decimal databaseValue = Db.Accounts.AsNoTracking().FirstOrDefault(a => a.Id == accountId).Balance;
                        decimal currentValue = (decimal)entry.Property("balance").CurrentValue;

                        entry.Property("balance").CurrentValue = currentValue + ((decimal)entry.Property("balance").OriginalValue - databaseValue);
                        entry.Property("balance").OriginalValue = databaseValue;
                    }
                    else
                    {
                        throw new NotSupportedException((int)entry.Property("id").CurrentValue + "账户变更冲突");
                    }
                }
                Db.SaveChanges();
            }

        }
        /// <summary>
        /// 实体对象关联到Dbcontext
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="obj">实体对象</param>
        public void Attach<T>(T obj) where T : class
        {
            if (obj != null)
            {
                Db.Attach(obj);
            }

        }
        /// <summary>
        /// 添加消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns><消息/returns>
        public Message AddMessage(Message message)
        {
            AddEntity<Message>(message);
            //if (db.Entry(message.Sender).State==EntityState.Added)
            //    {
            //    db.Entry(message.Sender).State = EntityState.Unchanged;
            //    }
            Db.SaveChanges();
            return message;
        }
        /// <summary>
        /// 得到玩家全部推广玩家
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public IList<Player> GetFrindOfPlayer(Player player)
        {
            return Db.Players.AsNoTracking().Include(p => p.Account).Where(p => p.IntroducerWeixinName == player.WeixinName).ToList();
        }
        /// <summary>
        /// 得到全部在线玩家
        /// </summary>
        /// <returns></returns>
        public IQueryable<Player> GetOnliePlayers()
        {
            return Db.Players.AsNoTracking().Include(p => p.Account).Where(p => p.OnlineState == true);
        }
        /// <summary>
        /// 得到全部玩家
        /// </summary>
        /// <returns></returns>
        public IList<Player> GetAllPlayers()
        {

            return Db.Players.AsNoTracking().Include(p => p.Account).ToList();
        }
        /// <summary>
        /// 得到玩家全部消息
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public IList<Message> GetMessagsOfPlayer(Player player)
        {
            return Db.Messages.AsNoTracking().Include(m => m.Sender).Where(m => m.Receiver.Id == player.Id).ToList();
            // return db.Messages.Include(m => m.sender).Where(m => m.receiver.id == player.id).ToList();
        }
        /// <summary>
        /// 通过用户名openid找到玩家
        /// </summary>
        /// <param name="name">openId</param>
        /// <returns></returns>
        public Player GetPlayerByName(string name)
        {
            //   var player=db.Players.Include(p=>p.account)
            //      .ThenInclude(a=>a.accountDetails).FirstOrDefault(p => p.weixinName == name);
            Player player = Db.Players.FirstOrDefault(p => p.WeixinName == name);
            if (player == null)
            {
                return player;
            }
            Db.Entry(player).Reference(p => p.Account).Load();
            Db.Entry(player.Account).Collection(a => a.AccountDetails).Load();
            return player;
        }
        /// <summary>
        /// 获得管理员
        /// </summary>
        /// <returns></returns>
        public ManagePlayer GetManagePlayer()
        {

            var mananger = Db.ManagePlayers.FirstOrDefault();
            Db.Entry(mananger).Reference(p => p.Account).Load();
            Db.Entry(mananger.Account).Collection(a => a.AccountDetails).Load();

            return mananger;
        }
        /// <summary>
        /// 添加玩家
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public Player AddPlayer(Player player)
        {

            Db.Players.Add(player);
            Db.SaveChanges();
            return player;
        }
        /// <summary>
        /// 获得全部公告
        /// </summary>
        /// <returns></returns>
        public IList<Notice> GetAllNotices()
        {
            var notices = from item in Db.Notices
                          select item;
            return notices.ToList();
        }
        #region IStoreHoseForWeixin 微信支付仓库接口
        /// <summary>
        /// 找到订单
        /// </summary>
        /// <param name="WxPayData">微信返回</param>
        /// <returns></returns>
        public PayOrder FindPayOrder(string out_trade_no)
        {
          //  string out_trade_no = WxPayData.GetValue("out_trade_no").ToString();
           // string openid = WxPayData.GetValue("openid").ToString();
           // decimal total_fee = (int.Parse(WxPayData.GetValue("total_fee").ToString())) / 100;
            return Db.PayOrders.FirstOrDefault(p => p.Out_trade_no == out_trade_no);
        }
        /// <summary>
        ///无跟踪返回玩家账户
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public Account GetAccountAsNoTracking(int accountId)
        {
            return Db.Accounts.AsNoTracking().FirstOrDefault(a => a.Id == accountId);
        }
        /// <summary>
        /// 红包列表
        /// </summary>
        /// <param name="fromDate">开始日期</param>
        /// <param name="toDate">结束日期</param>
        /// <returns></returns>
        public IList<RedPack> GetRedPackgerList(DateTime fromDate, DateTime toDate)
        {
            return Db.RedPacks.AsNoTracking().Where(r => r.CreateTime >= fromDate && r.CreateTime <= toDate).OrderByDescending(r => r.CreateTime).ToList();
        }
        /// <summary>
        /// 支付订单列表
        /// </summary>
        /// <param name="fromDate">开始日期</param>
        /// <param name="toDate">结算日期</param>
        /// <returns></returns>
        public IList<PayOrder> GetPayOrderList(DateTime fromDate, DateTime toDate)
        {
            return Db.PayOrders.AsNoTracking().Where(r => r.CreateTime >= fromDate && r.CreateTime <= toDate).OrderByDescending(r => r.CreateTime).ToList();

        }
#endregion

    }
}
