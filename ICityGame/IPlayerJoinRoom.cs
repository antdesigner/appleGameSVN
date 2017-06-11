using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.GameCity
{
    /// <summary>
    /// 进入房间的玩家接口
    /// </summary>
    public interface IPlayerJoinRoom
    {/// <summary>
     /// 玩家名称
     /// </summary>
        string WeixinName { get; set; }
        /// <summary>
        /// 玩家Id
        /// </summary>
        int Id { get; set; }
        /// <summary>
        /// 玩家余额
        /// </summary>
        decimal Account { get; set; }
        /// <summary>
        /// 玩家账户增加(-)减少(+)
        /// </summary>
        /// <param name="ticketPrice"></param>
        /// <returns></returns>
        bool DecutMoney(decimal ticketPrice_, string cause = "");
        /// <summary>
        /// 玩家账户是否足够支付
        /// </summary>
        /// <param name="ammount_">比较金额</param>
        /// <returns>YN</returns>
        bool AccountNotEnough(decimal ammount_);

    }
}
