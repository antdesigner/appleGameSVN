using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.GameCity
{/// <summary>
/// 自定义错误
/// </summary>
    public class BaseException : Exception
    {
        public int PlayerId { get; }
        public BaseException(int playerId)
        {
            PlayerId = playerId;
        }
        public BaseException(int playerId, string message) : base(message)
        {
            PlayerId = playerId;
        }
        public BaseException(int playerId, string message, Exception inner) : base(message, inner)
        {
            PlayerId = playerId;
        }
    }
    /// <summary>
    /// 账户余额不足
    /// </summary>
    public class AccountNotEnoughException : BaseException
    {
        public AccountNotEnoughException(int playerId):base(playerId)
        { }
        public AccountNotEnoughException(int playerId, string message):base(playerId,message)
        { }
        public AccountNotEnoughException(int playerId,string message,Exception inner):base(playerId,message,inner)
        { }
    }
    /// <summary>
    /// 房间不存在
    /// </summary>
    public class RoomIsNotExistException:BaseException
    {
        public RoomIsNotExistException(int playerId):base(playerId)
        { }
        public RoomIsNotExistException(int playerId, string message):base(playerId,message)
        { }
        public RoomIsNotExistException(int playerId, string message, Exception inner):base(playerId,message,inner)
        { }

    }
    
}
