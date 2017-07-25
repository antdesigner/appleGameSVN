using System;
using AntDesigner.NetCore.GameCity;
using System.Collections.Generic;
using System.Reflection;
namespace AntDesigner.NetCore.Games.GameSimpleCards
{
    /// <summary>
    /// 二人单张扑克比大小游戏
    /// </summary>
    public class GameSimpleCards : ABGameProject, IGameProject
    {
        #region 自定义游戏的属性区域
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
            ShowName = "二人比大小";//游戏名称,可根据游戏项目指定属性值
            Name = this.GetType().Name;//反射调用名称(勿修改
            PlayerCountLimit = 2;//人数上限,可根据游戏项目指定属性值
            PlayerCountLeast = 2;//人数下限,可根据游戏项目指定属性值
            #endregion
            #region 自定义初始化区域
            ChipInAmount = 1;
#endregion
        }
        #region 覆写ABGameProject默认方法的区域
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
                base.InningeGame = innineGame_;
                // Notify?.Invoke(WebscoketSendObjs.RoomMessage(0, "人数不足,不能启动游戏"));
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
            //foreach (var seat in InningeGame.NotEmptySeats())
            //{
                
            //    seat.GameDataObj.Add("getPokers", new List<object>());
            //    seat.GameDataObj.Add("playOutPokers", new List<object>());
            //    seat.GameDataObj.Add("compareResult", new List<object>());
            //}
            foreach (var item in InningeGame.NotEmptySeats())
            {
                PlayerGetOnePoker(item.IPlayer.Id);
            }
             base.GameStart(InningeGame, e);  
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
        {
            var roomMessage = WebscoketSendObjs.RoomMessage(0, "有玩家进入");
            NotifyRoomPlayers(roomMessage);
           // Notify?.Invoke(roomMessage);
        }
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
        {
            string playerId = ((PlayerEventArgs)e).Player.Id.ToString();
            var roomMessage = WebscoketSendObjs.RoomMessage(0, "玩家" + playerId + "离开了");
           // Notify?.Invoke(roomMessage);
            NotifyRoomPlayers(roomMessage);
        }
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
            NotifyRoomPlayers(WebscoketSendObjs.GameOver(0));
        }
        /// 重启游戏
        /// </summary>
        /// <param name="inningeGame"></param>
        /// <param name="e"></param>
        public override void ResetGame(object inningeGame, EventArgs e) {
            NotifyRoomPlayers(WebscoketSendObjs.ResetGame(0));
        }
        #endregion
        /// <summary>
        /// 刷新玩家客户端数据
        /// </summary>
        /// <param name="playerId">玩家Id</param>
        /// <returns>发送到客户端玩家数据</returns>
        /// /// <summary>
        ///   
        public  object FreshGameFace(int playerId)
        {
            Seat seat = GetSeatByPlayerId(playerId);
            Dictionary<int, object> otherPlayerPokers = new Dictionary<int, object>();
            int opponentId = GetOpponentId(playerId);
            Seat opponentSeat =GetSeatByPlayerId(opponentId);
            object opponetPoker = new { Name = "", CardColor = "" };
                if (null!=opponentSeat.CompareResult ){
                opponetPoker = opponentSeat.CompareResult;

            }
            object myPoker_ = new { Name = "", CardColor = "" };
            if (null!=seat.GetPoker)
            {
                myPoker_ = seat.GetPoker;
            }
            object myComparePoker_ = new { Name = "", CardColor = "" };
            if (null!=seat.CompareResult)
            {
                myComparePoker_ = seat.CompareResult;
            }
            object myPayOutPoker_ = new { Name = "", CardColor = "" };
            if (null!=seat.PlayOutPokers)
            {
                myPayOutPoker_ = seat.PlayOutPokers;
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
        /// <summary>
        ///创建游戏项目的特性座位对象,继承自seat
        /// </summary>
        /// <param name="inningeGame"></param>
        /// <returns></returns>
        public ISeat CreatSeat(IInningeGame inningeGame) {
            return new Seat(inningeGame);
        }
        /// <summary>
        /// 根据玩家Id获得对应座位
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        private Seat GetSeatByPlayerId(int playerId) {

            return (Seat)InningeGame.GetSeatByPlayerId(playerId);
        }
        #region 自定义方法区域
        /// <summary>
        /// 洗牌
        /// </summary>
        public void Riffile()
        {
            Poker.Riffile();
        }
        private object Locker=new object();
        /// <summary>
        /// 玩家获得一张牌
        /// </summary>
        /// <param name="playerId_">玩家Id</param>
        /// <returns>获得的牌</returns>
        public ABCard PlayerGetOnePoker(int playerId)
        {
        Seat seat = GetSeatByPlayerId(playerId);
            var player = seat.IPlayer;
            int opponentId = GetOpponentId(playerId);
            Seat opponentSeat =GetSeatByPlayerId(opponentId);
            if (player.AccountNotEnough(ChipInAmount))
            {
                NotifySinglePlayer(WebscoketSendObjs.Alert(player.Id, "账户余额不足"), player.Id);
            }
            if (null==seat.PlayOutPokers&&null!=seat.GetPoker)
            {
                return null;
            }
            player.DecutMoney(ChipInAmount); 
            lock(Locker)
            { 
            if (Poker.RemaindCount > 0 )
                {
                    if (null==seat.PlayOutPokers)
                    {
                        var card = Poker.TackOut(1)[0];
                        seat.GetPoker=card;
                        seat.CompareResult = null;
                        NotifySinglePlayer(WebscoketSendObjs.FreshGameFace(player.Id), player.Id);
                        return card;
                    }
                    return null;
            }
            InningeGame.GameOver();//触发游戏结束事件
            return null;
            }
        }
        /// <summary>
        /// 出牌或出牌后比较大小
        /// </summary>
        /// <param name="playerId">玩家Id</param>
        /// <param name="card_">一张牌</param>
        /// <returns>获胜玩家座位</returns>
        public void PlayerPlayOutOnePoker(int playerId)
        {
           Seat meSeat = GetSeatByPlayerId(playerId);
            int opponentId = GetOpponentId(playerId);
            Seat opponentSeat = GetSeatByPlayerId(opponentId);
            var opponentPayOut = opponentSeat.PlayOutPokers;
            var opponentCompareResult = opponentSeat.CompareResult;
           ABCard  card = null;
            if (null==meSeat.GetPoker)
            {
                return;
            }
            card = meSeat.GetPoker;
            var meCompareResult = meSeat.CompareResult;
            ISeat winPlayer = null;
            if (card != null)
            {
                lock (Locker) 
                {
                    meSeat.GetPoker = null;
                    meSeat.PlayOutPokers = card;
                if (opponentPayOut!=null)
                {
                        if (meSeat.PlayOutPokers.ComparedValue >opponentSeat.PlayOutPokers.ComparedValue) {
                        winPlayer = meSeat;
                    }
                    else
                    {
                        winPlayer = opponentSeat;
                    }
                        meSeat.PlayOutPokers = null;
                        meSeat.CompareResult = card;
                        opponentSeat.CompareResult = opponentSeat.PlayOutPokers;
                        opponentSeat.PlayOutPokers = null;
                    winPlayer.IPlayer.DecutMoney(-ChipInAmount);
                    foreach (var item in InningeGame.NotEmptySeats())//通知全部客户端刷新
                    {
                            NotifySinglePlayer(WebscoketSendObjs.FreshGameFace(item.IPlayer.Id), item.IPlayer.Id);
                    }
                }
         NotifySinglePlayer(WebscoketSendObjs.FreshGameFace(playerId), playerId);
                return;
                }
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
                ((Seat)item).PlayOutPokers = null;
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
            Seat  seat =GetSeatByPlayerId(playerId_);
            ABCard card = null;
                if (seat.GetPoker.Name == card_.Name && seat.GetPoker.CardColor == card_.CardColor) {
                card = seat.GetPoker;
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
