using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppleGame.Games.Citys.Models
{
    public class ConfigGameCityView
    {
        public  string Id { get; set; }
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
        public string Notic{ get; set; }
    }
}
