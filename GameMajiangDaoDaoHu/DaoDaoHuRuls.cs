using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    /// <summary>
    /// 倒倒胡规则
    /// </summary>
    class DaoDaoHuRuls : IMajiangRuls {
        /// <summary>
        /// 存放要牌操作
        /// </summary>
        /// <param name="handCards"></param>
        /// <returns></returns>
        public OptionorCollection FreshOptions(HandCardManager handCards) {
            handCards.Optionors.Clear();
            MaJiangCollection huCards = GetHuCards(handCards.StandCards);
            if (huCards.Count>0) {
                for (int i = 0; i < huCards.Count; i++) {
                    Optionor optionor = new Optionor(new OptionHu(), huCards[i]);
                    handCards.AddOptionor(optionor);
                }
            }
            MaJiangCollection PendCards = handCards.GetPengCards();
            if (PendCards.Count>0) {
                for (int i = 0; i < PendCards.Count; i++) {
                    Optionor optionor = new Optionor(new OptionPeng(), PendCards[0]);
                    handCards.AddOptionor(optionor);
                }
            }
            MaJiangCollection GangCards = handCards.GetGangCards();
            if (GangCards.Count >0) {
                for (int i = 0; i < GangCards.Count ; i++) {
                    Optionor optionor = new Optionor(new OptionGang(), GangCards[i]);
                    handCards.AddOptionor(optionor);
                }
            }
            return handCards.Optionors;
        }
        /// <summary>
        /// 获得可以胡的牌
        /// </summary>
        /// <returns></returns>
        public virtual MaJiangCollection GetHuCards(MaJiangCollection maJiangs) {
            List<Card> HuCards = new MaJiangCollection();
            MaJiangCollection doubleCards;
            doubleCards = maJiangs.FindTheSameCards(2);
            List<MaJiangCollection> matchedCards = new List<MaJiangCollection>();
            if (doubleCards.Count > 0) {
                for (int i = 0; i < doubleCards.Count; i++) {
                    MaJiangCollection tempMajiangs = new MaJiangCollection();
                    tempMajiangs.AddRange(maJiangs);
                    tempMajiangs = tempMajiangs.RemoveCountOf(doubleCards[i], 2);
                    List<MaJiangCollection> matchedCardsNeed = tempMajiangs.GetMatchedCardsCollection();
                    if (matchedCardsNeed.Count == 1) {
                        matchedCards.Add(matchedCardsNeed[0]);
                    }
                    List<MaJiangCollection> matchedCardsNeedDesc = tempMajiangs.GetMatchedCardsDescCollection();
                    if (matchedCardsNeedDesc.Count == 1) {
                        matchedCards.Add(matchedCardsNeedDesc[0]);
                    }
                }
            }
            for (int i = 0; i < maJiangs.Count; i++) {
                MaJiangCollection tempMajiangs = new MaJiangCollection();
                tempMajiangs.AddRange(maJiangs);
                MaJiangCollection tempOneCard = new MaJiangCollection {
                    maJiangs[i]
                };
                tempMajiangs = tempMajiangs.RemoveCountOf(maJiangs[i], 1);

                List<MaJiangCollection> matchedCardsNeed = tempMajiangs.GetMatchedCardsCollection();
                if (matchedCardsNeed.Count == 0) {
                    matchedCards.Add(tempOneCard);
                }
                List<MaJiangCollection> matchedCardsNeedDesc = tempMajiangs.GetMatchedCardsDescCollection();
                if (matchedCardsNeedDesc.Count == 0) {
                    matchedCards.Add(tempOneCard);
                }
            }
            if (matchedCards.Count > 0) {
                for (int i = 0; i < matchedCards.Count; i++) {
                    HuCards.AddRange(matchedCards[i].GetMatchedCards());
                }
            }
            MaJiangCollection husdistnct = new MaJiangCollection();
            if (HuCards.Count == 0) {
                return husdistnct;
            }
            int k = 0;
            do {
                husdistnct.Add(HuCards[k]);
                k++;
            } while (!husdistnct.Exists(c => c.IsTheSameWith(HuCards[k]))
            && k < HuCards.Count
                );
            return husdistnct;
        }
        /// <summary>
        /// 定义操作名称
        /// </summary>
        /// <param name="name"></param>
        /// <param name="card"></param>
        /// <returns></returns>
        public Optionor GetOptionorByName(string name,CardModel cardM) {
            Optionor optionor = null;
            switch (name) {
                case "碰": optionor=new Optionor(new OptionPeng(), cardM);
                    break;
                case "杠": optionor = new Optionor(new OptionGang(), cardM);
                    break;
                case "胡":optionor = new Optionor(new OptionHu(), cardM);
                    break;
                default:
                    break;
            }
            return optionor;
        }
    }
}

