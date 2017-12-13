namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    /// <summary>
    /// 出牌类
    /// </summary>
    internal class PutOutCard{
        int PlayerId { get; set; }
        internal Card Card { get; set; }
        PutOutCard(int playerId,Card card) {
            PlayerId = playerId;
            Card = card;
        }
    }
}