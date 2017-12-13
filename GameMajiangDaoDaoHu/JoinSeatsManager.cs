using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    /// <summary>
/// 座位控制器
/// </summary>
    class JoinSeatsManager {
        List<Seat> JoinSeats;
        Seat CurrentSeat { get; set; }
        Seat FirsetSeat { get; set; }
        internal JoinSeatsManager(Seat seat) {
            seat.PreSeat = seat;
            seat.NextSeat = seat;
            CurrentSeat = seat;
            FirsetSeat = seat;
            JoinSeats = new List<Seat> {
                seat
            };
        }
        internal void Add(Seat seat) {
            if (JoinSeats.Count > 0&&JoinSeats.Count<4) {
                Seat nextSeat= CurrentSeat;
                Seat preSeat = CurrentSeat.PreSeat;
                nextSeat.PreSeat = seat;
                preSeat.NextSeat = seat;
                seat.PreSeat = preSeat;
                seat.NextSeat = nextSeat;
                JoinSeats.Add(seat);
            }

        }
        internal  void Remove(Seat seat) {
            if (JoinSeats.Count <= 1) {
                CurrentSeat = null;
                FirsetSeat = null;
                JoinSeats.Clear();
            }
            if (CurrentSeat==seat) {
                CurrentSeat = seat.NextSeat;
            }
            if (FirsetSeat==seat) {
                FirsetSeat = seat.NextSeat;
            }
            seat.PreSeat.NextSeat = seat.NextSeat;
            seat.NextSeat.PreSeat = seat.PreSeat;
            seat.PreSeat = null;
            seat.NextSeat = null;
            JoinSeats.Remove(seat);


        }
        internal List<Seat> GetSeats() {
            return JoinSeats;
        }
       internal void MoveToNext() {
            CurrentSeat = CurrentSeat.NextSeat;
        }
    }
}
