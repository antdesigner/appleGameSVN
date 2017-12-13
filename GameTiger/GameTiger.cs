using AntDesigner.GameCityBase.boxs;
using AntDesigner.NetCore.GameCity;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AntDesigner.NetCore.Games.GameTiger
{
    public class GameTiger:ABGameProject, IGameProject
    {
 
        public GameTiger()
        {
            ShowName = "苹果机";
            Name = this.GetType().Name;
            PlayerCountLimit = 1;
            PlayerCountLeast = 1;

        }
        /// <summary>
        /// 检查能不能开始
        /// </summary>
        /// <param name="innineGame_">本局游戏</param>
        /// <returns>YN</returns>
        public override bool CheckStart(IInningeGame innineGame_)
        {
            int playerOnSeatCopunt = innineGame_.NotEmptySeats().Count;
            if (playerOnSeatCopunt < innineGame_.IGameProject.PlayerCountLeast || playerOnSeatCopunt > PlayerCountLimit)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 可以开始游戏前事件处理
        /// </summary>
        /// <param name="inningeGame">本局游戏</param>
        /// <param name="e"></param>
        public override void BeforGameStart(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="sender">IInngineGame</param>
        /// <param name="e"></param>
        public override void GameStart(object sender, EventArgs e)
        {
            InningeGame = (IInningeGame)sender;
            player =InningeGame.IRoom.RoomManager;

        }
        /// <summary>
        /// 添加座位检查
        /// </summary>
        /// <param name="innineGame_">本局游戏</param>
        /// <returns>YN</returns>
        public override bool CheckAddSeat(IInningeGame innineGame_)
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
        public override void BeforAddSeat(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 添加座位后事件处理
        /// </summary>
        /// <param name="inningeGame">本局游戏</param>
        /// <param name="e"></param>
        public override  void AfterAddSeat(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 玩家能否坐下
        /// </summary>
        /// <param name="inningeGame">一局游戏</param>
        /// <returns></returns>
        public override bool CheckSitDown(IInningeGame inningeGame)
        {
            return true;
        }
        /// <summary>
        /// 玩家坐下前事件处理
        /// </summary>
        /// <param name="inningeGame">本局游戏</param>
        /// <param name="e"></param>
        public override void BeforSitDown(object inningeGame, EventArgs e)
        { }
        /// <summary>
        /// 玩家坐下后事件处理
        /// </summary>
        /// <param name="inningeGame"></param>
        /// <param name="e"></param>
        public override void AfterSitDown(object inningeGame, EventArgs e)
        { }
        /// <summary>
        /// 玩家离开座位前事件出路
        /// </summary>
        /// <param name="inningeGame"></param>
        /// <param name="e"></param>
        public override void BeforPlayerLeave(object inningeGame, EventArgs e)
        { }
        /// <summary>
        /// 玩家离开座位后事件处理
        /// </summary>
        /// <param name="inningeGame"></param>
        /// <param name="e"></param>
        public override void AfterPlayerLeave(object inningeGame, EventArgs e)
        { }
        /// <summary>
        /// 游戏异常中断
        /// </summary>
        /// <param name="inningeGame">本局游戏</param>
        /// <param name="e"></param>
        public override void Stoped(object inningeGame, EventArgs e)
        {
            var myE = (GameStopedEventArgs)e;
            NotifyRoomPlayers(WebscoketSendObjs.Stoped(0, myE.Message));
        }
        /// <summary>
        /// 游戏正常结束
        /// </summary>
        /// <param name="inningeGame">本局游戏</param>
        /// <param name="e"></param>
        public override void GameOver(object inningeGame, EventArgs e)
        {
           // Notify?.Invoke(WebscoketSendObjs.GameOver(0));
            NotifyRoomPlayers(WebscoketSendObjs.GameOver(0));

        }
        /// <summary>
        /// 刷新玩家客户端数据
        /// </summary>
        /// <param name="playerId">玩家Id</param>
        /// <returns>发送到客户端玩家数据</returns>
        [CanVisitByClientAttibue]
        public object FreshGameFace(int id)
        {
            return null;
        }
        /// <summary>
        /// 重置游戏后事件
        /// </summary>
        /// <param name="inningeGame"></param>
        /// <param name="e"></param>
        public override void ResetGame(object inningeGame, EventArgs e)
        {
           // Notify?.Invoke(WebscoketSendObjs.ResetGame(0));
            NotifyRoomPlayers(WebscoketSendObjs.ResetGame(0));
        }
        #region 各个游戏单独定义的内容
        private IPlayerJoinRoom player;
        [CanVisitByClientAttibue]
        public string GetHitBoxs(string stackeBoxsStr)
        {

            // StreamReader streamReader = new StreamReader(httpContextAccessor.HttpContext.Request.Body, Encoding.UTF8);
            // string stackeBoxsStr = streamReader.ReadToEnd();

            List<StakeBox> stakeBoxs = JsonConvert.DeserializeObject<List<StakeBox>>(stackeBoxsStr);
           
            
            if (player.Account* 10 < stakeBoxs.Sum(p => p.Stake))
           //if (player.AccountNotEnough(stakeBoxs.Sum(p => p.Stake)/10))
            {
                return null;
            }

            BoxsManager boxsManager = new BoxsManager()
            {
                deductPlayerAccount = ChangePlayerAccount,
                addPlayerAccount = ChangePlayerAccount
            };
            //boxsManager.deductPlayerAccount = (amount, explain) => { playerService.AdjustAccount(player, amount, explain); };
            // boxsManager.addPlayerAccount = (amount, explain) => { playerService.AdjustAccount(player, amount, explain); };
            Collection<Box> winningBoxs = boxsManager.WinningResult(stakeBoxs);
            string hitSakeBoxsJsonarry = JsonConvert.SerializeObject(winningBoxs);
            return hitSakeBoxsJsonarry;
        }
        private void ChangePlayerAccount(decimal amount,string explain)
        {
            player.Account=(decimal)DChangePlayerAccount?.Invoke(player.WeixinName, amount, explain);
        }

        public ISeat CreatSeat(IInningeGame inningeGame) {
            return new Seat(inningeGame);
        }
        #endregion
    }
}
