using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.GameCity
{
  public static class WebscoketSendObjs
    {
      public static FreshGameFace FreshGameFace(int playerId)
        {

            return new FreshGameFace(playerId);
        }
      public static Alert Alert(int playerId,string content)
        {

            return new Alert(playerId,content);
        }
        public static GameOver GameOver(int playerId)
        {
            return new GameOver(playerId);
        }
        public static Stoped Stoped(int playerId)
        {
            return new Stoped(playerId);
        }
        public static LeaveRoom LeaveRoom(int playerId)
        {
            return new LeaveRoom(playerId);
        }
    }
  public abstract  class WebsocketSendObjctBase
    {
        public string ClientMethodName { get; set; }
        public int PlayerId { get; set; }
        public WebsocketSendObjctBase(int playerId,string clientMethodName)
        {
            ClientMethodName = clientMethodName;
            PlayerId = playerId;
        }
    }
    public class Alert : WebsocketSendObjctBase
    {
       public string AlertContent { get; set; }
        public Alert(int playerId,string alertContent, string clientMethodName = "Alert") : base(playerId,clientMethodName)
        {
            AlertContent = alertContent;
        }
    }
    public class FreshGameFace : WebsocketSendObjctBase
    {

        public FreshGameFace(int playerId,string clientMethodName= "_gameFace.Fresh") : base(playerId,clientMethodName )
        {
   
          
        }
    }
    public class Stoped : WebsocketSendObjctBase
    {
        public Stoped(int playerId,string clientMethodName="Stoped") : base(playerId,clientMethodName)
        {
        }
    }
    public class GameOver : WebsocketSendObjctBase
    {
        public GameOver(int playerId,string clientMethodName="GameOver") : base(playerId,clientMethodName)
        {
        }
    }
    public class LeaveRoom : WebsocketSendObjctBase
    {
        public LeaveRoom(int playerId, string clientMethodName= "LeaveRoom") : base(playerId, clientMethodName)
        {
        }
    }
    public class RoomMessage : WebsocketSendObjctBase
    {
       public  string Name { get; set; }
       public  string Message { get; set; }
        public RoomMessage(int playerId, string clientMethodName="RoomMessage") : base(playerId, clientMethodName)
        {
        }
    }
}
