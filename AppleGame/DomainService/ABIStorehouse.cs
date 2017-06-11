using AntDesigner.GameCityBase.EF;
using AntDesigner.GameCityBase.interFace;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameCitys.DomainService
{
    public abstract class ABIStorehouse
    {
      protected  IStorehouse _storeHouse;
    
      public ABIStorehouse(IStorehouse istoreHose)
        {
            _storeHouse = istoreHose;
         
        }

    }
}
