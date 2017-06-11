using Microsoft.AspNetCore.Mvc;
using AntDesigner.GameCityBase.Controllers;
using Microsoft.AspNetCore.Http;
using AntDesigner.NetCore.GameCity;
using AppleGame.Games.Citys.Models;
using GameCitys.DomainService;
namespace AppleGame.Games.Citys.Controllers
{
    [Area("Citys")]
    public class CitysController : CityGameController
    {
        public CitysController( IHttpContextAccessor httpContextAccessor, IPlayerService playerService) : base( httpContextAccessor, playerService)
        {

        }
        /// <summary>
        /// 游戏城列表
        /// </summary>
        /// <returns>游戏城列表view</returns>
        public IActionResult Index()
        {
            return View(CityGameController.GameCityList);
        }
        /// <summary>
        /// 游戏城配置
        /// </summary>
        /// <param name="gameCityId">游戏城Id</param>
        /// <returns>配置View</returns>
        [HttpGet]
        public IActionResult ConfigGameCity(string gameCityId)
        {
            IGameCity gameCity = CityGameController.GameCityList.FindGameCityById(gameCityId);
            if (player.Id==gameCity.ICityManager.Id)
            {
                return View("ConfigGameCity", gameCity);
            }
            return View("ConfigGameCity",gameCity);
        }
        /// <summary>
        /// 配置游戏城
        /// </summary>
        /// <param name="configGameCity">配置model</param>
        /// <returns>游戏城列表</returns>
        [HttpPost]
        public IActionResult ConfigGameCity(ConfigGameCityView configGameCity)
        {
            string id_ = configGameCity.Id;
            IGameCity gameCity_ = CityGameController.GameCityList.FindGameCityById(id_);
            gameCity_.Name = configGameCity.Name;
            gameCity_.SecretKey = configGameCity.SecretKey;
            gameCity_.TicketPrice = configGameCity.TicketPrice;
            gameCity_.IsOpening = configGameCity.IsOpening;
            return RedirectToAction("Index");
        }
    }
}
