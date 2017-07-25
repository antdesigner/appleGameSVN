using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.GameCity {
  public  interface  IGameCityFactory
    {
       ISeat CreatSeat(IInningeGame inningeGame) ;
    }
}
