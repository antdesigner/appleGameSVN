using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntDesigner.GameCityBase.interFace
{
   public interface ILoginGame
    {
       Func<string, Player> DgetPlayerByWeixianName{ get; set; }
       Func<Player, Player> DaddPlayer { get; set; }
       Player Login(string name,string shareId);
    }
}
