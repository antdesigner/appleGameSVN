﻿using System;
using Xunit;
using AntDesigner.NetCore.GameCity;
using Moq;

namespace GameCityTest
{
    public class GameCityTest
    {
        ICityManager _cityManager;
        GameCity _gameCity_ticket_0;
        GameCity _gameCity_ticket_5;
        MockRepository _mock;
        Room _room;
        IPlayerJoinRoom _player;
        IInningeGame _inningGame;
        Mock<IPlayerJoinRoom> _playerFactory;
        Mock<Room> _roomFactory;
        RoomConfig _roomConfig;
        public GameCityTest()
        {
            _mock = new MockRepository(MockBehavior.Default);
            _playerFactory = _mock.Create<IPlayerJoinRoom>();
            _roomFactory = _mock.Create<Room>();
            _cityManager = _mock.Create<ICityManager>().Object;
            _gameCity_ticket_0 = new GameCity("拉斯维加斯", _cityManager);
            _gameCity_ticket_5= new GameCity("拉斯维加斯", _cityManager,ticketPrice_:5);
            _player = _mock.Create<IPlayerJoinRoom>().Object;
            _inningGame = _mock.Create<IInningeGame>().Object;
             _roomConfig = new RoomConfig(_inningGame);
            _room = new Room(DateTime.Now.AddHours(1), 10, _inningGame, _player, ticketPrice_: 50);
        }
        [Fact(DisplayName = "进入游戏城验证身份委托_通过")]
        public void DCheckTicket_allowPass()
        {
            bool Did = false;
            _gameCity_ticket_0.DCheckTicket += delegate { Did= true; return true; };
            _gameCity_ticket_0.AddPlayer(_mock.Create<IJoinGameCityTicket>().Object);
            Assert.True(Did);
        }
        [Fact(DisplayName = "进入游戏城验证身份委托_不通过")]
        public void DCheckTicket_notAllowPass()
        {
            bool Did = false;
            _gameCity_ticket_0.DCheckTicket += delegate { Did = true; return false; };
            _gameCity_ticket_0.AddPlayer(_mock.Create<IJoinGameCityTicket>().Object);
            Assert.True(Did);
        }
        //[Fact("加入黑名单者禁止入内")]
        //Fact("开房钱不够事件")]
        [Theory(DisplayName = "玩家开房_玩家账户不足")]
        [InlineData(0)]
        [InlineData(1)]
        public void AddRoom_accountNotEnough_fail(decimal mony_)
        {
            bool _playAddRoomSuccess = true;
          
           _playerFactory.Setup(p => p.Account).Returns(mony_);
            _roomConfig.TicketPrice = 50;
            var room = new Room(_playerFactory.Object, _roomConfig);
            int preCount = _gameCity_ticket_5.Rooms.Count;
            bool success = true;
            _gameCity_ticket_5.FailAddRoomNotEnoughMoney += delegate{ success = false; };
            _gameCity_ticket_5 .AddRoom(room);
            Assert.True(_gameCity_ticket_5.Rooms.Count == preCount);
            Assert.False(success);
            Assert.False(_playAddRoomSuccess, "没能触发玩家开房失败对象事件");
           
        }
        [Theory(DisplayName = "触发开房前事件")]
        [InlineData(0)]
        [InlineData(10)]
        public void AddRoom_failed2(decimal money_)
        {
            GameCity gameCity = new GameCity("收费开房", _cityManager, ticketPrice_: 50);
            int preCount = gameCity.Rooms.Count;
            bool success = true;
            gameCity.FailAddRoomNotEnoughMoney += delegate { success = false; };
           // gameCity.DAddRoomChek += delegate { return true; };
            gameCity.AddRoom(_room);
            Assert.False(success);
        }
        [Theory(DisplayName = "玩家开房(需要付款)_执行扣款")]
        [InlineData(10)]
        public void AddRoom_deductMoney(decimal money_)
        {
            var gameCityConfig = new GameCityConfig()
            {
                TicketPrice = 5
            };
            gameCityConfig.DAddRoomChek += delegate { return true; };
            GameCity gameCity = new GameCity(_cityManager, gameCityConfig);
           _playerFactory.Setup(p => p.Account).Returns(money_);
            _playerFactory.Setup(p => p.DecutMoney(5,""));
            var room = new Room(_playerFactory.Object, _roomConfig);
            gameCity.AddRoom(room);
            _playerFactory.Verify(p => p.DecutMoney(gameCity.TicketPrice,""));
        }
        [Theory(DisplayName = "玩家开房(需要付款)_开房成功")]
        [InlineData(10)]
        public void AddRoom_needMoney_success(decimal money_)
        {
            var gameCity = _gameCity_ticket_5;
            _playerFactory.Setup(p => p.Account).Returns(money_);
            _playerFactory.Setup(p => p.DecutMoney(5,""));
            var room = new Room(_playerFactory.Object, _roomConfig);
            gameCity.AddRoom(room);
        }
        [Fact(DisplayName = "玩家开房(不需付款)_完成开房")]
        public void AddRoom()
        {
          
            int preCount = _gameCity_ticket_0.Rooms.Count;
            _gameCity_ticket_0.AddRoom(_room);
            Assert.True(_gameCity_ticket_0.Rooms.Count - preCount == 1, "房间数量未增加1");
            Assert.Contains(_room, _gameCity_ticket_0.Rooms);
        }
        [Theory(DisplayName = "触发开房成功后事件")]
        [InlineData(10,0)]
        [InlineData(10,5)]
        public void AddRoom_success(decimal money_,decimal ticket_)
        {
  
            _playerFactory.Setup(p => p.Account).Returns(money_);
            _playerFactory.Setup(p => p.DecutMoney(ticket_,"")).Returns(true);
            var player = _playerFactory.Object;
            var gameCityConfig = new GameCityConfig()
            {
                TicketPrice = ticket_
            };
            GameCity gameCity = new GameCity(_cityManager, gameCityConfig);
            var room = new Room(player, new RoomConfig(_inningGame));
            bool result = false;
            gameCity.AfterAddRoomHandler += delegate { result = true; };
            gameCity.AddRoom(room);
            Assert.True(result);
           
        }
        [Fact(DisplayName ="删除房间")]
        public void DeleteRoom_beforEvent()
        {
            bool raisEvent = false;
            _gameCity_ticket_0.BeforDeleteRoomHandler += delegate { raisEvent = true; };
            _gameCity_ticket_0.AddRoom(_room);
            int preCount = _gameCity_ticket_0.Rooms.Count;
            _gameCity_ticket_0.RemoveRoom(_room);
            Assert.True(raisEvent, "删除房间前事件");
            Assert.DoesNotContain<IRoom>(_room, _gameCity_ticket_0.Rooms);
            Assert.True(_gameCity_ticket_0.Rooms.Count - preCount==-1, "房间数量未减少1");
        }
        [Fact(DisplayName ="删除房间前触发检查")]
        public void DeleteRoom_DelegateForCheck()
        {
            bool _canDeleteRoomCheck=false;
            _room.DCanDeleteRoomCheck += delegate { _canDeleteRoomCheck = true; return true; };
            _gameCity_ticket_0.AddRoom(_room);
            _gameCity_ticket_0.RemoveRoom(_room);
            Assert.True(_canDeleteRoomCheck);
        }
        [Fact(DisplayName ="根据Id找到房间")]
        public void FindRoomById()
        {
            Room myRoom = _room;
            myRoom.Id = "1";
            _gameCity_ticket_0.AddRoom(myRoom);
            Assert.NotNull(_gameCity_ticket_0.FindRoomById("1"));
            Assert.Null(_gameCity_ticket_0.FindRoomById("-1"));
        }
        [Fact(DisplayName = "没有房间可提供")]
        public IRoom ProvideRandomRoom_null()
        {
            Assert.Null(_gameCity_ticket_0.ProvideRandomFreeRoom());
            return _gameCity_ticket_0.ProvideRandomFreeRoom();
        }
        [Fact(DisplayName ="随机提供一个房间")]
        public  IRoom  ProvideRandomRoom()
        {
            AddRoom();
            Assert.NotNull(_gameCity_ticket_0.ProvideRandomRoom());
           return   _gameCity_ticket_0.ProvideRandomRoom();
        }
        [Fact(DisplayName = "随机提供一个人数未满房间")]
        public IRoom ProvideRandomRoom_notFull()
        {
            _gameCity_ticket_0.AddAutoRoom(_inningGame, 1, 1);
            Assert.Null(_gameCity_ticket_0.ProvideRandomFreeRoom());
            return _gameCity_ticket_0.ProvideRandomFreeRoom();
        }
        [Fact(DisplayName = "随机提供一个免费且人数未满房间")]
        public IRoom ProvideRandomRoom_notFullAndFree()
        {
            _gameCity_ticket_0.AddAutoRoom(_inningGame, 5, 10);
            foreach (var item in _gameCity_ticket_0.Rooms)
            {
                item.SetTicketPrice(10);
            }
            Assert.Null(_gameCity_ticket_0.ProvideRandomFreeRoom());
            return _gameCity_ticket_0.ProvideRandomFreeRoom();
        }
       [Fact(DisplayName ="游戏城自动创建ID")]
       public void GameCity_install_autoId()
        {
            var gameCity = new GameCity(_cityManager, new GameCityConfig());
            Assert.NotEmpty(_gameCity_ticket_0.Id);
            Assert.NotEmpty(gameCity.Id);
        }
        [Fact(DisplayName = "委托检查是否能开房_能")]
        public void AddRoom_DelegateCheck_yes()
        {
            bool addRoomCheck = false ;
            _gameCity_ticket_0.DAddRoomChek += delegate { addRoomCheck = true; return true; };
            int preCount = _gameCity_ticket_0.Rooms.Count;
            _gameCity_ticket_0.AddRoom(_room);
            Assert.True (addRoomCheck);
        }
        [Fact(DisplayName = "委托检查是否能开房_不能")]
        public void AddRoom_DelegateCheck_no()
        {
            bool addRoomCheck = true;
            _gameCity_ticket_0.DAddRoomChek += delegate { addRoomCheck = false; return false; };
            int preCount = _gameCity_ticket_0.Rooms.Count;
            _gameCity_ticket_0.AddRoom(_room);
            Assert.False(addRoomCheck);
        }

    }
}
