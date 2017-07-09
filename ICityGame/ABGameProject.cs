using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using System.Diagnostics;
using AntDesigner.NetCore.GameCity;
namespace AntDesigner.NetCore.GameCity
{
    /// <summary>
    /// 游戏基类
    /// </summary>
  public   abstract  class ABGameProject
    {
        /// <summary>
        /// 游戏名称
        /// </summary>
        public string ShowName { get; set; }
        /// <summary>
        /// 反射加载名称(和Games区域文件夹对应的控制器和view文件夹名称必须一致)
        /// </summary>
        public string Name { get;protected set; }
        /// <summary>
        /// 人数下限
        /// </summary>
        public int PlayerCountLeast { get; set; }
        /// <summary>
        /// 人数上限
        /// </summary>
        public int PlayerCountLimit { get; set; }
        /// <summary>
        /// 游戏本局数据
        /// </summary>
        public IInningeGame InningeGame { get; set; }
        /// <summary>
        ///委托websocket方式发送给客户端数据
        /// </summary>

        public virtual Action<WebsocketSendObjctBase> Notify { get; set; }
        /// <summary>
        ///委托websocket方式发送给客户端数据
        /// </summary>
        public virtual Action<WebsocketSendObjctBase,object> NotifyByWebsockLink { get; set; }
        public Func <string,decimal,string,decimal> DChangePlayerAccount { get; set; }
        public ABGameProject()
        {
            
        }
        /// <summary>
        /// 接送客户端请求并调用请求方法返回json格式
        /// </summary>
        /// <param name="askMethodName"></param>
        /// <param name="jsonInfo"></param>
        /// <returns></returns>
        public virtual  string ClinetHandler(string askMethodName, Dictionary<string, string> methodParams)
        {
            Type myType_ = this.GetType();
         
            MethodInfo methodInfo_ = myType_.GetMethod(askMethodName);
            ParameterInfo[] paramsInfo = methodInfo_.GetParameters();
            if (methodParams.Count != paramsInfo.Length && !methodParams.ContainsKey("playerId"))
            {
                throw new Exception("客户端提供的"+ askMethodName + "_方法参数和调用的服务端方法参数数量不相等");
            }
            object[] params_ = new object[paramsInfo.Length];
           
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Type paramType = paramsInfo[i].ParameterType;
               string paramName = paramsInfo[i].Name;
                if (methodParams.ContainsKey(paramName))
                {
                    if (!methodParams.TryGetValue(paramName, out string paramValue))
                    {
                        throw new Exception("客户端提供的" + paramName + "方法参数绑定失败");
                    }
                    if (paramType== typeof(string))
                    {
                        params_[i] = paramValue;
                    }
                    else {
                        params_[i] = JsonConvert.DeserializeObject(paramValue, paramType);
                    }
                    
                }

        

            }
            var obj = methodInfo_.Invoke(this, params_);
            var jsonResult = JsonConvert.SerializeObject(obj);

            return jsonResult;
        }
        /// <summary>
        /// 检查能不能开始
        /// </summary>
        /// <param name="innineGame_">本局游戏</param>
        /// <returns>YN</returns>
        public virtual bool CheckStart(IInningeGame innineGame_)
        {
            int playerOnSeatCopunt = innineGame_.NotEmptySeats().Count;
            if (playerOnSeatCopunt < innineGame_.IGameProject.PlayerCountLeast || playerOnSeatCopunt > PlayerCountLimit)
            {
                //Notify?.Invoke(WebscoketSendObjs.RoomMessage(0, "人数不足,不能启动游戏"));
                NotifyRoomPlayers(WebscoketSendObjs.RoomMessage(0, "人数不足,不能启动游戏"));
                return false;
            }

            return true;
        }
        /// <summary>
        /// 可以开始游戏前事件处理
        /// </summary>
        /// <param name="inningeGame">本局游戏</param>
        /// <param name="e"></param>
        public virtual  void BeforGameStart(object inningeGame, EventArgs e)
        {

        }
        /// <summary>
        /// 开始游戏处理事件
        /// </summary>
        /// <param name="sender">IInngineGame</param>
        /// <param name="e"></param>
        public virtual void GameStart(object inngineGame, EventArgs e)
        {
            InningeGame = (IInningeGame)inngineGame;
           // Notify?.Invoke(WebscoketSendObjs.RoomMessage(0, "游戏开始了!"));
            NotifyRoomPlayers(WebscoketSendObjs.RoomMessage(0, "游戏开始了!"));
        }
        /// <summary>
        /// 添加座位检查
        /// </summary>
        /// <param name="innineGame_">本局游戏</param>
        /// <returns>YN</returns>
        public virtual bool CheckAddSeat(IInningeGame innineGame_)
        {
            if (innineGame_.IsStarted)
            {
                return false;
            }
            if (innineGame_.SeatCount >= innineGame_.IGameProject.PlayerCountLimit)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 添加座位前事件处理
        /// </summary>
        /// <param name="inningeGame">本局游戏</param>
        /// <param name="e"></param>
        public virtual void BeforAddSeat(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 添加座位后事件处理
        /// </summary>
        /// <param name="inningeGame">本局游戏</param>
        /// <param name="e"></param>
        public virtual  void AfterAddSeat(object inningeGame, EventArgs e)
        {

        }
        /// <summary>
        /// 玩家能否坐下
        /// </summary>
        /// <param name="inningeGame">一局游戏</param>
        /// <returns></returns>
        public virtual bool CheckSitDown(IInningeGame inningeGame)
        {
            return true;
        }
        /// <summary>
        /// 玩家坐下前事件处理
        /// </summary>
        /// <param name="inningeGame">本局游戏</param>
        /// <param name="e"></param>
        public virtual void BeforSitDown(object inningeGame, EventArgs e)
        { }
        /// <summary>
        /// 玩家坐下后事件处理
        /// </summary>
        /// <param name="inningeGame"></param>
        /// <param name="e"></param>
        public virtual void AfterSitDown(object inningeGame, EventArgs e)
        {
            var roomMessage = WebscoketSendObjs.RoomMessage(0, "有玩家进入");
            NotifyRoomPlayers(roomMessage);
        }
        /// <summary>
        /// 玩家离开座位前事件出路
        /// </summary>
        /// <param name="inningeGame"></param>
        /// <param name="e"></param>
        public virtual void BeforPlayerLeave(object inningeGame, EventArgs e)
        { }
        /// <summary>
        /// 玩家离开座位后事件处理
        /// </summary>
        /// <param name="inningeGame"></param>
        /// <param name="e"></param>
        public virtual void AfterPlayerLeave(object inningeGame, EventArgs e)
        {
            string playerId = ((PlayerEventArgs)e).Player.Id.ToString();
            var roomMessage = WebscoketSendObjs.RoomMessage(0, "玩家" + playerId + "离开了");
            NotifyRoomPlayers(roomMessage);
        }
        /// <summary>
        /// 游戏异常中断
        /// </summary>
        /// <param name="inningeGame">本局游戏</param>
        /// <param name="e"></param>
        public   virtual   void Stoped(object inningeGame, EventArgs e)
        {
            var myE = (GameStopedEventArgs)e;
            NotifyRoomPlayers(WebscoketSendObjs.Stoped(0, myE.Message));
        }
        /// <summary>
        /// 游戏正常结束
        /// </summary>
        /// <param name="inningeGame">本局游戏</param>
        /// <param name="e"></param>
        public  virtual  void GameOver(object inningeGame, EventArgs e)
        {
            NotifyRoomPlayers(WebscoketSendObjs.GameOver(0));
        }
        /// <summary>
        /// 添加座位到游戏最低数
        /// </summary>
        /// <param name="innineGame_"></param>
        private void AddSeatToLeastCount(IInningeGame innineGame_)
        {
            if (innineGame_.SeatCount < PlayerCountLeast)
            {
                innineGame_.AddSet(PlayerCountLeast - innineGame_.SeatCount);
            }
        }
        /// <summary>
        /// 重置游戏,以便开始新的一局
        /// </summary>
        public virtual void ResetGame(object inningeGame, EventArgs e)
        {
           // Notify?.Invoke(WebscoketSendObjs.ResetGame(0));
            NotifyRoomPlayers(WebscoketSendObjs.ResetGame(0));
        }

        protected  void NotifyRoomPlayers(WebsocketSendObjctBase websocketSendObjctBase)
        {
            
            foreach (IPlayerJoinRoom item in InningeGame.IRoom.Players)
            {
                if (null!= item.WebSocketLink)
                {
                    NotifyByWebsockLink?.Invoke(websocketSendObjctBase, item.WebSocketLink);
                }
               
            }
        }
        protected void NotifySinglePlayer(WebsocketSendObjctBase websocketSendObjctBase,int playerId)
        {
            var myPlayer = InningeGame.IRoom.Players.Find(p => p.Id == playerId);
            NotifyByWebsockLink?.Invoke(websocketSendObjctBase, myPlayer.WebSocketLink);
        }
    }
}
