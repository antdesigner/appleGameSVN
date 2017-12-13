using AntDesigner.NetCore.GameCity;
using System;
using System.Collections.Generic;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    public class GameMajiangDaoDaoHu : ABGameProject, IGameProject {

        /// <summary>
        /// 麻将发牌器
        /// </summary>
        MajiangDaoDaoHuManager MaJiangManager { get; set; }
        /// <summary>
        /// 进行状态
        /// </summary>
        Stage CurrentStage { get; set; }
        /// <summary>
        /// 底金大小
        /// </summary>
        decimal ChipInAmount { get; set; }
        /// <summary>
        /// 参与座位控制器
        /// </summary>
        JoinSeatsManager SeatsManager { get; set; }
        /// <summary>
        /// 客户端请求收集控制器
        /// </summary>
        OptionorRequestCollection OptionorReuestManager { get; set; }
        /// <summary>
        /// 操作令牌控制器
        /// </summary>
        TakonManager TakonManager { get; set; }
        IMajiangRuls Ruls { get; set; }
        public GameMajiangDaoDaoHu() {
            #region 初始化时每项游戏必须设置的属性
            ShowName = "麻将倒倒胡";//游戏名称,可根据游戏项目指定属性值
            Name = this.GetType().Name;//反射调用名称(勿修改)
            PlayerCountLimit = 4;//人数上限,可根据游戏项目指定属性值
            PlayerCountLeast = 2;//人数下限,可根据游戏项目指定属性值
            #endregion
            #region 自定义初始化区域
            Ruls = new DaoDaoHuRuls();
            CurrentStage = Stage.Waiting;
            ChipInAmount = 1;
            MaJiangManager = new MajiangDaoDaoHuManager();
            Seat managerSeat = GetSeatByPlayerId(InningeGame.IRoom.RoomManager.Id);
            SeatsManager = new JoinSeatsManager(managerSeat);
            OptionorReuestManager = new OptionorRequestCollection();
            TakonManager = new TakonManager();
            TakonManager.RecievedToken += TakonManager_RecievedToken;
            #endregion
        }
        /// <summary>
        /// 令牌控制器接受座位抛出的令牌
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TakonManager_RecievedToken(object sender, EventArgs e) {
           //通知客户端
        }

        public ISeat CreatSeat(IInningeGame inningeGame) {
            var seat = new Seat(inningeGame);
            seat.PayedCard += Seat_PayedCard;
            seat.GetCardAction = GetCardAction_Handler;
            seat.EjectionedToken += Seat_EjectionedToken; ;
            return seat;
        }

        /// <summary>
        /// 处理座位抛出令牌事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Seat_EjectionedToken(object sender, EventArgs e) {
            Seat seat = (Seat)sender;
            TakonManager.RecieveTokenFrom(seat);
        }
        private void Seat_PayedCard(object sender, EventArgs e) {
            var seat = (Seat)sender;
            seat.HandCards.FreshOptionors(Ruls.FreshOptions);
            TakonManager.RecieveTokenFrom(seat);
        }
        private void GetCardAction_Handler(Seat seat) {
            seat.HandCards.FreshOptionors(Ruls.FreshOptions);

           // GetingCard(seat.IPlayer.Id);
        }
        public object FreshGameFace(int id) {
            throw new NotImplementedException();
        }
        #region 覆写ABGameProject方法区域
        public override void GameStart(object inngineGame, EventArgs e) {
            InningeGame = (IInningeGame)inngineGame;
            MaJiangManager.Riffile();
            NotifyRoomPlayers(new FreshGameFace(0));
            NotifyRoomPlayers(WebscoketSendObjs.RoomMessage(0, "游戏开始了!请准备!"));
        }
        /// <summary>
        /// 检查玩家是否超时
        /// </summary>
        /// <param name="obj"></param>
        private void CheckPlayOverTime(object obj) {

        }
        private void RemoveCurrentPlayer() {

        }
        public override void GameOver(object inningeGame, EventArgs e) {

        }
        public override void ResetGame(object inningeGame, EventArgs e) {

            NotifyRoomPlayers(new FreshGameFace(0));
            NotifyRoomPlayers(WebscoketSendObjs.RoomMessage(0, "新一局开始!开始押底!"));
        }
        private void InitPublicInfo() {

        }
        #endregion 覆写ABGameProject方法区域
        #region 自定义方法区域
        /// <summary>
        /// 玩家准备
        /// </summary>
        /// <param name="playerId"></param>
        public void Ready(int playerId) {
            Seat seat = GetSeatByPlayerId(playerId);
            SeatsManager.Add(seat);
        }
        /// <summary>
        /// 发牌
        /// </summary>
        public void Deal() {
            var seats = SeatsManager.GetSeats();
            for (int i = 0; i < seats.Count; i++) {
                var cards = MaJiangManager.TackOut(13);
                seats[i].InitialHandCars((StandCardsCollection)cards);
            }
        }
        /// <summary>
        /// 出牌
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="name"></param>
        /// <param name=""></param>
        public void PayOut(int playerId, string name, string cardColor) {
            Seat seat = GetSeatByPlayerId(playerId);
            if (seat.Token is null) {
                return;
            }
            seat.PayCard(name, cardColor);
        }
        /// <summary>
        /// 玩家操作请求
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="optionName"></param>
        public void OptionorRequest(int playerId, string optionName, CardModel cardM) {
            if (!TakonManager.HaveToken) {
                return; 
            }
            Seat seat = GetSeatByPlayerId(playerId);
            var optionor = Ruls.GetOptionorByName(optionName, cardM);
            OptionorRequest optionorRequest = new OptionorRequest(seat, optionor);
            OptionorReuestManager.Add(optionorRequest);
        }
        /// <summary>
        /// 找到玩家座位
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        private Seat GetSeatByPlayerId(int playerId) {
            Seat seat = (Seat)InningeGame.GetSeatByPlayerId(playerId);
            return seat;
        }
        #endregion 自定义方法区域
    }
}
