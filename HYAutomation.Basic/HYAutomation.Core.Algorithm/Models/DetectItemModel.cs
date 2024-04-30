using HYAutomation.BaseView;
using HYWindowUtils.WPF.VMBaseUtil;
using System;
using System.Windows;
using System.Windows.Input;

namespace HYAutomation.Core.Algorithm.Models
{
    /// <summary>
    /// 检测条目
    /// </summary>
    public class DetectItemModel : NotifyPropertyObject
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        /// <summary>
        /// 检测相机
        /// </summary>
        public string CameraName { get; set; }
        private DetectItemConfigModel _detectItemConfig = new DetectItemConfigModel();
        /// <summary>
        /// 检测条目
        /// </summary>
        public DetectItemConfigModel DetectItemConfig
        {
            get { return _detectItemConfig; }
            set
            {
                _detectItemConfig = value;
                if (_detectItemConfig == null)
                {

                }
                RaisePropertyChanged();
            }
        }
        private Int32Rect _detectItemReion;
        /// <summary>
        /// 检测区域
        /// </summary>
        public Int32Rect DetectItemRegion
        {
            get { return _detectItemReion; }
            set { _detectItemReion = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 位置坐标
        /// </summary>
        public Point DetectItemLocation { get; set; }
        private bool _isUsing = true;
        /// <summary>
        /// 是否使用
        /// </summary>
        public bool IsUsing
        {
            get { return _isUsing; }
            set
            {
                _isUsing = value;
                RaisePropertyChanged("TargetVisibility");
            }
        }
        public Visibility TargetVisibility => IsUsing ? Visibility.Visible : Visibility.Hidden;
        private bool? _retIsOk;
        /// <summary>
        /// 检测结果
        /// </summary>
        public bool? RetIsOk
        {
            get { return _retIsOk; }
            set { _retIsOk = value; RaisePropertyChanged(); }
        }
        private string result;

        public string Result
        {
            get { return result; }
            set { result = value; RaisePropertyChanged(); }
        }
        private ICommand _showResultCmd;

        public ICommand ShowResultCmd => _showResultCmd = _showResultCmd ?? new DelegateCommand(() => { HYMessageBox.Show(Result); });

        /// <summary>
        /// 型号编号
        /// </summary>
        public string TypeCode { get; set; }

    }
}
