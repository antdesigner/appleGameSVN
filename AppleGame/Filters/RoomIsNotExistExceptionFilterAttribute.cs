using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AntDesigner.NetCore.GameCity;
using GameCitys.GamCityBase;

namespace AntDesigner.GameCityBase
{
    /// <summary>
    /// 房间不存在异常过滤器跳转到大厅
    /// </summary>
    public class RoomIsNotExistExceptionFilterAttribute:ExceptionFilterAttribute
    {
  
        public override void OnException(ExceptionContext context)
        {
            
            if (context.Exception.GetType()==typeof(RoomIsNotExistException))
            {
                context.Result = new RedirectToActionResult("RoomsList", "Rooms", new { Area = "Citys" });
            }
        
        }
    }
}
