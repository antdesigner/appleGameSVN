using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameThreePokers {
    public enum EStage{
        Reading,//开始第一局游戏前
        CanChipIning,//押底阶段
        Running,//
        Computed//比牌后
    }
}
