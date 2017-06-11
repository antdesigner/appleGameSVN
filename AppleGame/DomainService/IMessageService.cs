using System.Collections.Generic;
using AntDesigner.GameCityBase;

namespace GameCitys.DomainService
{
    public interface IMessageService
    {
        Message GetMessageById(int id);
        IList<Message> Messages(string name);
        IList<Message> MessagesOfPlayer(Player player);
        void ReplayMessage(Message message, string content);
        void SendMessage(Message message);

    }
}