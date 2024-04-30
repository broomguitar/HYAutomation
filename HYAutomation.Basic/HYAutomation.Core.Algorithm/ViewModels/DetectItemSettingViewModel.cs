using HYAutomation.BaseView;
using HYAutomation.Core.Algorithm.Models;
using HYWindowUtils.WPF.VMBaseUtil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace HYAutomation.Core.Algorithm.ViewModels
{
    public class DetectItemSettingViewModel : NotifyPropertyObject
    {
        public DetectItemSettingViewModel()
        {
            InitialData();
        }
        #region DetectItem
        private DetectItemConfigModel _currentDetectItemConfig = new DetectItemConfigModel();
        /// <summary>
        /// 临时条目数据
        /// </summary>
        public DetectItemConfigModel CurrentDetectItemConfig
        {
            get { return _currentDetectItemConfig; }
            set { _currentDetectItemConfig = value; RaisePropertyChanged(); }
        }
        private int _selectedDetectItemConfigIndex = -1;
        /// <summary>
        /// 选中索引
        /// </summary>
        public int SelectedDetectItemConfigIndex
        {
            get { return _selectedDetectItemConfigIndex; }
            set
            {
                _selectedDetectItemConfigIndex = value;
                if (_selectedDetectItemConfigIndex > -1)
                {
                    var selectedData = DetectItemConfigs[_selectedDetectItemConfigIndex];
                    CurrentDetectItemConfig = new DetectItemConfigModel { MarkerBorderBrushStr = selectedData.MarkerBorderBrushStr, DetectItemDesc = selectedData.DetectItemDesc, DetectItemName = selectedData.DetectItemName, AlgorithmConfig = AlgorithmConfigs.FirstOrDefault(a => a.AlgorithmName == selectedData.AlgorithmConfig.AlgorithmName) };
                }
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 检测条目列表
        /// </summary>
        public ObservableCollection<DetectItemConfigModel> DetectItemConfigs { get; set; }
        private ICommand _addDetectItemCmd;
        /// <summary>
        /// 添加条目数据
        /// </summary>
        public ICommand AddDetectItemCmd => _addDetectItemCmd = _addDetectItemCmd ?? new DelegateCommand(() =>
        {
            SelectedDetectItemConfigIndex = -1;
            CurrentDetectItemConfig = new DetectItemConfigModel();
        });
        private ICommand _delSelectedDetectItemCmd;
        /// <summary>
        /// 删除条目数据
        /// </summary>
        public ICommand DelSelectedDetectItemCmd => _delSelectedDetectItemCmd = _delSelectedDetectItemCmd ?? new DelegateCommand<DetectItemConfigModel>(data =>
        {
            if (HYMessageBox.Show("是否要删除该条目", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DetectItemConfigs.Remove(data);
            }
        });
        private ICommand _saveDetectItemCmd;
        /// <summary>
        /// 保存条目数据
        /// </summary>
        public ICommand SaveDetectItemCmd => _saveDetectItemCmd = _saveDetectItemCmd ?? new DelegateCommand(() =>
        {
            if (string.IsNullOrEmpty(CurrentDetectItemConfig.DetectItemName) || string.IsNullOrEmpty(CurrentDetectItemConfig.DetectItemDesc) || string.IsNullOrEmpty(CurrentDetectItemConfig.MarkerBorderBrushStr) || CurrentDetectItemConfig.AlgorithmConfig == null)
            {
                HYMessageBox.Show("数据不完善");
                return;
            }
            if (SelectedDetectItemConfigIndex < 0)
            {
                if (DetectItemConfigs.FirstOrDefault(a => a.DetectItemName == CurrentDetectItemConfig.DetectItemName || a.DetectItemDesc == CurrentDetectItemConfig.DetectItemDesc) != null)
                {
                    HYMessageBox.Show("条目名称或条目描述已存在");
                    return;
                }
                DetectItemConfigs.Add(CurrentDetectItemConfig);
                SelectedDetectItemConfigIndex = -1;
                CurrentDetectItemConfig = new DetectItemConfigModel();
            }
            else
            {
                DetectItemConfigs[SelectedDetectItemConfigIndex] = CurrentDetectItemConfig;
            }
            AlgorithmConfig.Instance.DetectItemConfigs = DetectItemConfigs.ToList();
        });
        #endregion
        #region AlgorithmItem
        private AlgorithmConfigModel _currentAlgorithmConfigModel = new AlgorithmConfigModel();
        /// <summary>
        /// 临时算法数据
        /// </summary>
        public AlgorithmConfigModel CurrentAlgorithmConfig
        {
            get { return _currentAlgorithmConfigModel; }
            set { _currentAlgorithmConfigModel = value; RaisePropertyChanged(); }
        }
        private int _selectedAlgorithmConfigIndex = -1;
        /// <summary>
        /// 选中索引
        /// </summary>
        public int SelectedAlgorithmConfigIndex
        {
            get { return _selectedAlgorithmConfigIndex; }
            set
            {
                _selectedAlgorithmConfigIndex = value;
                if (_selectedAlgorithmConfigIndex > -1)
                {
                    var selectedData = AlgorithmConfigs[_selectedAlgorithmConfigIndex];
                    CurrentAlgorithmConfig = new AlgorithmConfigModel
                    {
                        AlgorithmName = selectedData.AlgorithmName,
                        AlgorithmUrl = selectedData.AlgorithmUrl,
                        AlgorithmType = selectedData.AlgorithmType,
                        AlgorithmUtilsItems = selectedData.AlgorithmUtilsItems
                    };
                }
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 算法数据集
        /// </summary>
        public ObservableCollection<AlgorithmConfigModel> AlgorithmConfigs { get; set; }
        private ICommand _addAlgorithmCmd;
        /// <summary>
        /// 添加算法
        /// </summary>
        public ICommand AddAlgorithmCmd => _addAlgorithmCmd = _addAlgorithmCmd ?? new DelegateCommand(() =>
        {
            SelectedAlgorithmConfigIndex = -1;
            CurrentAlgorithmConfig = new AlgorithmConfigModel();
        });
        private ICommand _delSelectedAlgorithmCmd;
        /// <summary>
        /// 删除算法
        /// </summary>
        public ICommand DelSelectedAlgorithmCmd => _delSelectedAlgorithmCmd = _delSelectedAlgorithmCmd ?? new DelegateCommand<AlgorithmConfigModel>(data =>
        {
            if (HYMessageBox.Show("是否要删除该条目", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                AlgorithmConfigs.Remove(data);
            }
        });
        private ICommand _saveAlgorithmCmd;
        /// <summary>
        /// 保存算法数据
        /// </summary>
        public ICommand SaveAlgorithmCmd => _saveAlgorithmCmd = _saveAlgorithmCmd ?? new DelegateCommand(() =>
        {
            if (string.IsNullOrEmpty(CurrentAlgorithmConfig.AlgorithmName) || string.IsNullOrEmpty(CurrentAlgorithmConfig.AlgorithmUrl))
            {
                HYMessageBox.Show("数据不完善");
                return;
            }
            if (SelectedAlgorithmConfigIndex < 0)
            {
                if (AlgorithmConfigs.FirstOrDefault(a => a.AlgorithmName == CurrentAlgorithmConfig.AlgorithmName) != null)
                {
                    HYMessageBox.Show("算法已存在");
                    return;
                }
                AlgorithmConfigs.Add(CurrentAlgorithmConfig);
                SelectedDetectItemConfigIndex = -1;
                CurrentAlgorithmConfig = new AlgorithmConfigModel();
            }
            else
            {
                AlgorithmConfigs[SelectedAlgorithmConfigIndex] = CurrentAlgorithmConfig;
            }
            var data = DetectItemConfigs.FirstOrDefault(a => a.AlgorithmConfig.AlgorithmName == CurrentAlgorithmConfig.AlgorithmName);
            if (data != null)
            {
                data.AlgorithmConfig = CurrentAlgorithmConfig;
            }
        });
        #endregion
        #region AlgorithmUtils
        private AlgorithmUtilsModel _currentAlgorithmUtils = new AlgorithmUtilsModel();
        /// <summary>
        /// 临时算法参数
        /// </summary>
        public AlgorithmUtilsModel CurrentAlgorithmUtils
        {
            get { return _currentAlgorithmUtils; }
            set { _currentAlgorithmUtils = value; RaisePropertyChanged(); }
        }
        private int _selectedAlgorithmUtilsIndex = -1;

        public int SelectedAlgorithmUtilsIndex
        {
            get { return _selectedAlgorithmUtilsIndex; }
            set
            {
                _selectedAlgorithmUtilsIndex = value;
                if (_selectedAlgorithmUtilsIndex > -1)
                {
                    var selectedData = CurrentAlgorithmConfig.AlgorithmUtilsItems[_selectedAlgorithmUtilsIndex];
                    CurrentAlgorithmUtils = new AlgorithmUtilsModel { AlgorithmUtilsName = selectedData.AlgorithmUtilsName, AlgorithmUtilsValue = selectedData.AlgorithmUtilsValue, IsStandardValue = selectedData.IsStandardValue };
                }
                RaisePropertyChanged();
            }
        }
        private ICommand _delSelectedAlgorithmUtilsCmd;
        /// <summary>
        /// 删除该算法辅助数据
        /// </summary>
        public ICommand DelSelectedAlgorithmUtilsCmd => _delSelectedAlgorithmUtilsCmd = _delSelectedAlgorithmUtilsCmd ?? new DelegateCommand<AlgorithmUtilsModel>(data =>
        {
            if (HYMessageBox.Show("是否要删除该检测内容", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                CurrentAlgorithmConfig.AlgorithmUtilsItems.Remove(data);
            }
        });
        private ICommand _detailsCmd;
        /// <summary>
        /// 查看标准值详情
        /// </summary>
        public ICommand DeteialsCmd => _detailsCmd = _detailsCmd ?? new DelegateCommand<AlgorithmUtilsModel>(data =>
        {
            if (System.IO.File.Exists(data.AlgorithmUtilsValue))
            {
                System.Diagnostics.Process.Start(Core.FileUtils.FilePathUtil.GetDirectoryName(data.AlgorithmUtilsValue));
            }
        });
        private ICommand _addAlgorithmUtilsCmd;
        /// <summary>
        /// 添加算法辅助数据
        /// </summary>
        public ICommand AddAlgorithmUtilsCmd => _addAlgorithmUtilsCmd = _addAlgorithmUtilsCmd ?? new DelegateCommand(
            () =>
            {
                SelectedAlgorithmUtilsIndex = -1;
                CurrentAlgorithmUtils = new AlgorithmUtilsModel();
            }
            );
        private ICommand _saveAlgorithmUtilsCmd;
        /// <summary>
        /// 保存算法辅助参数
        /// </summary>
        public ICommand SaveAlgorithmUtilsCmd => _saveAlgorithmUtilsCmd = _saveAlgorithmUtilsCmd ?? new DelegateCommand(() =>
        {
            if (string.IsNullOrEmpty(CurrentAlgorithmUtils.AlgorithmUtilsName) || string.IsNullOrEmpty(CurrentAlgorithmUtils.AlgorithmUtilsValue))
            {
                HYMessageBox.Show("数据不完善");
                return;
            }
            if (SelectedAlgorithmUtilsIndex < 0)
            {
                if (CurrentAlgorithmConfig.AlgorithmUtilsItems.FirstOrDefault(a => a.AlgorithmUtilsName == CurrentAlgorithmUtils.AlgorithmUtilsName) != null)
                {
                    HYMessageBox.Show("内容名称已存在");
                    return;
                }
                CurrentAlgorithmConfig.AlgorithmUtilsItems.Add(CurrentAlgorithmUtils);
                SelectedAlgorithmUtilsIndex = -1;
                CurrentAlgorithmUtils = new AlgorithmUtilsModel();
            }
            else
            {
                CurrentAlgorithmConfig.AlgorithmUtilsItems[SelectedAlgorithmUtilsIndex] = CurrentAlgorithmUtils;
            }
            AlgorithmConfig.Instance.AlgorithmConfigs = AlgorithmConfigs.ToList();
        });
        #endregion
        private ICommand _closeCmd;
        /// <summary>
        /// 关闭窗体
        /// </summary>
        public ICommand CloseCmd => _closeCmd = _closeCmd ?? new DelegateCommand<Window>(w =>
        {
            AlgorithmConfig.Instance.DetectItemConfigs = DetectItemConfigs.ToList();
            AlgorithmConfig.Instance.AlgorithmConfigs = AlgorithmConfigs.ToList();
            w.Close();
            AlgorithmConfig.Instance.RefreshDetecteItemConfig();
        });
        private void InitialData()
        {
            if (AlgorithmConfig.Instance.DetectItemConfigs != null)
            {
                DetectItemConfigs = new ObservableCollection<DetectItemConfigModel>(AlgorithmConfig.Instance.DetectItemConfigs);
            }
            else
            {
                DetectItemConfigs = new ObservableCollection<DetectItemConfigModel>();

            }
            if (AlgorithmConfig.Instance.AlgorithmConfigs != null)
            {
                AlgorithmConfigs = new ObservableCollection<AlgorithmConfigModel>(AlgorithmConfig.Instance.AlgorithmConfigs);
            }
            else
            {
                AlgorithmConfigs = new ObservableCollection<AlgorithmConfigModel>();

            }
        }
        public IEnumerable<AlgorithmTypeEnum> AlgorithmTypes { get; } = Enum.GetValues(typeof(AlgorithmTypeEnum)).OfType<AlgorithmTypeEnum>();
    }
}
