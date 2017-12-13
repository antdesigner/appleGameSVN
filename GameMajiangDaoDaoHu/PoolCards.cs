using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu 
{
    /// <summary>
    /// 出牌池
    /// </summary>
   internal class PoolCards
    {
        List<PutOutCard> PutOutCards { get; set; }
        internal PoolCards() {
            PutOutCards = new List<PutOutCard>();
        }
    }
}
