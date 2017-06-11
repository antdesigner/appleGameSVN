using AntDesigner.weiXinPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WxPayAPI;

namespace GameCitys.EF
{
    public interface IStoreHoseForWeixin
    {
        PayOrder FindPayOrder(string out_trade_no);
    }
}
