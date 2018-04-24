using System;
using System.Collections.Generic;
using System.Text;
using AntDesigner.NetCore.GameCity;

namespace AntDesigner.NetCore.Games.GameSimpleCards {

    public class Seat : GameCity.Seat {
        public ABCard GetPoker { get; set; }
        public ABCard PlayOutPokers { get; set; }
        public ABCard CompareResult { get; set; }
        public Seat(IInningeGame inningeGame) : base(inningeGame) {
        }
    }
}
