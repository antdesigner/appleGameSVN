using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    /// <summary>
    /// 操作容器
    /// </summary>
    public class OptionorCollection {
        /// <summary>
        /// 操作编号
        /// </summary>
        int OptionorNumber { get; set; }
        List<Optionor> Options { get; set; }
        public OptionorCollection() {
            Options = new List<Optionor>();
        }
        /// <summary>
        /// 清空操作
        /// </summary>
        internal void Clear() {
            OptionorNumber = 0;
            Options.Clear();
        }
        /// <summary>
        /// 添加操作
        /// </summary>
        /// <param name="option"></param>
        internal void Add(Optionor option) {
            option.Number = ++OptionorNumber;
            Options.Add(option);
        }
        /// <summary>
        /// 是否存在操作
        /// </summary>
        /// <param name="optionor"></param>
        /// <returns></returns>
        internal bool HaseOptionor(Optionor optionor) {
            return Options.Exists(o => o.IsTheSameWith(optionor));
        }
        /// <summary>
        /// 是否有此操作编号
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        internal bool HaseOptionor(int number) {
            return Options.Exists(o => o.Number==number);
        }
        /// <summary>
        /// 返回对指定牌的可抄作集合
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        internal OptionorCollection GetOptionorsOf(Card card) {
            OptionorCollection optionorCollection = new OptionorCollection();
            var myOptions = Options.FindAll(c => c.Card.IsTheSameWith(card));
            optionorCollection.Options.AddRange(myOptions);
            return optionorCollection;
        }
    }
}
