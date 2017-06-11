using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameCitys.DomainService
{
    public class MyServiceCollection: ServiceCollection,IServiceCollection
    {
       public  MyServiceCollection()
        {
           
        }
    }
}
