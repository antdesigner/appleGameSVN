using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AntDesigner.GameCityBase;
using AntDesigner.GameCityBase.EF;
using Microsoft.AspNetCore.Http;
using AntDesigner.GameCityBase.interFace;
using GameCitys.DomainService;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AntDesigner.GameCityBase.Controllers
{
    public class MessageController:MyController
    {
        public MessageController( IHttpContextAccessor httpContextAccessor, IPlayerService playerService) : base( httpContextAccessor, playerService)
        {

        }
        public IActionResult Messages([FromServices]IMessageService messageService)
        {
            IList<Message> messages = messageService.MessagesOfPlayer(player);
            return View("messages", messages);
        }
        public IActionResult Index_replayMessage(int messageId, [FromServices]IMessageService messageService)
        {
            // Message message = IstoreHouse.getEntityById<Message>(messageId);
            Message message = messageService.MessagesOfPlayer(player).FirstOrDefault(p => p.Id == messageId);
            return View("replayMessage",message);
        }

        public IActionResult ReplayMessage(int messageId,string replayContent,[FromServices]IMessageService messageService)
        {
            //Message message = player.SeeMessags().FirstOrDefault(p => p.Id == messageId);
            //player.ReplyMessage(message, replayContent);
            //IstoreHouse.SaveChanges();
            Message message = messageService.GetMessageById(messageId);
            messageService.ReplayMessage(message, replayContent);
            return Content("replySuccess");
        }
    }
}
