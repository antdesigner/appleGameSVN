﻿using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using AntDesigner.NetCore.Games;
namespace CardsGameTest
{
    public class PokerWithoutKingTest
    {
        [Theory(DisplayName = "生成牌的覆写Specific方法")]
        [InlineData("X")]
        [InlineData("XX")]
        public void Specific_1(string cardName_)
        {
          PokersWithoutKingManger poker = new PokersWithoutKingManger();
            bool hasCardsXXorX=false;
            while (poker.RemaindCount > 0  )
            {
                if (poker.TackOut(1)[0].Name == cardName_)
                {
                    hasCardsXXorX = true;
                    break;
                }
            }
            Assert.False(hasCardsXXorX);
        }
        [Theory(DisplayName = "生成牌的覆写Specific方法")]
        [InlineData("2")]
        public void Specific_2(string cardName_)
        {
            PokersWithoutKingManger poker = new PokersWithoutKingManger();
            bool hasCardsXXorX =false;
            while (poker.RemaindCount > 0)
            {
                if (poker.TackOut(1)[0].Name == cardName_)
                {
                    hasCardsXXorX = true;
                    break;
                }
            }
            Assert.True (hasCardsXXorX);
        }

    }
}
