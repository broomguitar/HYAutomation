using HYWindowUtils.WPF.VMBaseUtil;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HYAutomation.Core.Views.Models
{
    public class StatisticsChartModel : NotifyPropertyObject
    {
        public StatisticsChartModel()
        {
            InitialPieSeriesData();
        }
        #region 属性
        private string _selectedItem;

        public string SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; RaisePropertyChanged(); }
        }
        private SeriesCollection _lineSeriesCollection;
        /// <summary>
        /// 折线图集合
        /// </summary>
        public SeriesCollection LineSeriesCollection
        {
            get
            {
                return _lineSeriesCollection;
            }
            set
            {
                _lineSeriesCollection = value;
                RaisePropertyChanged();
            }
        }
        private SeriesCollection _colunmSeriesCollection;
        /// <summary>
        /// 柱状图集合
        /// </summary>
        public SeriesCollection ColunmSeriesCollection
        {
            get
            {
                return _colunmSeriesCollection;
            }
            set
            {
                _colunmSeriesCollection = value;
                RaisePropertyChanged();
            }
        }
        private SeriesCollection _pieSeriesCollection;
        /// <summary>
        /// 饼状图集合
        /// </summary>
        public SeriesCollection PieSeriesCollection
        {
            get
            {
                return _pieSeriesCollection;
            }
            set
            {
                _pieSeriesCollection = value;
                RaisePropertyChanged();
            }
        }
        private List<string> _lineXLabels;
        /// <summary>
        /// 折线图坐标
        /// </summary>
        public List<string> LineXLabels
        {
            get { return _lineXLabels; }
            set { _lineXLabels = value; RaisePropertyChanged(); }
        }
        private List<string> _columnXLables;
        /// <summary>
        /// 柱状图坐标
        /// </summary>
        public List<string> ColumnXLabels
        {
            get { return _columnXLables; }
            set { _columnXLables = value; }
        }
        #endregion
        #region 方法
        public void SetLineSeriesData()
        {
            LineXLabels = new List<string>();
            LineSeriesCollection = new SeriesCollection();
            List<string> titles = new List<string> { "合格", "不合格" };
            List<List<double>> values = new List<List<double>>
            {
                new List<double> { 30, 40, 60 },
                new List<double> { 20, 10, 50 },
                new List<double> { 10, 50, 30 }
            };
            List<string> _dates = new List<string>();
            _dates = GetCurrentMonthDates();
            for (int i = 0; i < titles.Count; i++)
            {
                LineSeries lineseries = new LineSeries();
                lineseries.DataLabels = true;
                lineseries.Title = titles[i];
                lineseries.PointForeground = System.Windows.Media.Brushes.White;
                lineseries.Foreground = System.Windows.Media.Brushes.White;
                lineseries.Values = new ChartValues<double>(values[i]);
                LineXLabels.Add(_dates[i]);
                LineSeriesCollection.Add(lineseries);
            }
        }

        public void SetColumnSeriesData()
        {
            ColumnXLabels = new List<string>();
            ColunmSeriesCollection = new SeriesCollection();
            List<string> titles = new List<string> { "门齐", "门缝", "外观检", "Logo" };
            List<double> columnValues = new List<double> { 10, 70, 15, 5 };

            for (int i = 0; i < titles.Count; i++)
            {
                ColumnXLabels.Add(titles[i]);
            }
            ColumnSeries colunmseries = new ColumnSeries();
            colunmseries.DataLabels = true;
            colunmseries.Title = "检测份额";
            colunmseries.FontSize = 12;
            colunmseries.Foreground = System.Windows.Media.Brushes.White;
            colunmseries.Values = new ChartValues<double>(columnValues);
            ColunmSeriesCollection.Add(colunmseries);

        }
        private void InitialPieSeriesData()
        {
            PieSeriesCollection = new SeriesCollection();
            List<string> titles = new List<string> { "OK", "NG" };
            for (int i = 0; i < titles.Count; i++)
            {
                PieSeries series = new PieSeries();
                series.DataLabels = false;
                series.FontSize = 12;
                series.Foreground = System.Windows.Media.Brushes.White;
                series.Title = titles[i];
                series.Values = new ChartValues<ObservableValue> { new ObservableValue(1 - i) };
                PieSeriesCollection.Add(series);
            }
        }
        public void SetPieSeriesData(int OK, int NG)
        {
            if (OK + NG < 1) return;
            PieSeriesCollection[0].Values.Cast<ObservableValue>().ElementAt(0).Value = OK;
            PieSeriesCollection[1].Values.Cast<ObservableValue>().ElementAt(0).Value = NG;
        }

        /// <summary>
        /// 获取当前月的每天的日期
        /// </summary>
        /// <returns>日期集合</returns>
        List<string> GetCurrentMonthDates()
        {
            List<string> dates = new List<string>();
            DateTime dt = DateTime.Now;
            int year = dt.Year;
            int mouth = dt.Month;
            int days = DateTime.DaysInMonth(year, mouth);
            //本月第一天时间      
            DateTime dt_First = dt.AddDays(1 - (dt.Day));
            dates.Add(String.Format("{0:d}", dt_First.Date));

            for (int i = 1; i < days; i++)
            {
                DateTime temp = dt_First.AddDays(i);
                dates.Add(String.Format("{0:d}", temp.Date));
            }
            return dates;
        }
        #endregion
    }
}
