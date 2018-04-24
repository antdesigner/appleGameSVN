using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameThreePokers {
    /// <summary>
    /// 专用来生成客户端接受的json对象
    /// </summary>
    class Poker {
        public string CardColor { get; set; }
        public string Name { get; set; }
        public int  ComparedValue{get;set;}
        public Poker() {
            CardColor = "";
            Name = "";
            ComparedValue = -1;
        }
        public  Poker(string cardColor,string name,int comparedValue) {
            CardColor = cardColor;
            Name = name;
            ComparedValue = comparedValue;
        }
    }
}
