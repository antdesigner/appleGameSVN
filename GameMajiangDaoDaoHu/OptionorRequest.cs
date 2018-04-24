using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    /// <summary>
    /// 客户端玩家请求的操作
    /// </summary>
    internal class OptionorRequest {
        Seat Seat { get; set; }
         internal Optionor Optionor { get; set; }
        internal OptionorRequest(Seat seat, Optionor optionor) {
            Seat = seat;
            Optionor = optionor;
        }
        void Do() {
            Seat.Do(Optionor);
        }

    }
}
