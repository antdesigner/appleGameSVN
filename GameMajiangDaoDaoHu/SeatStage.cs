using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    /// <summary>
    /// 座位状态
    /// </summary>
   enum SeatStage
    {
        /// <summary>
        /// 初始状态
        /// </summary>
        Starting=1,
        /// <summary>
        ///准备
        /// </summary>
        Reading,
        /// <summary>
        /// 等待
        /// </summary>
        Waiting,
        /// <summary>
        /// 竞牌,玩家出牌后,考虑要不要
        /// </summary>
        Challenging,
        /// <summary>
        /// 出牌
        /// </summary>
        Paying,
        /// <summary>
        /// 出牌后
        /// </summary>
        Payed,
        /// <summary>
        /// 暂停
        /// </summary>
        Paused


    }
}
