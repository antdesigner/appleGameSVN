using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.GameCity
{
   public class Gplayer : IPlayerJoinRoom
    {
        public string WeixinName { get; set; }
        public int Id { get; set; }

        public decimal Account { get; set; }

        public bool AccountNotEnough(decimal ammount_)
        {
            if (Account < ammount_)
            {
                return true;
            }
            return false;
        }

        public bool DecutMoney(decimal ticketPrice,string cause)
        {
            var account = Account - ticketPrice;
            if (account >= 0)
            {
                Account = account;
                return true;
            }
            return false;
        }
    }
}
