using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameCitys.Tools
{
    public static class ToolsData
    {
    /// <summary>
    //日期段转换
    /// </summary>
    /// <param name="fromDateStr">开始日期</param>
    /// <param name="toDateStr">结束日期</param>
    /// <param name="fromDate">开始日期</param>
    /// <param name="toDate">结束日期</param>
       public static void DataFromToOfStrToDateTime(string fromDateStr, string toDateStr, out DateTime fromDate, out DateTime toDate)
        {
            try
            {
                fromDate = Convert.ToDateTime(fromDateStr);
                if (toDateStr.Length == 0)
                {
                    toDate = DateTime.Now;
                    return;
                }
                toDate = Convert.ToDateTime(toDateStr);
            }
            catch (Exception)
            {
                DateTime today = DateTime.Now.Date;
                fromDate = today;
                toDate = today.AddDays(1);
            }
        }
    }
}
