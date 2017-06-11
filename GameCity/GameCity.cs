using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.GameCity
{
    public class GameCityConfig : IGameCityConfig
    {
        /// <summary>
        /// 游戏城名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 开房价格
        /// </summary>
        public decimal TicketPrice { get; set; }
        /// <summary>
        /// 进入密码
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// 开放?
        /// </summary>
        public bool IsOpening { get; set; }
        /// <summary>
        /// 公告
        /// </summary>
        public INotice INotic { get; set; }
        /// <summary>
        /// 开房验票委托
        /// </summary>
        public Func<IJoinGameCityTicket, bool> DCheckTicket { get; set; }
        /// <summary>
        /// 委托检查能否添加房间
        /// </summary>
        public Func<IRoom, bool> DAddRoomChek { get; set; }

        public GameCityConfig()
        {
            Name = "游戏城";
            TicketPrice = 0;
            SecretKey = "";
            IsOpening = true;

        }
    }
    public class GameCity:IGameCity
    {
        /// <summary>
        /// 游戏城Id
        /// </summary>
        public string Id { get; private set; }
        /// <summary>
        /// 游戏城名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 禁止通过改属性直接删除房间
        /// </summary>
        public List<IRoom> Rooms { get; private set; }
        /// <summary>
        /// 管理员
        /// </summary>
        public ICityManager ICityManager { get; private set; }
        /// <summary>
        /// 开房价格
        /// </summary>
        public decimal TicketPrice { get; set; }
        /// <summary>
        /// 进入密码
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// 开放?
        /// </summary>
        public bool IsOpening { get; set; }
        /// <summary>
        /// 公告
        /// </summary>
        public INotice INotic { get; set; }
        /// <summary>
        /// 检查票据委托
        /// </summary>
        public Func<IJoinGameCityTicket, bool> DCheckTicket { get; set; }
        /// <summary>
        /// 检查能否田间房间的委托
        /// </summary>
        public Func<IRoom,bool> DAddRoomChek { get; set; }
        /// <summary>
        /// 删除房间前事件
        /// </summary>
        public event EventHandler BeforDeleteRoomHandler;
        /// <summary>
        /// 添加房间后事件
        /// </summary>
        public event EventHandler AfterAddRoomHandler;
        /// <summary>
        /// 开房钱发现钱不够事件
        /// </summary>
        public event EventHandler FailAddRoomNotEnoughMoney;
        /// <summary>
        /// 玩家开房不成功执行
        /// </summary>
        public Action<IRoom> DPlayCreateRoomFail { get; set; }
        /// <summary>
        /// 初始化房间容器和游戏城Id
        /// </summary>
        private GameCity()
        {
            Id = PublicObject.GenerateTimeStamp();
            Rooms = new List<IRoom>();
        }
        /// <summary>
         /// 游戏城
         /// </summary>
         /// <param name="cityManager_">管理员</param>
         /// <param name="config_">配置</param>
        public GameCity(ICityManager cityManager_, IGameCityConfig config_) : this()
        {
            Name = config_.Name;
            ICityManager = cityManager_;
            TicketPrice = config_.TicketPrice;
            SecretKey = config_.SecretKey;
            IsOpening = config_.IsOpening;
            DAddRoomChek = config_.DAddRoomChek;
        }
        /// <summary>
        /// 游戏城
        /// </summary>
        /// <param name="name_">名称</param>
        /// <param name="cityManager_">管理员</param>
        /// <param name="ticketPrice_">开房价格</param>
        /// <param name="secretKey_">进入密码</param>
        /// <param name="isOpening">是否开放</param>
        public GameCity(string name_, ICityManager cityManager_,
            decimal ticketPrice_ = 0, string secretKey_ = "", bool isOpening = true) : this()
        {
            Name = name_;
            ICityManager = cityManager_;
            TicketPrice = ticketPrice_;
            SecretKey = secretKey_;
            IsOpening = isOpening;
        }
        /// <summary>
        /// 玩家进入游戏城验票
        /// </summary>
        /// <param name="ticket_">门票(用于验票)</param>
        /// <returns></returns>
        public bool AddPlayer(IJoinGameCityTicket ticket_)
        {
            bool IsSuccess = false;
            if (CheckTicket(ticket_))
            {
                IsSuccess = true;
            }
            return IsSuccess;
        }
        /// <summary>
         /// 验票
         /// </summary>
         /// <param name="ticket_">验票委托</param>
         /// <returns></returns>
        bool CheckTicket(IJoinGameCityTicket ticket_)
        {
            bool allowJoin = true;
            if (DCheckTicket!= null)
            {
                allowJoin = DCheckTicket(ticket_);
            }
            return allowJoin;
        }
        /// <summary>
        /// 添加房间
        /// </summary>
        /// <param name="room_">房间</param>
        /// <returns>成功?</returns>
        private bool MyAddRoom(IRoom room_)
        {
            if (room_.RoomManager.Account < TicketPrice)
            {
                FailAddRoomNotEnoughMoney?.Invoke(this, new EventArgs());
                return false;
            }
            if (TicketPrice > 0 && !room_.RoomManager.DecutMoney(TicketPrice))
            {
                return false;
            }
            Rooms.Add(room_);
            room_.GameCityId = this.Id;
            return true;
        }
        /// <summary>
        /// 添加房间
        /// </summary>
        /// <param name="room_">房间</param>
        /// <returns>成功?</returns>
        public bool AddRoom(IRoom room_)
        {
            if (CheckAddRoom(room_))
            {
                if (MyAddRoom(room_))
                {
                    AfterAddRoomHandler?.Invoke(this, new EventArgs());
                    return true;
                }
            }
            DPlayCreateRoomFail?.Invoke(room_);
            return false;
        }
        private bool CheckAddRoom(IRoom Room_)
        {
            if (DAddRoomChek!=null)
            {
                return DAddRoomChek(Room_);
            }
            return true;
        }
        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="room_">房间</param>
        /// <returns>成功?</returns>
        public bool RemoveRoom(IRoom room_)
        {
            if (CheckDeleteRoom(room_))
            {
                BeforDeleteRoomHandler?.Invoke(this, new EventArgs());
                if (MyDeleteRoom(room_))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="room_">房间</param>
        /// <returns>成功?</returns>
        private bool MyDeleteRoom(IRoom room_)
        {
            try
            {
                Rooms.Remove(room_);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
               
         
        }
        private bool CheckDeleteRoom(IRoom room_)
        {
            if (room_.DCanDeleteRoomCheck!=null)
            {
                return room_.DCanDeleteRoomCheck(room_);
            }
            return true;
        }
        private IRoom RoomCreator(IInningeGame inningGame_, int playerCount_)
        {
            Room room = new Room(DateTime.Now.AddHours(999999), playerCount_, inningGame_, ICityManager);
            return room;
        }
        /// <summary>
        /// 游戏城自动生成房间
        /// </summary>
        /// <param name="inningGame_">游戏</param>
        /// <param name="playerCount_">房间人数上限</param>
        /// <param name="n">生成个数</param>
        public void AddAutoRoom(IInningeGame inningGame_, int playerCount_, int n)
        {
            for (int i = 0; i < n; i++)
            {
                Rooms.Add(RoomCreator(inningGame_, playerCount_));
            }

        }
        /// <summary>
        /// 返回指定Id的房间
        /// </summary>
        /// <param name="id_">房间id</param>
        /// <returns>房间</returns>
        public IRoom FindRoomById(string id_)
        {
            try
            {
                return Rooms.Find(r => r.Id == id_);
            }
            catch (Exception)
            {
                return null;
            }

        }
        /// <summary>
        /// 随机提供免费房间
        /// </summary>
        /// <returns>免费房间</returns>
        public IRoom ProvideRandomFreeRoom()
        {

            Random random = new Random();

            if (Rooms.FindAll(r => r.Players.Count < r.PlayerCountTopLimit && r.TicketPrice == 0).Count > 0)
            {
                int index;
                index = random.Next(Rooms.Count);
                return Rooms[index];
            }
            return null;
        }
        /// <summary>
        /// 随机提供一个房间
        /// </summary>
        /// <returns>随机房间</returns>
        public IRoom ProvideRandomRoom()
        {
            Random random = new Random();
            if (Rooms.Count > 0)
            {
                int index;
                index = random.Next(Rooms.Count);
                return Rooms[index];
            }
            return null;
        }

        public IList<IRoom> FindRoomsByName(string name)
        {
            try
            {
                return Rooms.FindAll(r => r.Name == name);
            }
            catch (ArgumentNullException)
            {

                return null;
            }
        }
    }

}
