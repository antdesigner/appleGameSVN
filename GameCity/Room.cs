using System;
using System.Collections.Generic;

namespace AntDesigner.NetCore.GameCity
{
    public class RoomConfig : IRoomConfig
    {
        /// <summary>
        /// 房间名称
        /// </summary>

        public string Name { get; set; }
        /// <summary>
        /// 满员?
        /// </summary>
       
        public bool IsFull { get; set; }
        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime Timelimit { get; set; }
        /// <summary>
        /// 允许进入?
        /// </summary>
        public bool IsOpening { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// 门票价格
        /// </summary>
        public decimal TicketPrice { get; set; }
        /// <summary>
        /// 人数上限
        /// </summary>
        public int PlayerCountTopLimit { get; set; }
        /// <summary>
        /// 游戏
        /// </summary>
        public IInningeGame InningGame { get; set; }
        /// <summary>
        /// 公告
        /// </summary>
        public string Affiche { get; set; }
        /// <summary>
        /// 是否随机?
        /// </summary>
        public bool IsRandom { get; set; }
        /// <summary>
        /// 房间默认配置
        /// </summary>
        /// <param name="inningGame_"></param>
    
        public RoomConfig(IInningeGame inningGame_)
        {
            InningGame = inningGame_;
            IsFull = false;
            Timelimit = DateTime.Now.AddHours(1);
            IsOpening = true;
            SecretKey = "";
            TicketPrice = 0;
            PlayerCountTopLimit = 8;
            Affiche = "";
            IsRandom = true;
        }
    }

    public class Room : IRoom
    {
        
        /// <summary>
        /// 房间id自动生成
        /// </summary>
        public string Id { set; get; }
        /// <summary>
        /// 房间名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 满员
        /// </summary>
        public bool IsFull { get; private set; }
        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime Timelimit { get; private set; }
        /// <summary>
        /// 开放?
        /// </summary>
        public bool IsOpening { get; private set; }
        /// <summary>
        /// 进入密码
        /// </summary>
        public string SecretKey { get; private set; }
        /// <summary>
        /// 门票价格
        /// </summary>
        public decimal TicketPrice { get; private set; }
        /// <summary>
        /// 人数上限
        /// </summary>
        public int PlayerCountTopLimit { get; private set; }
        /// <summary>
        /// 游戏
        /// </summary>
        public IInningeGame InningGame { get; private set; }
        /// <summary>
        /// 房主
        /// </summary>
        public IPlayerJoinRoom RoomManager { get; private set; }
        /// <summary>
        /// 公告
        /// </summary>
        public string Affiche { get; set; }
        /// <summary>
        /// 玩家列表
        /// </summary>
        public List<IPlayerJoinRoom> Players { get; private set; }
        /// <summary>
        /// 是否允许随机加入
        /// </summary>
        public bool IsRandom { get; set; }
        public string GameCityId { get; set; }
        /// <summary>
        /// 玩家进入人数满了事件
        /// </summary>
        public  event EventHandler AddPlayerFailRoomFullEvent;
        /// <summary>
        /// 玩家成功进入后事件
        /// </summary>
        public  event EventHandler AddPlayer_SuccessEvent;
        /// <summary>
        /// 玩家离开后事件
        /// </summary>
        public event EventHandler RemovePlayer_SuccessEvent;
        /// <summary>
        /// 玩家不能支付门票事件
        /// </summary>
        public  event EventHandler PlayCanNotPayTicketEvent;
        /// <summary>
        /// 房主易主后事件
        /// </summary>
        public  event EventHandler ManagerChangedHandler;
    
        /// <summary>
        /// 是否能删除房间委托检查
        /// </summary>
        public  Func<IRoom, bool> DCanDeleteRoomCheck { get; set; }

        /// <summary>
        /// 开房
        /// </summary>
        /// <param name="timelimit_">到期时间</param>
        /// <param name="playerCountTopLimit_">人数上限</param>
        /// <param name="inningGame_">游戏</param>
        /// <param name="roomManager_">房主</param>
        /// <param name="name_">房间名称</param>
        /// <param name="affiche_">房间在大厅公告</param>
        /// <param name="secretKey_">密码</param>
        /// <param name="ticketPrice_">门票价格</param>
        /// <param name="isRandom_">是否玩家随机进入</param>

        private Room()
        {

            Id = PublicObject.GenerateTimeStamp();
            Players = new List<IPlayerJoinRoom>();
        }
       /// <summary>
       /// 新房间
       /// </summary>
       /// <param name="roomManager_">房主</param>
       /// <param name="config_">房间配置</param>
        public Room(IPlayerJoinRoom roomManager_, IRoomConfig config_) : this()
        {
            Id = Id + roomManager_.Id;
            Name = config_.Name;
            IsFull = config_.IsFull;
            Timelimit = config_.Timelimit;
            IsOpening = config_.IsOpening;
            SecretKey = config_.SecretKey;
            TicketPrice = config_.TicketPrice;
            InningGame = config_.InningGame;
            InningGame.IRoom = this;
            if (InningGame.IGameProject.PlayerCountLeast < config_.PlayerCountTopLimit
          && config_.PlayerCountTopLimit <= InningGame.IGameProject.PlayerCountLimit)
            {
                PlayerCountTopLimit = config_.PlayerCountTopLimit;
            }
            else
            {
                PlayerCountTopLimit = InningGame.IGameProject.PlayerCountLimit;
            }
            if (PlayerCountTopLimit==1)
            {
                IsFull = true;
            }
            RoomManager = roomManager_;
            Affiche = config_.Affiche;
            IsRandom = config_.IsRandom;
            Players.Add(roomManager_);
            InningGame.PlaySitDown(roomManager_);
        }
        public Room(
             DateTime timelimit_
            , int playerCountTopLimit_
            , IInningeGame inningGame_
            , IPlayerJoinRoom roomManager_
            , string name_ = ""
            , string affiche_ = ""
            , string secretKey_ = ""
            , decimal ticketPrice_ = 0
            , bool isRandom_ = true) : this()
        {
            TicketPrice = ticketPrice_;
            IsFull = false;
            IsOpening = true;
            // Players = new List<IPlayerJoinRoom>();
            InningGame = inningGame_;
            RoomManager = roomManager_;
            Players.Add(roomManager_);
            if (inningGame_.IGameProject.PlayerCountLeast <= playerCountTopLimit_
          && playerCountTopLimit_ <= inningGame_.IGameProject.PlayerCountLimit)
            {
                PlayerCountTopLimit = playerCountTopLimit_;
            }
            else
            {
                PlayerCountTopLimit = inningGame_.IGameProject.PlayerCountLimit;
            }
        }
        /// <summary>
        /// 玩家加入
        /// </summary>
        /// <param name="player_">玩家</param>
        public bool AddPlayer(IPlayerJoinRoom player_)
        {
            if (Players.Exists(p => p.Id == player_.Id))
            { return true; }
                if (Players.Count >= PlayerCountTopLimit)
            {
                AddPlayerFailRoomFullEvent?.Invoke(player_, new PlayerEventArgs(player_));
                return false;
            }
            if (player_.Id!=RoomManager.Id && TicketPrice > 0 
                && player_.Account < TicketPrice)
            {
                PlayCanNotPayTicketEvent?.Invoke(player_, new PlayerEventArgs(player_));
                return false;
            }
            if (player_.Id!=RoomManager.Id)
            {
                if (!player_.DecutMoney(TicketPrice))
                {
                    return false;
                }
            }
            if (!Players.Exists(p => p.Id == player_.Id))
            {
                Players.Add(player_);
                try
                {
                    InningGame.PlaySitDown(player_);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    CheckFull();
                    //AddPlayerSuccessEvent?.Invoke(this, new PlayerEventArgs(player_));
                    AddPlayer_SuccessEvent?.Invoke(this, new PlayerEventArgs(player_));
                }

            }
            return true;
        }
        /// <summary>
        /// 检查设置满员标记
        /// </summary>
        /// <returns>满员</returns>
        private bool CheckFull()
        {
            if (Players.Count >= PlayerCountTopLimit)
            {
                IsFull = true;
                return true;
            }
            else
            {
                IsFull = false;
                return false;
            }
        }
        /// <summary>
        /// 修改房间公告
        /// </summary>
        /// <param name="affice_">公告</param>
        public void ChangeAffice(string affice_)
        {
            Affiche = affice_;
        }
        /// <summary>
        /// 玩家离开房间
        /// </summary>
        /// <param name="player_">玩家</param>
        public void RemovePlayer(IPlayerJoinRoom player_)
        {
            ISeat seat = InningGame.GetSeatByPlayerId(player_.Id );
            if (seat != null)
            {
                seat.PlayLeave();
            }
            var realPlayer =Players.Find(p => p.Id == player_.Id);
            Players.Remove(realPlayer);
            if (player_.Id == RoomManager.Id && Players.Count>0)
            {
                ChanageManger(Players[0]);
            }
            RemovePlayer_SuccessEvent?.Invoke(this, new PlayerEventArgs(player_));
            CheckFull();
        }
        /// <summary>
        /// 玩家被踢离开房间
        /// </summary>
        /// <param name="id">玩家Id</param>
        public void RemovePlayerById(int id)
        {
           var player= Players.Find(p => p.Id == id);
            RemovePlayer(player);
         
        }
        /// <summary>
        /// 房间易主
        /// </summary>
        /// <param name="player">新主人</param>
        public void ChanageManger(IPlayerJoinRoom player_)
        {
            RoomManager = player_;
            ManagerChangedHandler?.Invoke(player_, new PlayerEventArgs(player_));
        }
        /// <summary>
        /// 不让玩家进入房间
        /// </summary>
        public void Close()
        {
            IsOpening = false;
        }
        /// <summary>
        /// 延长房间分钟数
        /// </summary>
        /// <param name="minute_"></param>
        public void ProlongTimelimit(int minute_)
        {
            if (minute_ <= 0)
            {
                throw new Exception("延长时间应正数");
            }
            Timelimit = Timelimit.AddMinutes(minute_);
        }
        /// <summary>
        /// 房间加密
        /// </summary>
        /// <param name="secretKey_"></param>
        public void Encrypt(string secretKey_)
        {
            if (secretKey_.Length == 0)
            {
                throw new Exception("密码不能为空");
            }
            SecretKey = secretKey_;
        }
        /// <summary>
        /// 设置人数上线
        /// </summary>
        /// <param name="count_"></param>
        public void SetPlayerCountTopLimit(int count_)
        {
            if (count_ < 1)
            {
                throw new Exception("房间人数上限不能小于1");
            }
            PlayerCountTopLimit = count_;
        }
        /// <summary>
        /// 允许进入房间
        /// </summary>
        public void Open()
        {
            IsOpening = true;
        }
        /// <summary>
        /// 取消房间密码
        /// </summary>
        public virtual void DeEncrypt()
        {
            SecretKey = "";
        }
        /// <summary>
        /// 设置房间门票
        /// </summary>
        /// <param name="price_"></param>
        public virtual void SetTicketPrice(decimal price_)
        {
            TicketPrice = price_;
        }
        /// <summary>
        /// 更换游戏
        /// </summary>
        /// <param name="game">游戏</param>
        public void ChangeGame(IGameProject game)
        {
            if (InningGame.IsStarted)
            {
                throw new Exception("游戏已经开始了,不能更换");
            }
            if (!(game.PlayerCountLeast <= PlayerCountTopLimit
        && PlayerCountTopLimit <= game.PlayerCountLimit))
            {
                PlayerCountTopLimit = game.PlayerCountLimit;
            }
            InningGame.IGameProject= game;
        }
    }
}
