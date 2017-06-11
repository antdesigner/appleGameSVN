using AntDesigner.GameCityBase;
using AntDesigner.GameCityBase.interFace;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameCitys.DomainService
{
    /// <summary>
    /// 公告管理
    /// </summary>
    public class NoticeService : ABIStorehouse, INoticeService
    {

        public NoticeService([FromServices]IStorehouse istoreHose):base(istoreHose)
        {
        }
       /// <summary>
       /// 发布公告
       /// </summary>
       /// <param name="notice">公告</param>
        public void PublishNotice(Notice notice)
        {
            _storeHouse.AddEntity<Notice>(notice);
            _storeHouse.SaveChanges();

        }
        /// <summary>
        /// 修改公告
        /// </summary>
        /// <param name="notice">公告</param>
        /// <param name="content">新内容</param>
        public void ModifyNotice(Notice notice, string content)
        {
            notice.Content = content;
            _storeHouse.SaveChanges();
            return;
        }
 
        /// <summary>
        /// 删除公告
        /// </summary>
        /// <param name="notice">公告</param>
        public void RemoveNotice(Notice notice)
        {
            _storeHouse.RemoveEntity<Notice>(notice);
            _storeHouse.SaveChanges();
            return;
        }
        /// <summary>
        /// 返回全部公告
        /// </summary>
        /// <returns>全部公告</returns>
        public IList<Notice> AllNotices()
        {
            return _storeHouse.GetAllNotices();
        }

        public IList<Notice> GetNotices(int n)
        {
           return _storeHouse.GetEntitys<Notice>(n);
        }

        public Notice GetNoticeById(int id)
        {
            return _storeHouse.GetEntityById<Notice>(id);
        }
    }
}
