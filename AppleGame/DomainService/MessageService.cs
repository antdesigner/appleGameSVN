using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntDesigner.GameCityBase.interFace;
using AntDesigner.GameCityBase;

namespace GameCitys.DomainService
{
    public class MessageService : ABIStorehouse, IMessageService
    {
        public MessageService(IStorehouse istoreHose) : base(istoreHose)
        {
        }
        public void SendMessage( Message message)
        {
           // Message message = new Message(sender, content, receiver);
            _storeHouse.AddMessage(message);
            _storeHouse.SaveChanges();
            return;
        }
        public IList<Message> Messages(string name)
        {
            Player player = _storeHouse.GetPlayerByName(name);
            return _storeHouse.GetMessagsOfPlayer(player);
        }

        public IList<Message> MessagesOfPlayer(Player player)
        {
            return _storeHouse.GetMessagsOfPlayer(player);
        }
        public Message GetMessageById(int id)
        {
            return _storeHouse.GetEntityById<Message>(id);
        }
        public void ReplayMessage(Message message, string content)
        {
            message.ReplyContent = content;
            _storeHouse.SaveChanges();
            return;
        }
    }
}
