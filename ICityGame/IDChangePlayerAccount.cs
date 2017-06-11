using System;

namespace AntDesigner.NetCore.GameCity
{
    public interface IDChangePlayerAccount
    {
        Func<string, decimal, string,decimal> DChangePlayerAccount { get; set; }
    }
}