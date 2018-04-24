using System;
using System.Collections.Generic;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    /// <summary>
    /// 倒下的牌,碰,杠过在一遍的牌
    /// </summary>
    internal class SleepMaJiangsCollection {
        List<MaJiangCollection> SleepMajiangs { get; }
        internal SleepMaJiangsCollection() {
            SleepMajiangs = new List<MaJiangCollection>();
        }
        /// <summary>
        /// 添加碰,杠,吃的一组牌
        /// </summary>
        /// <param name="majiangs"></param>
        internal void Add(MaJiangCollection majiangs) {
            SleepMajiangs.Add(majiangs);
        }
        /// <summary>
        /// 获得一组3张相同的牌(碰牌)
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        internal MaJiangCollection  FindThree(CardModel card) {
            for (int i = 0; i < SleepMajiangs.Count; i++) {
                var majiangs = SleepMajiangs[i];
                if (majiangs.IsThreeSame()&&majiangs[0].IsTheSameWith(card)) {
                    return SleepMajiangs[i];
                }
            }
            return null;
        }
        /// <summary>
        /// 获得一组4张相同的牌(杠牌)
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        internal MaJiangCollection FindFour(CardModel card) {
            for (int i = 0; i < SleepMajiangs.Count; i++) {
                var majiangs = SleepMajiangs[i];
                if (majiangs.IsFourSame() && majiangs[0].IsTheSameWith(card)) {
                    return SleepMajiangs[i];
                }
            }
            return null;
        }
        /// <summary>
        /// 获得可杠的牌的列表
        /// </summary>
        /// <returns></returns>
        internal MaJiangCollection FindAllThree() {
            MaJiangCollection majiangs = new MaJiangCollection();
            for (int i = 0; i < SleepMajiangs.Count; i++) {
                var item = SleepMajiangs[i];
                if (item.IsThreeSame()) {
                    majiangs.Add(item[0]);
                }
            }
            return majiangs;
        }
        /// <summary>
        /// 添加杠牌
        /// </summary>
        /// <param name="card"></param>
        internal Card AddToThree(Card card) {
            var majians = FindThree(card);
            if (!(majians is null)) {
                throw new KeyNotFoundException("没有一组3张相同的牌");
            }
            majians.Add(card);
            return card;
        }
        /// <summary>
        /// 移牌(抢杠)
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        internal Card RemoveCardByGrapGang(CardModel card) {
            MaJiangCollection majiangs = FindFour(card);
            if (majiangs is null ) {
                throw new KeyNotFoundException("没有一组4张相同的牌");
            }
            return majiangs[3];
        }
    }
}