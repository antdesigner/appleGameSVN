using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    class OptionGetCard : IOption {
        public string Name { get; }
        public decimal Priority { get; }
        public bool Do(HandCardManager handcards, CardModel card) {
            throw new NotImplementedException();
        }
        public OptionGetCard(decimal priority=0,string name = "摸") {
            Name = name;
            Priority = priority;
        }
    }
}
