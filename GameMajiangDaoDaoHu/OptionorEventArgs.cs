using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
  internal  class OptionorEventArgs:EventArgs
    {
        internal Optionor Optionor { get; set; }
       internal OptionorEventArgs(Optionor optionor ) {
            Optionor = optionor;
        }
    }
}
