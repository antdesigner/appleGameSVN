using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    /// <summary>
    /// 游戏状态
    /// </summary>
    enum Stage
    {
        /// <summary>
        /// 游戏第一次启动前
        /// </summary>
        Waiting=1,
        /// <summary>
        /// 等待玩家准备
        /// </summary>
        Reading,
        /// <summary>
        /// 开始
        /// </summary>
        Running,
        /// <summary>
        /// 暂停
        /// </summary>
        Paused,
        /// <summary>
        /// 结算
        /// </summary>
        Computed
    }
}
