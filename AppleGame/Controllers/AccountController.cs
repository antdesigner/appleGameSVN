
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AntDesigner.GameCityBase.EF;
using Microsoft.AspNetCore.Http;
using AntDesigner.GameCityBase.interFace;
using AntDesigner.GameCityBase.Controllers;
using AntDesigner.GameCityBase;
using GameCitys.DomainService;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AntDesigner.GameCityBase.Controllers
{
    public class AccountController : MyController
    {
        //public AccountController(DataContext db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        //{

        //}
        public AccountController(IHttpContextAccessor httpContextAccessor,IPlayerService playerService) : base(httpContextAccessor,playerService)
        {

        }

        public IActionResult AccountDetail()
        {

            List<AccountDetail> accountDetails = player.GetAccountDetails().OrderByDescending(a=>a.Id).ToList();
            ViewBag.account = player.Account;
            return View("accountDetail",accountDetails);
        }

       
    }
}
