using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppleGame.Games.Citys.Models
{
    public class ConfigRoomView
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
        public string  GameProjectName { get; set; }
        /// <summary>
        /// 公告
        /// </summary>
        public string Affiche { get; set; }
    }
}
