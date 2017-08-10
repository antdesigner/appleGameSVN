using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameThreePokers {
    public enum EChipinType
    {
        Nothing,
        NoLook,//暗注
        Follow,//跟注
        Double,//加倍
        ChipIn,//加注
        Limit,//直封
        Compare //开牌
        PlayerChipIn,//打底
        GaveUp//放弃
    }
}
