using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading;
using AntDesigner.NetCore.GameCity;
namespace GameCitys.GamCityBase
{
    public static class ClientWebsocketsManager
    {

        public static ClientWebsocketDictionary PlayerIdAndWebsockets { get; private set; }
        static ClientWebsocketsManager()
        {
            PlayerIdAndWebsockets = new ClientWebsocketDictionary();
        }
        public static void Add(int? playerId, WebSocket websocket)
        {
            PlayerIdAndWebsockets.Add(playerId, websocket);
        }
        public static WebSocket FindClientWebSocketByPlayerId(int? id)
        {
            WebSocket websocket = PlayerIdAndWebsockets.FirstOrDefault(p => p.Key == id).Value;
            return websocket;
        }
        public static void Send(WebsocketSendObjctBase obj)
        {
            string sendjsonStr = Tools.ToolsSerialize.SerializeObjectToJson(obj);
            if (obj.PlayerId == 0)
            {
                foreach (var item in PlayerIdAndWebsockets.Values)
                {
                    if (item.State == WebSocketState.Open)
                    {
                        item.SendAsync(new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(sendjsonStr)),
                    WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
                return;
            }
            WebSocket websocket = FindClientWebSocketByPlayerId(obj.PlayerId);
            if (websocket.State == WebSocketState.Open)
            {
                websocket.SendAsync(new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(sendjsonStr)),
                WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
    public class ClientWebsocketDictionary : Dictionary<int?, WebSocket>
    {
        public new void Add(int? playerId, WebSocket websocket)
        {
            if (playerId == null)
            {
                throw new Exception("用户Id为空");
            }
            if (this.ContainsKey((int)playerId))
            {
                this.Remove(playerId);
            }
            base.Add(playerId, websocket);
        }
    }
}
