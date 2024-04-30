using HYAutomation.Core.Algorithm.Models;
using HYCommonUtils.FileUtils;
using HYWindowUtils.WPF.VMBaseUtil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HYAutomation.Core.Algorithm
{
    public class AlgorithmConfig : NotifyPropertyObject
    {
        private static readonly object _lockObj = new object();
        private static AlgorithmConfig _instance;
        public static AlgorithmConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new AlgorithmConfig();
                        }
                    }
                }
                return _instance;
            }
        }
        private AlgorithmConfig() { }
        #region 检测条目
        public List<DetectItemConfigModel> DetectItemConfigs
        {
            get
            {
                try
                {
                    List<DetectItemConfigModel> detectItems = ObjectToFileUtil.ReadDataFromJsonFile<List<DetectItemConfigModel>>(AppDomain.CurrentDomain.BaseDirectory + "\\DetectionItems.json");
                    detectItems?.ForEach(a => a.AlgorithmConfig = AlgorithmConfigs?.FirstOrDefault(b => b.AlgorithmName == a.AlgorithmConfig?.AlgorithmName));
                    return detectItems;
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.AddLog(ex.ToString(), true);
                    return null;
                }
            }
            set
            {
                try
                {
                    ObjectToFileUtil.WriteDataToJsonFile(value, AppDomain.CurrentDomain.BaseDirectory + "\\DetectionItems.json");
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.AddLog(ex.ToString(), true);
                }
            }
        }
        public List<AlgorithmConfigModel> AlgorithmConfigs
        {
            get
            {
                try
                {
                    return ObjectToFileUtil.ReadDataFromJsonFile<List<AlgorithmConfigModel>>(AppDomain.CurrentDomain.BaseDirectory + "\\AlgorithmItems.json");
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.AddLog(ex.ToString(), true);
                    return null;
                }
            }
            set
            {
                try
                {
                    ObjectToFileUtil.WriteDataToJsonFile(value, AppDomain.CurrentDomain.BaseDirectory + "\\AlgorithmItems.json");
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.AddLog(ex.ToString(), true);
                }
            }
        }
        #endregion
        /// <summary>
        /// 刷新配置
        /// </summary>
        public event EventHandler RefreshDetecteItemsEvent;
        public void RefreshDetecteItemConfig()
        {
            try
            {
                RefreshDetecteItemsEvent?.Invoke(this, null);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString(), true);
            }
        }
    }
}

