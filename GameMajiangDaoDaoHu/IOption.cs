namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
   public interface IOption {
        string Name { get; }
        decimal Priority { get; }
        bool Do(HandCardManager handcards,CardModel card);
    }
}