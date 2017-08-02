using AntDesigner.NetCore.GameCity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameThreePokers {
    public class FreshSeatIco : WebsocketSendObjctBase {
        int PlayerCount { get; set; }
        public FreshSeatIco(int playerId, string clientMethodName="FreshSeatIco") : base(playerId, clientMethodName) {
            PlayerCount = 1;
        }
    }
}
