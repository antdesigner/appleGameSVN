using AntDesigner.GameCityBase;
using System.Collections.Generic;

namespace GameCitys.DomainService
{
    public interface INoticeService
    {
        Notice GetNoticeById(int Id);
        void ModifyNotice(Notice notice, string content);
        void PublishNotice(Notice notice);
        void RemoveNotice(Notice notice);
        IList<Notice> AllNotices();
        IList<Notice> GetNotices(int n);
    }
}