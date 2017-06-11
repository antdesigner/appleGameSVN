using System;

namespace AntDesigner.NetCore.GameCity
{
    /// <summary>
    /// 房间配置
    /// </summary>
    public interface IRoomConfig
    {
        /// <summary>
        /// 房间名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 满员
        /// </summary>
        bool IsFull { get; set; }
        /// <summary>
        /// 开放
        /// </summary>
        bool IsOpening { get; set; }
        /// <summary>
        /// 参与随机
        /// </summary>
        bool IsRandom { get; set; }
        /// <summary>
        /// 人数上限
        /// </summary>
        int PlayerCountTopLimit { get; set; }
        /// <summary>
        /// 设置密码
        /// </summary>
        string SecretKey { get; set; }
        /// <summary>
        /// 门票价格
        /// </summary>
        decimal TicketPrice { get; set; }
        /// <summary>
        /// 到期时间
        /// </summary>
        DateTime Timelimit { get; set; }
        /// <summary>
        /// 公告
        /// </summary>
        string Affiche { get; set; }
        /// <summary>
        /// 游戏的一局
        /// </summary>
        IInningeGame InningGame { get; set; }
    }
}