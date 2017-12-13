using AntDesigner.GameCityBase.EF;
using AntDesigner.weiXinPay;
using System;
using System.Collections.Generic;
using System.Linq;
using WxPayAPI;

namespace AntDesigner.GameCityBase.interFace
{
    public interface IStorehouse
    {
       
        IList<Message> GetMessagsOfPlayer(Player player);
        IList<Player> GetFrindOfPlayer(Player player);
        ManagePlayer GetManagePlayer();
        IQueryable<Player> GetOnliePlayers();
        Player GetPlayerByName(string name);
        IList<Notice> GetAllNotices();
        Player AddPlayer(Player player);
        Message AddMessage(Message message);
        //PayOrder FindPayOrder(WxPayData WxPayData);
        //IList<RedPack> GetRedPackgerList(DateTime fromDate, DateTime toDate);
        //IList<PayOrder> GetPayOrderList(DateTime fromDate, DateTime toDate);
        Account GetAccountAsNoTracking(int accountId);
        void SaveChanges();
        T AddEntity<T>(T t) where T : class;
        void RemoveEntity<T>(T t) where T : class;
        T GetEntityById<T>(int id) where T : class;
        IList<T> GetEntitys<T>(int n) where T : class;
        IList<Player> GetAllPlayers();
        void Attach<T>(T obj) where T : class;
    

        IList<RedPack> GetRedPackgerList(DateTime fromDate, DateTime toDate);
        IList<PayOrder> GetPayOrderList(DateTime fromDate, DateTime toDate);
    }
}