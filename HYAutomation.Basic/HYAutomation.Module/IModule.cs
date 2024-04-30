using System;
using System.Windows;
using System.Windows.Input;

namespace HYAutomation.Module
{
    /// <summary>
    /// 功能模块
    /// </summary>
    public interface IModule : IDisposable
    {
        /// <summary>
        /// 索引号
        /// </summary>
        int ModuleIndex { get; }
        /// <summary>
        /// 是否可用
        /// </summary>
        bool IsAvailable { get; }
        /// <summary>
        /// 是否身份验证
        /// </summary>
        bool IsVerify { get; }
        /// <summary>
        /// 模块名字
        /// </summary>
        string ModuleName { get; }
        /// <summary>
        /// 模块标题
        /// </summary>
        string ModuleTitle { get; }
        /// <summary>
        /// 用户界面
        /// </summary>
        Lazy<Window> UserInterface { get; }
        /// <summary>
        /// 连接命令
        /// </summary>
        ICommand OpenCmd { get; }
    }
}
