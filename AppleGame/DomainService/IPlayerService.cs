using System.Collections.Generic;
using AntDesigner.GameCityBase;

namespace GameCitys.DomainService
{
    public interface IPlayerService
    {
        Player AddPlayer(Player player);
        IList<Player> GetAllPlayers();
        Player FindPlayerByName(string name);
        ManagePlayer GetManagerPlayer();
        IList<Player> GetPlayersOfPlayer(Player player);
        void AttachPlayer(Player player);
        Account AdjustAccount(Player player, decimal amount, string explain);
        void AdjustAccount(string name, decimal amount, string explain);
        IList<AccountDetail> GetAccountDetailsOfPlayer(Player player);
    }
}