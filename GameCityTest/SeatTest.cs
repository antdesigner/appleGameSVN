using System;
using Xunit;
using AntDesigner.NetCore.GameCity;
using Moq;

namespace GameCityTest
{
    public class SeatTest
    {
        IPlayerJoinRoom _player;
        IPlayerJoinRoom _player2;
        public SeatTest()
        {
            Mock_Player();
        }
        private void Mock_Player()
        {
            Mock<IPlayerJoinRoom> _IPlayerJoinRoomFactory = new Mock<IPlayerJoinRoom>();
            _IPlayerJoinRoomFactory.SetupAllProperties();
            _player = _IPlayerJoinRoomFactory.Object;
            _player.Id = 1;
            _player2 = new Mock<IPlayerJoinRoom>().SetupAllProperties().Object;
            _player2.Id = 2;

        }
        public ISeat _ISeatCreator()
        {
            ISeat seat = new Seat(null);
            return seat;
        }

        [Fact(DisplayName = "玩家坐下_玩家Id=座位无人")]
        public void PlayerSetDown_()
        {
            ISeat seat = _ISeatCreator();

            seat.PlayerSitDown(_player);

            Assert.True(seat.IsEmpty == false);
            Assert.True(seat.IPlayer ==_player, "座位玩家Id不对");
        }
        [Fact(DisplayName = "玩家坐下_玩家Id=座位有人")]
        public void PlayerSetDown_PlayerExist()
        {
            ISeat seat = _ISeatCreator();

            seat.PlayerSitDown(_player);

            Assert.Throws<Exception>(() => seat.PlayerSitDown(_player2));
        }
        [Fact(DisplayName = "玩家离开座位")]
        public void PlayerLeaveSeat()
        {
            ISeat seat = _ISeatCreator();

            seat.PlayerSitDown(_player);
            seat.PlayLeave();

            Assert.True(seat.IsEmpty == true);
            Assert.True(seat.IPlayer  == null, "座位玩家Id=0");
        }
        [Fact(DisplayName = "委托检查能不能入座")]
        public void PlayerSeatDow_yes()
        {
            ISeat seat = _ISeatCreator();

            seat.DCheckSitDown += delegate { return true; };
            seat.PlayerSitDown(_player);

            Assert.True(seat.IPlayer  ==_player);
        }
        [Fact(DisplayName = "委托检查能不能入座")]
        public void PlayerSeatDow_no()
        {
            ISeat seat = _ISeatCreator();

            seat.DCheckSitDown += delegate { return false; };
            seat.PlayerSitDown(_player);

            Assert.False(seat.IPlayer == _player);
            Assert.True(seat.IsEmpty);
        }
        [Fact(DisplayName = "触发坐下事件")]
        public void PlayerSeatDown_beforEvent()
        {
            ISeat seat = _ISeatCreator();
            bool beforSitDownEvent_ = false;
            bool afterSitDownEvent_ = false;

            seat.BeforSitDownHandler += delegate { beforSitDownEvent_ = true; };
            seat.AfterSitDownHandler += delegate { afterSitDownEvent_ = true; };
            seat.PlayerSitDown(_player);

            Assert.True(beforSitDownEvent_);
            Assert.True(afterSitDownEvent_);
        }
        [Fact(DisplayName = "触发离开座位事件")]
        public void PlayerLeave_beforEvent()
        {
            ISeat seat = _ISeatCreator();
            bool beforPlayerLeave_= false;
            bool afterPlayerLeave_ = false;

            seat.BeforPlayerLeaveHandler += delegate { beforPlayerLeave_ = true; };
            seat.AfterPlayerLeaveHandler += delegate { afterPlayerLeave_ = true; };
            seat.PlayerSitDown(_player);
            seat.PlayLeave();

            Assert.True(beforPlayerLeave_);
            Assert.True(afterPlayerLeave_);
        }
    }
}
