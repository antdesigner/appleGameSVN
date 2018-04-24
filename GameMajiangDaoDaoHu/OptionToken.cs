using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    internal class OptionToken {
        internal TakonType Type { get; set; }
        internal Card SnatchCard { get; set; }
        /// <summary>
        /// 抛出座位
        /// </summary>
        internal Seat EjectionSeat { get; set; }
        /// <summary>
        /// 抛出时间
        /// </summary>
        internal DateTime EjectionTime { get; set; }
        /// <summary>
        /// 颁发时间
        /// </summary>
        internal DateTime ConferTime { get; set; }

    }
}
