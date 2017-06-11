using System;
using Xunit;
using AntDesigner.NetCore.GameCity;
using AntDesigner.NetCore.Games.GameSimpleCards;
using Moq;
using AntDesigner.NetCore.Games;

namespace Game_SimpleCardsTest
{
    public class SimpleCardsTest
    {
        GameSimpleCards _simpleCards;
        IInningeGame _inningeGame;
        IPlayerJoinRoom _playerA;
        IPlayerJoinRoom _playerB;

        public SimpleCardsTest()
        { Mock<IPlayerJoinRoom> playerFactoryA = new Mock<IPlayerJoinRoom>();
            playerFactoryA.SetupAllProperties();
            playerFactoryA.Setup(p => p.AccountNotEnough(It.IsAny<decimal>())).Returns(false);
            _playerA = playerFactoryA.Object;

            Mock<IPlayerJoinRoom> playerFactoryB = new Mock<IPlayerJoinRoom>();
            playerFactoryB.SetupAllProperties();
            _playerB = playerFactoryB.Object;
            _playerA.Id = 1;
            _playerB.Id = 2;
            _simpleCards = new GameSimpleCards();
           _inningeGame = new InningeGame(_simpleCards);
           _inningeGame.AddSet(2);
           _inningeGame.GetOneEmptySeat().PlayerSitDown(_playerA);
           _inningeGame.GetOneEmptySeat().PlayerSitDown(_playerB);

        }
        [Fact(DisplayName ="游戏初始化")]
        public void GameStart()
        {
            _inningeGame.Start();
            foreach (var seat in _inningeGame.NotEmptySeats())
            {
                Assert.True(seat.GameDateObj.ContainsKey("getPokers"));
                Assert.True(seat.GameDateObj.ContainsKey("playOutPokers"));
            }
        }
        [Fact(DisplayName ="玩家获得一张发出的牌")]

        public void PlayerGetPokers()
        {
           
            _inningeGame.Start();
            var seatOfPlayer = _inningeGame.GetSeatByPlayerId(_playerA.Id);
            Assert.True(seatOfPlayer.GameDateObj["getPokers"].Count ==0);
            _simpleCards.PlayerGetOnePoker(_playerA);
            Assert.True(seatOfPlayer.GameDateObj["getPokers"].Count == 1);
        }
        [Fact(DisplayName ="玩家手中存在指定牌")]
        public void CheckCardExist()
        {
            _inningeGame.Start();
            var seatOfplayer = _inningeGame.GetSeatByPlayerId(_playerA.Id);
               var card= _simpleCards.PlayerGetOnePoker(_playerA);
            
            Assert.True(_simpleCards.CheckCardExistForTest(1, card).Name ==card.Name  );
            Assert.True(_simpleCards.CheckCardExistForTest(1, card).CardColor== card.CardColor);
        }
        [Fact(DisplayName ="玩家出牌")]
        public void PlayerPlayOutPokers()
        {
            _inningeGame.Start();
            var seatOfplayer = _inningeGame.GetSeatByPlayerId(_playerA.Id);
            var card2 = _simpleCards.PlayerGetOnePoker(_playerA );
            var card3 = new Card(card2.ComparedValue, card2.CardColor, card2.Name);

            _simpleCards.PlayerPlayOutOnePoker(1, card3);
            var card = (ABCard)seatOfplayer.GameDateObj["playOutPokers"][0];

            Assert.Equal(card2, card);
            Assert.DoesNotContain((object)card2, seatOfplayer.GameDateObj["getPokers"]);
            Assert.Empty(seatOfplayer.GameDateObj["getPokers"]);
        }
        [Fact(DisplayName ="返回对手的Id")]
        public void  GetOpponentId()
        {
            _inningeGame.Start();
           int opponentId= _simpleCards.GetOpponentId(_playerA.Id);
            Assert.Equal(opponentId, _playerB.Id);
        }
    }
}
