using HYAutomation.Core.Views.Models;
using HYWindowUtils.WPF.VMBaseUtil;

namespace HYAutomation.Core.Views.ViewModels
{
    public class StatisticsViewModel : NotifyPropertyObject
    {
        /// <summary>
        /// 统计图模型
        /// </summary>
        public StatisticsChartModel StatisticsChartVM { get; set; } = new StatisticsChartModel();

        private bool _checkedLine;

        public bool CheckedLine
        {
            get { return _checkedLine; }
            set { _checkedLine = value; RaisePropertyChanged(); }
        }
        private bool _checkedColumn;

        public bool CheckedColumn
        {
            get { return _checkedColumn; }
            set { _checkedColumn = value; RaisePropertyChanged(); }
        }
        private bool _checkedPie = true;

        public bool CheckedPie
        {
            get { return _checkedPie; }
            set { _checkedPie = value; RaisePropertyChanged(); }
        }
        public StatisticsViewModel()
        {
            InitialData();
        }
        private void InitialData()
        {
            StatisticsChartVM.SetPieSeriesData(0, 0);
            StatisticsChartVM.SetColumnSeriesData();
            StatisticsChartVM.SetLineSeriesData();
        }
    }
}
