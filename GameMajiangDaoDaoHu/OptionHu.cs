using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu 
{
      class OptionHu : IOption {
        public string Name { get; }

        public decimal Priority { get; }

        public bool Do(HandCardManager  handCards, Card card) {

            return false;
        }

        public bool Do(HandCardManager handcards, CardModel card) {
            throw new NotImplementedException();
        }

        public OptionHu(decimal priority=8,string name = "胡") {
            Name = name;
            Priority = priority;
        }
    }
}
