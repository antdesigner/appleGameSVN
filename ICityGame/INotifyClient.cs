using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.GameCity
{
    /// <summary>
    /// 委托通知玩家客户端接口
    /// </summary>
   public  interface INotifyClient
    {
        /// <summary>
        /// 委托通知玩家客户端
        /// </summary>
       // Action<int?, string> Notify { get; set; }
        Action<WebsocketSendObjctBase> Notify { get; set; }
       Action<WebsocketSendObjctBase, object> NotifyByWebsockLink { get; set; }
       
    }
}
