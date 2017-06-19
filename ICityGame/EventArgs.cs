using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.GameCity
{
    public  class PlayerEventArgs: EventArgs
    {
        public IPlayerJoinRoom Player { get; set; }
        public PlayerEventArgs(IPlayerJoinRoom player)
        {
            Player = player;
        }

    }
    public class GameStopedEventArgs : EventArgs
    {
        public string Message { get; set; }
        public GameStopedEventArgs(string message)
        {
            Message = message;
        }

    }
}
