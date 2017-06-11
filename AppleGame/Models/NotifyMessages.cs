using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntDesigner.GameCityBase
{/// <summary>
/// 调用websock通知客户端协议
/// </summary>
    public static class NotifyMessages
    {
      public   const string AccountNotEnough = "AccountNotEnough";
      public   const string RoomIsNotExist = "RoomIsNotExist";
      public const string GameOver = "GameOver";
      public const string Stopted = "Stopted";
    }
}
