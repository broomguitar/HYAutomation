using System.Windows.Controls;

namespace HYAutomation.Core.Views.UserControls.StatisticsCharts
{
    /// <summary>
    /// PieChart.xaml 的交互逻辑
    /// </summary>
    public partial class PieChart : UserControl
    {
        public PieChart()
        {
            InitializeComponent();
        }

        private void Chart_OnDataClick(object sender, LiveCharts.ChartPoint chartPoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartPoint.ChartView;

            //clear selected slice.
            foreach (LiveCharts.Wpf.PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (LiveCharts.Wpf.PieSeries)chartPoint.SeriesView;
            selectedSeries.PushOut = 8;
        }
    }
}
