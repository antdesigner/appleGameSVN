using System;
using System.Collections.Generic;


namespace WxPayAPI
{
    public class WxPayException : Exception 
    {
        public WxPayException(string msg) : base(msg) 
        {

        }
     }
}