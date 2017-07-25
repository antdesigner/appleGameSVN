using AntDesigner.NetCore.GameCity;
using System.Collections.Generic;
namespace AntDesigner.NetCore.Games.GameThreePokers {
    /// <summary>
    /// 该游戏项目的特殊化对象,添加属性来保存该游戏项目的座位属性和特殊业务逻辑
    /// </summary>
    public class Seat: GameCity.Seat {
        public Seat(IInningeGame inningeGame) : base(inningeGame) {
            PokersShow = new List<Card>();
            for (int i = 0; i < 3; i++) {
                PokersShow.Add(new Card(0, "", ""));
            }
            PokerOtherCanSee = null;
            PreChipInAmount = 0;
            IsChipIned = false;
            IsLooked = false;
            IsGaveUp = false;
        }
        /// <summary>
        /// 得到的三张牌
        /// </summary>
        public ThreeCards Pokers { get; set; } 
        /// <summary>
        /// 可以看的牌
        /// </summary>
        public List<Card> PokersShow { get; set; }
        public Card PokerOtherCanSee { get; set; }
        /// <summary>
        /// 可以查看其他玩家的一张牌的Id
        /// </summary>
        public int PlayerIdWhichCanSee { get; set; }
        /// <summary>
        /// 自己上一次下注积分
        /// </summary>
        public decimal PreChipInAmount { get; set; }
        /// <summary>
        /// 是否已押底
        /// </summary>
        public bool IsChipIned { get; set; }
        /// <summary>
        /// 是否已看牌
        /// </summary>
        public bool IsLooked { get; set; }
        /// <summary>
        /// 是否已弃牌
        /// </summary>
        public bool IsGaveUp { get; set; }
        /// <summary>
        /// 清空座位每局数据
        /// </summary>
        public override  void ClearSeatInfo() {
            Pokers = null;
            PokersShow.Clear();
            PreChipInAmount = 0;
            IsChipIned = false;
            IsLooked = false;
            IsGaveUp = false;
        }
    }
}
