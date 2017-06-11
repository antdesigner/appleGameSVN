using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AntDesigner.GameCityBase;
using Microsoft.AspNetCore.Http;
using AntDesigner.GameCityBase.interFace;
using GameCitys.DomainService;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AntDesigner.GameCityBase.Controllers
{
    public class NoticeController : MyController
    {
        public NoticeController( IHttpContextAccessor httpContextAccessor, IPlayerService playerService) : base( httpContextAccessor, playerService)
        { }
        // GET: /<controller>/
        public IActionResult Affiche([FromServices]INoticeService  noticeService)
        {
            IList<Notice> notices = noticeService.GetNotices(5);
            return View("noticesList",notices);

        }
    }
}
