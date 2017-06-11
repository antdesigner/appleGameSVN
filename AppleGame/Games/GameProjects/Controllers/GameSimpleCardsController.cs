using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AntDesigner.GameCityBase.Controllers;
using AntDesigner.GameCityBase.interFace;
using Microsoft.AspNetCore.Http;
using GameCitys.DomainService;

namespace AppleGame.Games.SimpleCards.Controllers
{
    [Area("GameProjects")]
    public class GameSimpleCardsController : CityGameController
    {
        public GameSimpleCardsController( IHttpContextAccessor httpContextAccessor, IPlayerService playerService) : base( httpContextAccessor, playerService)
        {
        }
      
    }
}
