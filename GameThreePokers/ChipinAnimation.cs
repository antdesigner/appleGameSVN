using AntDesigner.NetCore.GameCity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameThreePokers {
    public class ChipinAnimation : WebsocketSendObjctBase {
        public decimal Amount { get; set; }
        public int ForPlayerId { get; set; }
        public EChipinType ChipinType { get; set; }
        public ChipinAnimation(int playerId,decimal amount,int forPlayerId,EChipinType chipinType=EChipinType.Nothing, string clientMethodName="ChipinAnimation") : base(playerId, clientMethodName) {
            ChipinType = chipinType;
            Amount = amount;
            ForPlayerId = forPlayerId;
        }
    }
}
