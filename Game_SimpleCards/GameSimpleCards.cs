using System;
using AntDesigner.NetCore.GameCity;
using System.Collections.Generic;
using System.Reflection;
using AntDesigner.NetCore.GameCity;
namespace AntDesigner.NetCore.Games.GameSimpleCards
{
    /// <summary>
    /// 二人单张扑克比大小游戏
    /// </summary>
    public class GameSimpleCards : ABGameProject, IGameProject
    {
        #region 和该项游戏相关属性
        /// <summary>
        /// 扑克发牌器
        /// </summary>
        Poker Poker { get; set; }
        /// <summary>
        /// 每次打底(入局)金额
        /// </summary>
        public decimal ChipInAmount { get; set; }
        #endregion

        public GameSimpleCards()
        {
            #region 初始化时每项游戏必须设置的属性
            ShowName = "二人比大小";//游戏名称
            Name = this.GetType().Name;//反射调用名称
            PlayerCountLimit = 2;//人数上限
            PlayerCountLeast = 2;//人数下限
            #endregion
            ChipInAmount = 1;
        }
        #region
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
        public override void BeforGameStart(object inningeGame, EventArgs e)
        {

        }
        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="sender">IInngineGame</param>
        /// <param name="e"></param>
        public override void GameStart(object inngineGame, EventArgs e)
        {
            InningeGame = (IInningeGame)inngineGame;
            Poker = new Poker();
            foreach (var seat in InningeGame.NotEmptySeats())
            {
                seat.GameDateObj.Add("getPokers", new List<object>());
                seat.GameDateObj.Add("playOutPokers", new List<object>());
                seat.GameDateObj.Add("compareResult", new List<object>());
            }
            foreach (var item in InningeGame.NotEmptySeats())
            {
                PlayerGetOnePoker(item.IPlayer.Id);
            }

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
        public override  void BeforAddSeat(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 添加座位后事件处理
        /// </summary>
        /// <param name="inningeGame">本局游戏</param>
        /// <param name="e"></param>
        public override void AfterAddSeat(object inningeGame, EventArgs e)
        {

        }
        /// <summary>
        /// 玩家能否坐下检查
        /// </summary>
        /// <param name="inningeGame">一局游戏</param>
        /// <returns>YN</returns>
        public override bool CheckSitDown(IInningeGame inningeGame)
        {
            if (inningeGame.IsStarted == false)
            {
                return true;
            }
            return false;
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
            Notify?.Invoke(WebscoketSendObjs.Stoped(0));
        }
        /// <summary>
        /// 游戏正常结束
        /// </summary>
        /// <param name="inningeGame">本局游戏</param>
        /// <param name="e"></param>
        public override void GameOver(object inningeGame, EventArgs e)
        {
            Notify?.Invoke(WebscoketSendObjs.GameOver(0));
        }
        /// <summary>
        /// 刷新玩家数据
        /// </summary>
        /// <param name="playerId">玩家Id</param>
        /// <returns>发送到客户端玩家数据</returns>
        public object FreshGameFace(int playerId)
        {
            var seat = InningeGame.GetSeatByPlayerId(playerId);
            //var seats = InningeGame.NotMyEmtySteats(playerId);
            Dictionary<int, object> otherPlayerPokers = new Dictionary<int, object>();
            var opponentId = GetOpponentId(playerId);
            var opponentSeat = InningeGame.GetSeatByPlayerId(opponentId);
            object opponetPoker = new { Name = "", CardColor = "" };
            if (opponentSeat.GameDateObj["compareResult"].Count == 1)
            {
                opponetPoker = opponentSeat.GameDateObj["compareResult"][0];

            }
            object myPoker_ = new { Name = "", CardColor = "" };
            if (seat.GameDateObj["getPokers"].Count == 1)
            {
                myPoker_ = seat.GameDateObj["getPokers"][0];
            }
            object myComparePoker_ = new { Name = "", CardColor = "" };
            if (seat.GameDateObj["compareResult"].Count == 1)
            {
                myComparePoker_ = seat.GameDateObj["compareResult"][0];
            }
            object myPayOutPoker_ = new { Name = "", CardColor = "" };
            if (seat.GameDateObj["playOutPokers"].Count == 1)
            {
                myPayOutPoker_ = seat.GameDateObj["playOutPokers"][0];
            }
            var GameFace = new
            {
                myPoker = myPoker_,
                myPayOutPoker = myPayOutPoker_,
                myComparePoker = myComparePoker_,
                otherPlayersPoker = opponetPoker
            };
            return GameFace;
        }
        #endregion
        #region 自定义
        /// <summary>
        /// 洗牌
        /// </summary>
        public void Riffile()
        {
            Poker.Riffile();
        }
        /// <summary>
        /// 玩家获得一张牌
        /// </summary>
        /// <param name="playerId_">玩家Id</param>
        /// <returns>获得的牌</returns>
        public ABCard PlayerGetOnePoker(int playerId)
        {
            var seat = InningeGame.GetSeatByPlayerId(playerId);
            var player = seat.IPlayer;
            var opponentId = GetOpponentId(playerId);
            var opponentSeat = InningeGame.GetSeatByPlayerId(opponentId);
            if (player.AccountNotEnough(ChipInAmount))
            {
                Notify?.Invoke(WebscoketSendObjs.Alert(player.Id,"账户余额不足"));
            }
            if (seat.GameDateObj["getPokers"].Count > 0)
            {
                return null;
            }
            player.DecutMoney(ChipInAmount);
            if (Poker.RemaindCount > 0)
            {
                var card = Poker.TackOut(1)[0];
                seat.GameDateObj["getPokers"].Add(card);
                seat.GameDateObj["compareResult"].Clear();
                Notify?.Invoke(WebscoketSendObjs.FreshGameFace(player.Id));
                return card;
            }
            return null;
        }
        /// <summary>
        /// 出牌或出牌后比较大小
        /// </summary>
        /// <param name="playerId">玩家Id</param>
        /// <param name="card_">一张牌</param>
        /// <returns>获胜玩家座位</returns>
        public void PlayerPlayOutOnePoker(int playerId)
        {
            var meSeat = InningeGame.GetSeatByPlayerId(playerId);
            var opponentId = GetOpponentId(playerId);
            var opponentSeat = InningeGame.GetSeatByPlayerId(opponentId);
            var opponentPayOut = opponentSeat.GameDateObj["playOutPokers"];
            var opponentCompareResult = opponentSeat.GameDateObj["compareResult"];
            object card = null;
            //var card = CheckCardExist(playerId, card_);
            if (meSeat.GameDateObj["getPokers"].Count == 0)
            {
                return;
            }
            var mePoker = meSeat.GameDateObj["getPokers"];
            card = mePoker[0];
            var mePayOut = meSeat.GameDateObj["playOutPokers"];
            var meCompareResult = meSeat.GameDateObj["compareResult"];
            ISeat winPlayer = null;
            if (card != null)
            {
                mePoker.Remove(card);
                mePayOut.Add(card);
                if (opponentPayOut.Count > 0)
                {
                    if (((ABCard)mePayOut[0]).ComparedValue > ((ABCard)opponentPayOut[0]).ComparedValue)
                    {
                        winPlayer = meSeat;
                    }
                    else
                    {
                        winPlayer = opponentSeat;
                    }
                    mePayOut.Remove(card);
                    meCompareResult.Add(card);
                    opponentCompareResult.Add(opponentPayOut[0]);
                    opponentPayOut.Clear();
                    winPlayer.IPlayer.DecutMoney(-ChipInAmount);
                    foreach (var item in InningeGame.NotEmptySeats())//通知全部客户端刷新
                    {
                       // Notify?.Invoke(item.IPlayer.Id, "FreshGameFace");
                        Notify?.Invoke(WebscoketSendObjs.FreshGameFace(item.IPlayer.Id));
                    }// ClearPayOut(opponentPayOut, mePayOut);
                }
                Notify?.Invoke(WebscoketSendObjs.FreshGameFace(playerId));
                return;
            }
            return;
        }
        /// <summary>
        /// 清理双方出的牌
        /// </summary>
        /// <param name="opponentPayOut_"></param>
        /// <param name="mePayOut_"></param>
        private void ClearPayOut()
        {
            var seats = InningeGame.NotEmptySeats();
            foreach (var item in seats)
            {
                item.GameDateObj["playOutPokers"].Clear();
            }
        }
        /// <summary>
        /// 检查玩家手里是否存在指定牌
        /// </summary>
        /// <param name="playerId_">玩家Id</param>
        /// <param name="card_">牌</param>
        /// <returns></returns>
        private ABCard CheckCardExist(int playerId_, ABCard card_)
        {
            var seat = InningeGame.GetSeatByPlayerId(playerId_);
            ABCard card = null;
            foreach (ABCard item in seat.GameDateObj["getPokers"])
            {
                if (item.Name == card_.Name && item.CardColor == card_.CardColor)
                {
                    card = item;
                    break;
                }
            }
            return card;
        }
        /// <summary>
        /// 获得对手的Id
        /// </summary>
        /// <param name="id_">自己的Id</param>
        /// <returns>对手Id</returns>
        public int GetOpponentId(int id_)
        {
            int opponentid = 0;
            var Seats = InningeGame.NotEmptySeats();
            foreach (ISeat seat in Seats)
            {
                if (seat.IPlayer.Id != id_)
                {
                    opponentid = seat.IPlayer.Id;
                    break;
                }
            }
            return opponentid;
        }
        #endregion

    }
}
