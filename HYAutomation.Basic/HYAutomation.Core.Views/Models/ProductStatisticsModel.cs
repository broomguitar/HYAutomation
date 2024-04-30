using HYAutomation.Core.Algorithm.Models;
using HYWindowUtils.WPF.VMBaseUtil;
using System;

namespace HYAutomation.Core.Views.Models
{
    public class DetectItemStatisticsModel
    {
        public DetectItemConfigModel DetectItemConfig { get; set; }
        public ProductStatisticsModel ProductStatistics { get; set; } = new ProductStatisticsModel();
    }
    /// <summary>
    /// 统计信息
    /// </summary>
    public class ProductStatisticsModel : NotifyPropertyObject
    {
        private int _totalOutputs;
        /// <summary>
        /// 总产量
        /// </summary>
        public int TotalOutputs
        {
            get { return _totalOutputs; }
            set
            {
                _totalOutputs = value;
                RaisePropertyChanged();
                RaisePropertyChanged("NoOutputs");
                RaisePropertyChanged("PercentOfOK");
                StatisticsData.SetPieSeriesData(_okOutputs, _totalOutputs - _okOutputs);
            }
        }
        private int _okOutputs;
        /// <summary>
        /// 合格量
        /// </summary>
        public int OkOutputs
        {
            get { return _okOutputs; }
            set
            {
                _okOutputs = value;
                RaisePropertyChanged();
                RaisePropertyChanged("NoOutputs");
                RaisePropertyChanged("PercentOfOK");
                StatisticsData.SetPieSeriesData(_okOutputs, _totalOutputs - _okOutputs);
            }
        }
        /// <summary>
        /// 不合格量
        /// </summary>
        public int NoOutputs { get { return TotalOutputs - OkOutputs; } }
        /// <summary>
        /// 合格率
        /// </summary>
        public string PercentOfOK { get { return OkOutputs==0?"0%":(Math.Round(OkOutputs / (TotalOutputs * 1.00) * 100.00, 2) + "%"); } }
        public StatisticsChartModel StatisticsData { get; set; } = new StatisticsChartModel();
    }
}
