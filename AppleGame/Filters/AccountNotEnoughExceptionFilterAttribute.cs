using AntDesigner.NetCore.GameCity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntDesigner.GameCityBase
{
    public class AccountNotEnoughExceptionFilterAttribute: ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            AccountNotEnoughException myException = context.Exception as AccountNotEnoughException;
        
        }


    }
}
