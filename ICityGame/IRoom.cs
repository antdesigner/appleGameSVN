using System;
using System.Collections.Generic;

namespace AntDesigner.NetCore.GameCity
{
    /// <summary>
    /// 游戏房间接口
    /// </summary>
    public interface IRoom
    {
        /// <summary>
        /// 房间公告
        /// <summary>
        /// 房间Id
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// 房间公告
        /// </summary>
        string Affiche { get; set; }
        /// <summary>
        /// 房间当前游戏
        /// </summary>
        IInningeGame InningGame { get; }
        /// <summary>
        /// 房间人数满了
        /// </summary>
        bool IsFull { get; }
        /// <summary>
        /// 允许进入?
        /// </summary>
        bool IsOpening { get; }
        /// <summary>
        /// 是否参与随机
        /// </summary>
        bool IsRandom { get; set; }
        /// <summary>
        /// 房间名字
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 人数上限
        /// </summary>
        int PlayerCountTopLimit { get; }
        /// <summary>
        /// 房间玩家
        /// </summary>
        List<IPlayerJoinRoom> Players { get; }
        /// <summary>
        /// 房主
        /// </summary>
        IPlayerJoinRoom RoomManager { get; }
        /// <summary>
        /// 房间密码
        /// </summary>
        string SecretKey { get; }
        /// <summary>
        /// 进入房间门票价格
        /// </summary>
        decimal TicketPrice { get; }
        /// <summary>
        /// 房间到期时间
        /// </summary>
        DateTime Timelimit { get; }
        /// <summary>
        /// 房间属于的游戏城id 
        /// </summary>
        string GameCityId { get; set; }
        
         Func<IRoom,bool> DCanDeleteRoomCheck { get; set; }
       
        /// <summary>
        /// 玩家成功进入房间事件
        /// </summary>
        event EventHandler AddPlayer_SuccessEvent;
        /// <summary>
        /// 玩家离开房间事件
        /// </summary>
        event EventHandler RemovePlayer_SuccessEvent;
        /// <summary>
        /// 玩家进入房间失败后事件
        /// </summary>
        event EventHandler AddPlayerFailRoomFullEvent;
        /// <summary>
        /// 玩家不能支付门票事件
        /// </summary>
        event EventHandler PlayCanNotPayTicketEvent;
        /// <summary>
        /// 房主易主后事件
        /// </summary>
        event EventHandler ManagerChangedHandler;
        /// <summary>
        /// 添加玩家入房间
        /// </summary>
        /// <param name="player_">玩家</param>
        /// <returns></returns>
        bool AddPlayer(IPlayerJoinRoom player_);
        /// <summary>
        /// 房间易主
        /// </summary>
        /// <param name="player_"></param>
        void ChanageManger(IPlayerJoinRoom player_);
        /// <summary>
        /// 改变公告
        /// </summary>
        /// <param name="affice_"></param>
        void ChangeAffice(string affice_);
        /// <summary>
        /// 房间关闭
        /// </summary>
        void Close();
        /// <summary>
        /// 取消房间密码
        /// </summary>
        void DeEncrypt();
        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="secretKey_"></param>
        void Encrypt(string secretKey_);
        /// <summary>
        /// 允许进入
        /// </summary>
        void Open();
        /// <summary>
        /// 延长时间
        /// </summary>
        /// <param name="minute_">分钟</param>
        void ProlongTimelimit(int minute_);
        /// <summary>
        /// 踢出玩家
        /// </summary>
        /// <param name="player_"></param>
        void RemovePlayer(IPlayerJoinRoom player_);
        /// <summary>
        /// 从房间移除玩家
        /// </summary>
        /// <param name="id">玩家Id</param>
        void RemovePlayerById(int id);
        /// <summary>
        /// 设置房间人数上限
        /// </summary>
        /// <param name="count_">人数</param>
        void SetPlayerCountTopLimit(int count_);
        /// <summary>
        /// 设置门票价格
        /// </summary>
        /// <param name="price_">房间门票价格</param>
        void SetTicketPrice(decimal price_);
        /// <summary>
        /// 更改房间游戏
        /// </summary>
        /// <param name="game"></param>
        void ChangeGame(IGameProject game);
    }
}