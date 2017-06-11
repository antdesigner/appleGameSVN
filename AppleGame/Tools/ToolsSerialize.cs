using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameCitys.Tools
{
    public static class ToolsSerialize
    {
        public static string SerializeObjectToJson(Object obj)
        {

            string hitBoxsCollectionJosnarry = JsonConvert.SerializeObject(obj);
            return hitBoxsCollectionJosnarry;

        }
    }
}
