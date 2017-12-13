
using System.Collections.Generic;
using System.Linq;
using AntDesigner.GameCityBase.interFace;
using AntDesigner.GameCityBase;
using AntDesigner.GameCityBase.EF;
using AntDesigner.AppleGame;
using Microsoft.EntityFrameworkCore;

namespace GameCitys.DomainService
{
    public class PlayerService : ABIStorehouse, IPlayerService
       
    {
        protected IMessageService _messageService;
        protected   ManagePlayer _managePlayer;
        /// <summary>
    /// 玩家
    /// </summary>
    /// <param name="istoreHose"></param>
        public PlayerService(IStorehouse istoreHose,IMessageService messageSevice) : base(istoreHose)
        {
            _messageService = messageSevice;
        }
        /// <summary>
        /// 查找玩家
        /// </summary>
        /// <param name="name">名称标记</param>
        /// <returns></returns>
        public Player FindPlayerByName(string name)
        {
            return _storeHouse.GetPlayerByName(name);
        }
        /// <summary>
        /// 通过账户Id查找玩家
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Player FindPlayerByAccountId(int id) {
            Account account = _storeHouse.GetAccountAsNoTracking(id);
            return _storeHouse.GetPlayerByName(account.WeixinName);
        }
        /// <summary>
        /// 全部玩家
        /// </summary>
        /// <returns></returns>
        public IList<Player> GetAllPlayers()
        {
          
            return _storeHouse.GetAllPlayers();
        }
        /// <summary>
        /// 管理员
        /// </summary>
        /// <returns></returns>
        public ManagePlayer GetManagerPlayer()
        {
            if (_managePlayer==null)
            {
                _managePlayer = _storeHouse.GetManagePlayer();
            }
            return _managePlayer;

        }
        /// <summary>
        /// 获取玩家发展的玩家
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public IList<Player> GetPlayersOfPlayer(Player player)
        {
            return _storeHouse.GetFrindOfPlayer(player);
        }
        /// <summary>
        ///调整玩家账户余额
        /// </summary>
        /// <param name="player">玩家</param>
        /// <param name="amount">金额</param>
        /// <param name="explain">变动原因</param>
        /// <returns></returns>
        public Account AdjustAccount(Player player, decimal amount, string explain)
        {
            player.Account.Addmount(amount, explain);
            var message = new Message(_managePlayer, explain, player);
            _messageService.SendMessage(message);
            return player.Account;
        }
        /// <summary>
        ///调整玩家账户余额
        /// </summary>
        /// <param name="player">玩家</param>
        /// <param name="amount">金额</param>
        /// <param name="explain">变动原因</param>
        /// <returns></returns>
        public void AdjustAccount(string name, decimal amount, string explain)
        {
            var player = FindPlayerByName(name);
            player.Account.Addmount(amount, explain);
            var message = new Message(_managePlayer, explain, player);
            _messageService.SendMessage(message);
            // _storeHouse.SaveChanges();
           
        }
        /// <summary>
        ///调整玩家账户余额(用于委托)
        /// </summary>
        /// <param name="player">玩家</param>
        /// <param name="amount">金额</param>
        /// <param name="explain">变动原因</param>
        /// <returns></returns>
        public static decimal AdjustAccountForDelegate(string name, decimal amount, string explain)
        {
            DbContextOptions<DataContext> dbBuilder = new DbContextOptionsBuilder<DataContext>()
           .UseMySql(Startup.Connection).Options;
            using (var db = new DataContext(dbBuilder))
            {
                using ( var transaction=db.Database.BeginTransaction())
                {
                    try
                    {
                        var player = db.Players.Include(p => p.Account).ThenInclude(a => a.AccountDetails).FirstOrDefault(p => p.WeixinName == name);
                        player.Account.Addmount(amount, explain);
                        db.SaveChanges();
                        var _managePlayer = db.ManagePlayers.FirstOrDefault();
                        var message = new Message(_managePlayer, explain, player);
                        db.Messages.Add(message);
                        db.SaveChanges();
                        transaction.Commit();
                        return player.Account.Balance;
                    }
                    catch (System.Exception)
                    {

                        throw;
                    }
                }
             
            };
        }
        public void AttachPlayer(Player player)
        {
           _storeHouse.Attach<Player>(player);
            return;
        }
        /// <summary>
        /// 玩家账户明细
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public IList<AccountDetail> GetAccountDetailsOfPlayer(Player player)
        {
            return player.Account.AccountDetails.OrderByDescending(a => a.Id).ToList();
        }
        /// <summary>
        /// 新增玩家
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public Player AddPlayer(Player player)
        {
            _storeHouse.AddEntity<Player>(player);
            _storeHouse.SaveChanges();
            return player;
     
        }
    }
}
