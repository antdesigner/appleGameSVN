using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntDesigner.GameCityBase.interFace;
using AntDesigner.NetCore.GameCity;
using AntDesigner.GameCityBase.Controllers;
using AntDesigner.GameCityBase;

namespace GameCitys.DomainService
{
    public class GameCityService : ABIStorehouse, IGameCityService
    {
        public GameCityService(IStorehouse istoreHose) : base(istoreHose)
        {
        }

        public void CreatGameCity(List<IGameCity> gameCityList, Player cityManager, string cityName = "游戏城")
        {

            ICityManager cityManager_;
            if (cityManager!=null)
            {
                cityManager_ = cityManager;
            }
            else
            {
                cityManager_ = ManagePlayer.GetOnlyInstance();
            }
            IGameCityConfig config_ = new GameCityConfig()
            {
                Name = cityName
            };
            IGameCity gameCity_ = new GameCity(cityManager_, config_);
            gameCityList.Add(gameCity_);
        }
    }
}
