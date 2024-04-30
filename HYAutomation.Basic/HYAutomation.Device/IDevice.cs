using System;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Input;

namespace HYAutomation.Device
{
    public interface IDevice : IDisposable
    {
        string DeviceId { get; set; }
        /// <summary>
        /// 索引号
        /// </summary>
        int DeviceIndex { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        bool IsAvailable { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        string DeviceName { get; set; }
        /// <summary>
        /// 设备描述
        /// </summary>
        string DeviceDesc { get; set; }
        /// <summary>
        /// 用户界面
        /// </summary>
        UIElement UserInterface { get; }
        /// <summary>
        /// 是否在线
        /// </summary>
        bool IsOnline { get; }
        /// <summary>
        /// 设备类型
        /// </summary>
        DeviceEnum DeviceEnum { get; }
        /// <summary>
        /// 图标
        /// </summary>
        object Icon { get; }
        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        bool Connect();
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        bool DisConnect();
        /// <summary>
        /// 数据堆栈
        /// </summary>
        ConcurrentQueue<object> DataQueue { get; }
        /// <summary>
        /// 结果推送事件
        /// </summary>
        event EventHandler<object> PushResultEvent;
        /// <summary>
        /// 连接命令
        /// </summary>
        ICommand ConnectCmd { get; }
    }
}
