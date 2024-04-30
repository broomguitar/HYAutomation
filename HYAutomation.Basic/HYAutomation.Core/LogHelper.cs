using System;

namespace HYAutomation.Core
{
    public class LogHelper
    {
        private static readonly object _lockObj = new object();
        private static LogHelper _instance;
        public static LogHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new LogHelper();
                        }
                    }
                }
                return _instance;
            }
        }
        private LogHelper() { }
        /// <summary>
        /// 新日志
        /// </summary>
        public EventHandler<LogInfoModel> NewLogEvent;
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="isError"></param>
        public void AddLog(string msg, bool isError = false)
        {
            if (isError)
            {
                HYCommonUtils.Logger.LogUtil.Instance.Error(msg);
            }
            else
            {
                HYCommonUtils.Logger.LogUtil.Instance.Info(msg);
            }
            NewLogEvent?.Invoke(_instance, new LogInfoModel { Msg = msg, IsError = isError });
        }
        public void AddLog(object o, LogInfoModel loginfoModel)
        {
            if (loginfoModel.IsError)
            {
                HYCommonUtils.Logger.LogUtil.Instance.Error(loginfoModel.Msg);
            }
            else
            {
                HYCommonUtils.Logger.LogUtil.Instance.Info(loginfoModel.Msg);
            }
            NewLogEvent?.Invoke(_instance, loginfoModel);
        }
    }
    /// <summary>
    /// 日志Model
    /// </summary>
    public class LogInfoModel
    {
        public DateTime Time { get; set; } = DateTime.Now;
        public string Msg { get; set; }
        public bool IsError { get; set; }
    }
}
