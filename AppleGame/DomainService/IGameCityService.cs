using System.Collections.Generic;
using AntDesigner.GameCityBase;
using AntDesigner.NetCore.GameCity;

namespace GameCitys.DomainService
{
    public interface IGameCityService
    {
        void CreatGameCity(List<IGameCity> gameCityList, Player cityManager, string cityName = "默认游戏城");
    }
}