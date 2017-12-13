using System;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    /// <summary>
    /// 令牌控制器
    /// </summary>
    internal class TakonManager {
        /// <summary>
        /// 令牌
        /// </summary>
        OptionToken Token { get; set; }
        internal event EventHandler RecievedToken;

        /// <summary>
        /// 令牌是否已经颁发出去
        /// </summary>
        internal bool HaveToken { get;private set; }
        /// <summary>
        /// 颁发令牌
        /// </summary>
        /// <param name="seat"></param>
        void ConferTokenTo(Seat seat) {
            lock (this) {
                if (HaveToken) {
                    seat.SetToken(Token);
                    HaveToken = false;
                }
                else {
                    throw new Exception("令牌已经颁发出去了");
                }
            }
        }
        /// <summary>
        /// 接受座位抛出的令牌
        /// </summary>
        /// <param name="seat"></param>
        internal void RecieveTokenFrom(Seat seat) {
            lock (this) {
                if (HaveToken) {
                    throw new Exception("不能重复接受令牌");
                }
                HaveToken = true;
            }
            OnRecievedToken();
        }
        void OnRecievedToken() {
            RecievedToken?.Invoke(this, new EventArgs());
        }
    }
}