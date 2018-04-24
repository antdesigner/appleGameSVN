using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    class OptionorRequestCollection
    {
        List<OptionorRequest> RequesetOptions;
        public OptionorRequestCollection() {
            RequesetOptions = new List<OptionorRequest>();
        }
        internal void Add(OptionorRequest optionorRequest) {
            RequesetOptions.Add(optionorRequest);
        }
        internal OptionorRequest GetFirstAsk() {
            if (RequesetOptions.Count>0) {
                RequesetOptions.OrderByDescending(o => o.Optionor.Option.Priority);
                return RequesetOptions[0];
            }
            return null;
        }
    }
}
