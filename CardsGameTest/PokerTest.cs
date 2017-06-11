using System;
using Xunit;
using AntDesigner.NetCore.Games;
namespace CardsGameTest
{
    public class PokerTest
    {
       [Theory(DisplayName = "实际发牌数量=设置数量")]
       [InlineData(2)]
       [InlineData(0)]
        public void TackOut_1(int n)
        {
            Poker p = new Poker();
            Assert.Equal(p.TackOut(n).Count,n);
        }
        [Theory(DisplayName = "发牌异常数量<0")]
        [InlineData(-1)]
        public void TackOut_2(int n)
        {
            Poker p = new Poker();
            Assert.Equal(p.TackOut(n).Count,0);
        }
        [Theory(DisplayName = "发牌异常数量>剩余数量")]
        [InlineData(500)]
        public void TackOut_3(int n)
        {
            Poker p = new Poker();
            p.TackOut(500);
            Assert.Equal(p.RemaindCount, 0);
        }
    }
}
