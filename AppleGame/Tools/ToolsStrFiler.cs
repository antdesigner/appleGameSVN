using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCitys.Tools
{
    public static class ToolsStrFiler
    {
        public static string ForWebClient(string message) {
            StringBuilder strBuilder = new StringBuilder(message);
            strBuilder = strBuilder.Replace("script", "");
            strBuilder = strBuilder.Replace("link", "");
            strBuilder = strBuilder.Replace("http", "");
            strBuilder = strBuilder.Replace(" ", "");
            strBuilder = strBuilder.Replace(".", "。");
            strBuilder = strBuilder.Replace("<", "");
            strBuilder = strBuilder.Replace(">", "");
            return strBuilder.ToString();
        }

    }
}
