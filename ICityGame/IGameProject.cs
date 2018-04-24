using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.GameCity
{
    /// <summary>
    /// 游戏
    /// </summary>
    public interface IGameProject:INotifyClient, IDChangePlayerAccount, IGameCityFactory {        
        /// <summary>
             /// 一局游戏
             /// </summary>
        IInningeGame InningeGame { get; set; }
        /// <summary>
        /// 游戏名称
        /// </summary>
        string ShowName { get; set; }
        /// <summary>
        /// 类名用于用于路由
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 人数下限
        /// </summary>
        int PlayerCountLeast { get; set; }
        /// <summary>
        /// 人数上限
        /// </summary>
        int PlayerCountLimit { get; set; }
        /// <summary>
        /// 处理游戏客户端调用游戏方法请求
        /// </summary>
        /// <param name="askMethodName">方法名称</param>
        /// <param name="jsonInfo">方法参数</param>
        /// <returns>调用结果</returns>
        string ClinetHandler(string askMethodName, Dictionary<string, string> ClientDictionaryInfo);
        /// <summary>
        /// 玩家坐下前事件处理
        /// </summary>
        /// <param name="inningeGame">本局游戏</param>
        /// <param name="e"></param>
        void BeforSitDown(object inningeGame, EventArgs e);
        /// <summary>
        /// 玩家坐下后事件处理
        /// </summary>
        /// <param name="inningeGame"></param>
        /// <param name="e"></param>
        void AfterSitDown(object inningeGame, EventArgs e);
        /// <summary>
        /// 玩家离开座位前事件处理
        /// </summary>
        /// <param name="inningeGame"></param>
        /// <param name="e"></param>
        void BeforPlayerLeave(object inningeGame, EventArgs e);
        /// <summary>
        /// 玩家离开座位后事件处理
        /// </summary>
        /// <param name="inningeGame"></param>
        /// <param name="e"></param>
        void AfterPlayerLeave(object inningeGame, EventArgs e);
        /// <summary>
        /// 能否启动游戏检查
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        bool CheckStart(IInningeGame inningeGame);
        /// <summary>
        /// 游戏启动前事件处理
        /// </summary>
        /// <param name="sender">IInningeGame一局游戏</param>
        /// <param name="e"></param>
        void BeforGameStart(object sender, EventArgs e);
        /// <summary>
        /// 游戏启动处理
        /// </summary>
        /// <param name="sender">IInningeGame一局游戏</param>
        /// <param name="e">暂无用</param>
        void GameStart(object sender, EventArgs e);
        /// <summary>
        /// 能否添加座位检查
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        bool CheckAddSeat(IInningeGame inningeGame);
        /// <summary>
        /// 添加座位前事件处理
        /// </summary>
        /// <param name="sender">IInningeGame一局游戏</param>
        /// <param name="e">暂无用</param>
        void BeforAddSeat(object sender, EventArgs e);
        /// <summary>
        /// 添加座位后事件处理
        /// </summary>
        /// <param name="sender">IInningeGame一局游戏</param>
        /// <param name="e">暂无用</param>
        void AfterAddSeat(object sender, EventArgs e);
        /// <summary>
        /// 玩家能否入座
        /// </summary>
        /// <param name="arg">一局游戏</param>
        /// <returns></returns>
        bool CheckSitDown(IInningeGame inningeGame);
        /// <summary>
        /// 处理游戏异常中断
        /// </summary>
        /// <param name="inningeGame">本局游戏</param>
        /// <param name="e"></param>
        void Stoped(object inningeGame, EventArgs e);
        /// <summary>
        /// 处理游戏结束
        /// </summary>
        /// <param name="inningeGame">本局游戏</param>
        /// <param name="e"></param>
        void GameOver(object inningeGame, EventArgs e);
        /// <summary>
        /// 刷新玩家客户端界面
        /// </summary>
        /// <param name="id">玩家id</param>
        /// <returns></returns>
        object FreshGameFace(int id);
        /// <summary>
        /// 重置游戏
        /// </summary>
        void ResetGame(object inningeGame, EventArgs e);

    }
}
