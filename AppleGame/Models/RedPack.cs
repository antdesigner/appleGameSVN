
using AntDesigner.GameCityBase.interFace;
using System;
using System.ComponentModel.DataAnnotations.Schema;

using WxPayAPI;

namespace AntDesigner.weiXinPay
{
    public class RedPack 
    {
        public int Id { get; set; }
        public string WeixinName { get; set; }
        public decimal Amount { get; set; }
        public string Err_code { get; set;}
        public string Mch_billno { get; set; }
        public string Send_listid { get; set;}
        public RedPack() { }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateTime
        {
            get; set;
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ModifyTime
        { get; set; }
    }
}