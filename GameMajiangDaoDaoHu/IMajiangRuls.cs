using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu 
{
    /// <summary>
    /// 各种麻将的规则
    /// </summary>
  public interface IMajiangRuls
    {
        /// <summary>
        /// 要牌规则
        /// </summary>
        /// <param name="handCards"></param>
        /// <returns></returns>
      OptionorCollection FreshOptions(HandCardManager handCards);
      MaJiangCollection GetHuCards(MaJiangCollection maJiangs);
      Optionor GetOptionorByName(string name,CardModel cardM);
    }
}
