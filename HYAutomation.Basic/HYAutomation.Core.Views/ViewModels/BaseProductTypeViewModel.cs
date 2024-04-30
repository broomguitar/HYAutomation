using HYAutomation.BaseView;
using HYAutomation.BLL;
using HYAutomation.Core.Algorithm.Models;
using HYAutomation.Core.Views.Extensions;
using HYAutomation.Core.Views.Models;
using HYAutomation.Device;
using HYAutomation.Entity_Base;
using HYCommonUtils.SerializationUtils;
using HYWindowUtils.WPF.VMBaseUtil;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace HYAutomation.Core.Views.ViewModels
{
    public abstract class BaseProductTypeViewModel<T, T1> : NotifyPropertyObject where T : ProductTypeModel, new() where T1 : ProductTypeInfo
    {
        public BaseProductTypeViewModel()
        {
            InitialData();
        }
        protected virtual void InitialData()
        {
            try
            {
                Search("");
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString(), true);
            }
        }
        private ICommand _closeCmd;

        public ICommand CloseCmd => _closeCmd = _closeCmd ?? new DelegateCommand<Window>(Close);
        protected virtual void Close(Window w)
        {
            TypeImage = null;
            w.Close();
        }
        private ImageSource _typeImage;
        /// <summary>
        /// 图片
        /// </summary>
        public ImageSource TypeImage
        {
            get { return _typeImage; }
            set
            {
                _typeImage = value;
                RaisePropertyChanged();
            }
        }
        private ProductTypeDetailModel _currentCameraDetail;

        public ProductTypeDetailModel CurrentCameraDetail
        {
            get { return _currentCameraDetail; }
            set
            {
                TypeImage = null;
                if (_currentCameraDetail != null)
                {
                    _currentCameraDetail.DetectItems.CollectionChanged -= DetectionItems_CollectionChanged;
                }
                _currentCameraDetail = value;
                if (_currentCameraDetail != null)
                {

                    _currentCameraDetail.DetectItems.CollectionChanged += DetectionItems_CollectionChanged;
                }
                if (System.IO.File.Exists(_currentCameraDetail?.TemplateImagePath))
                {
                    TypeImage = ImageHelper.GetBitmapImage(_currentCameraDetail.TemplateImagePath);
                }
                RaisePropertyChanged();
            }
        }
        private IDevice _selectedCamera;

        public IDevice SelectedCamera
        {
            get { return _selectedCamera; }
            set
            {
                _selectedCamera = value;
                if (_selectedCamera != null)
                {
                    if (_currentType != null)
                    {
                        CurrentCameraDetail = _currentType.Details.FirstOrDefault(a => a.CameraName == _selectedCamera.DeviceName);
                    }
                }
                else
                {
                    CurrentCameraDetail = null;
                }
                RaisePropertyChanged();
            }
        }

        public IEnumerable<IDevice> Cameras { get; set; } = Core.Views.GlobalManager.Instance.Devices?.Where(a => a.IsAvailable && (a.DeviceEnum == DeviceEnum.AreaCamera || a.DeviceEnum == DeviceEnum.LinearCamera || a.DeviceEnum == DeviceEnum._3DCamera));
        protected virtual void DetectionItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (sender is ObservableCollection<DetectItemModel> detectItems)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    try
                    {
                        SelectDetectItemTypeView selectDetectItemView = new SelectDetectItemTypeView();
                        bool b = selectDetectItemView.ShowDialog() == true;
                        if (b && detectItems.FirstOrDefault(a => a.DetectItemConfig?.DetectItemName == selectDetectItemView.SelectedDetectItem?.DetectItemName) == null)
                        {
                            foreach (var item in e.NewItems.OfType<DetectItemModel>())
                            {
                                try
                                {
                                    item.CameraName = CurrentCameraDetail.CameraName;
                                    item.DetectItemConfig = selectDetectItemView.SelectedDetectItem;
                                    if (System.IO.File.Exists(CurrentCameraDetail.TemplateImagePath))
                                    {
                                        System.Windows.Media.Imaging.BitmapSource TypeImage = Core.ImageHelper.GetBitmapImage(CurrentCameraDetail.TemplateImagePath);
                                        string detectItemPath = CurrentCameraDetail.TemplateImagePath;
                                        if (!item.DetectItemRegion.IsEmpty)
                                        {
                                            System.Windows.Media.Imaging.BitmapSource bitmapsource = Core.ImageHelper.CutImage(TypeImage, item.DetectItemRegion);
                                            detectItemPath = System.IO.Path.GetDirectoryName(CurrentType.Details.FirstOrDefault(a => a.CameraName == item.CameraName).TemplateImagePath) + $"/{item.CameraName}_{item.DetectItemConfig.DetectItemName}_{DateTime.Now:yyyyMMddHHmmssfff}.jpg";
                                            Core.ImageHelper.SaveImage(bitmapsource, detectItemPath, GlobalConfig.Instance.CutImgJpegQuality);
                                        }
                                        AlgorithmProcedure(item, detectItemPath);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.Instance.AddLog(ex.ToString());
                                }
                                item.TypeCode = CurrentType?.TypeCode;
                                BLL.AddDetectItem(item.ConverTo());
                            }
                        }
                        else
                        {
                            if (b && detectItems.FirstOrDefault(a => a.DetectItemConfig?.DetectItemName == selectDetectItemView.SelectedDetectItem?.DetectItemName) != null)
                            {
                                HYMessageBox.Show("该相机已有该检测条目");
                            }
                            foreach (var item in e.NewItems.OfType<DetectItemModel>())
                            {
                                item.CameraName = CurrentCameraDetail.CameraName;
                                item.DetectItemConfig = null;
                                item.TypeCode = CurrentType?.TypeCode;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Instance.AddLog(ex.ToString(), true);
                   
                    }
                }
            }
        }
        protected virtual void AlgorithmProcedure(DetectItemModel item, string detectItemPath)
        {
            try
            {
                switch (item.DetectItemConfig.AlgorithmConfig.AlgorithmType)
                {
                    case AlgorithmTypeEnum.ChromaticAberration:
                        break;
                    case AlgorithmTypeEnum.QRcode:
                    case AlgorithmTypeEnum.Barcode:
                        {
                            ((AbstractVisualAlgorithm)GlobalManager.Instance.Devices.FirstOrDefault(a => a.DeviceEnum == DeviceEnum.Algorithm)).GetCheckRet(item, detectItemPath, out string ret, default(DateTime));
                            var standardValue = item.DetectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.FirstOrDefault(a => a.IsStandardValue);
                            if (standardValue != null)
                            {
                                standardValue.AlgorithmUtilsValue = ret;
                            }

                        }
                        break;
                    case AlgorithmTypeEnum.OCR:
                        {
                            ((AbstractVisualAlgorithm)GlobalManager.Instance.Devices.FirstOrDefault(a => a.DeviceEnum == DeviceEnum.Algorithm)).GetCheckRet(item, detectItemPath, out string ret, default(DateTime), true);
                            List<string> lsStr = JsonUtils.JsonDeserialize<List<string>>(ret);
                            if (!string.IsNullOrEmpty(ret) && lsStr?.Count > 0)
                            {
                                var standardValues = item.DetectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.Where(a => a.IsStandardValue).ToList();
                                for (int i = 0; i < standardValues.Count; i++)
                                {
                                    string name = lsStr.FirstOrDefault(a => a.Contains(standardValues[i].AlgorithmUtilsName));
                                    if (name != null)
                                    {
                                        int index = lsStr.IndexOf(name);
                                        int nextIndex = index < lsStr.Count - 1 ? index + 1 : index;
                                        standardValues[i].AlgorithmUtilsValue = lsStr[nextIndex];
                                    }
                                    else
                                    {
                                        LogHelper.Instance.AddLog($"不包含{standardValues[i].AlgorithmUtilsName}");
                                    }
                                }
                            }
                        }
                        break;
                    case AlgorithmTypeEnum.TemplateMatching:
                        {
                            ((AbstractVisualAlgorithm)GlobalManager.Instance.Devices.FirstOrDefault(a => a.DeviceEnum == DeviceEnum.Algorithm)).GetCheckRet(item, detectItemPath, out string ret, default(DateTime));
                            var standardValue = item.DetectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.FirstOrDefault(a => a.IsStandardValue);
                            if (standardValue != null)
                            {
                                standardValue.AlgorithmUtilsValue = ret;
                            }
                        }
                        break;
                    case AlgorithmTypeEnum.DetectDoorAlign:
                        break;
                    case AlgorithmTypeEnum.DetectDoorCrack:
                        break;
                    case AlgorithmTypeEnum.DetectTarget:
                        break;
                    case AlgorithmTypeEnum.DetectSurface:
                        break;
                    default:
                        break;
                }
            }
            catch(Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString());
            }
        }
        private string _searchText;
        /// <summary>
        /// 搜索的文本
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; RaisePropertyChanged(); }
        }
        private ICommand _searchCmd;
        /// <summary>
        /// 搜索
        /// </summary>
        public ICommand SearchCmd => _searchCmd = _searchCmd ?? new DelegateCommand(() =>
        {
            Search(SearchText);
        });
        protected void Search(string searchText)
        {
            try
            {
                List<T1> productTypeInfos = new List<T1>();
                if (string.IsNullOrEmpty(searchText))
                {
                    productTypeInfos = BLL.GetProductTypes();
                }
                else
                {
                    productTypeInfos = BLL.GetProductTypes(searchText);
                }
                SearchResult.Clear();
                SearchResult = productTypeInfos.OrderByDescending(a => a.LastAccessTime).AsParallel().AsOrdered().Select(a => (T)new T().ConvertFrom(a)).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString(), true);
            }
        }
        private ICommand _addTypeCmd;
        /// <summary>
        /// 添加型号
        /// </summary>
        public ICommand AddTypeCmd => _addTypeCmd = _addTypeCmd ?? new DelegateCommand(() =>
        {
            T productType = new T();
            if (Cameras != null)
            {
                foreach (var item in Cameras)
                {
                    ProductTypeDetailModel productTypeDetail = new ProductTypeDetailModel();
                    productTypeDetail.CameraName = item.DeviceName;
                    productTypeDetail.DetectItems = new ObservableCollection<DetectItemModel>();
                    productType.Details.Add(productTypeDetail);
                }
            }
            CurrentType = productType;
            IsReadOnly = false;


        });
        private ICommand _delTypeCmd;
        /// <summary>
        /// 删除型号                                                                              
        /// </summary>
        public ICommand DelTypeCmd => _delTypeCmd = _delTypeCmd ?? new DelegateCommand<T>(a =>
        {
            if (HYMessageBox.Show("是否要删除选中数据", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    if (BLL.DeleteProductTypeByTypeCode(a.TypeCode))
                    {
                        Search(SearchText);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.AddLog(ex.ToString(), true);
               
                }
            }
        });
        private ICommand _addDetectItemCmd;
        /// <summary>
        /// 添加检测条目
        /// </summary>
        public ICommand AddDetectItemCmd => _addDetectItemCmd = _addDetectItemCmd ?? new DelegateCommand(() =>
        {
            if (CurrentCameraDetail == null)
            {
                HYMessageBox.Show("当前类型为空");
                return;
            }
            if (string.IsNullOrEmpty(CurrentType?.TypeCode))
            {
                HYMessageBox.Show("型号编号不能为空");
                return;
            }
            var newData = new DetectItemModel();
            CurrentCameraDetail.DetectItems.Add(newData);
            if (newData.DetectItemConfig == null)
            {
                CurrentCameraDetail.DetectItems.Remove(newData);
            }
        });
        protected T _currentType;
        /// <summary>
        /// 当前类型
        /// </summary>
        public virtual T CurrentType
        {
            get { return _currentType; }
            set
            {
                if (_currentType != null)
                {
                    _currentType.PropertyChanged -= CurrentType_PropertyChanged;
                }
                if (Set(ref _currentType, value))
                {
                    IsReadOnly = true;
                    if (_currentType != null)
                    {
                        _currentType.PropertyChanged += CurrentType_PropertyChanged;
                    }
                    CanEdit = !string.IsNullOrEmpty(_currentType?.TypeCode);
                    if (!string.IsNullOrEmpty(_currentType?.TypeCode))
                    {
                        foreach (var item in _currentType.Details)
                        {
                            try
                            {
                                var usingdata = item.DetectItems;
                                item.DetectItems = new ObservableCollection<DetectItemModel>(GlobalManager.Instance.GetDetectionItems<T1>(_currentType.TypeCode).Where(a => a.CameraName == item.CameraName));
                                //item.DetectItems.ToList().ForEach(a => { a.IsUsing = usingdata.FirstOrDefault(b => b.Guid == a.Guid) != null; });
                            }
                            catch(Exception ex)
                            {
                                LogHelper.Instance.AddLog(ex.ToString());
                            }

                        }
                    }
                    SelectedCamera = _currentType == null ? null : Cameras.FirstOrDefault();
                }
            }
        }

        protected void CurrentType_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is T typeModel)
            {
                if (e.PropertyName == "TypeCode")
                {
                    CanEdit = !string.IsNullOrEmpty(typeModel.TypeCode);
                }
            }
        }

        private bool _isReadOnly = true;

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { _isReadOnly = value; RaisePropertyChanged(); }
        }
        private bool _canEdit;

        public bool CanEdit
        {
            get { return _canEdit; }
            set { _canEdit = value; RaisePropertyChanged(); }
        }

        private ICommand _saveTypeCmd;
        /// <summary>
        /// 保存
        /// </summary>
        public ICommand SaveTypeCmd => _saveTypeCmd = _saveTypeCmd ?? new DelegateCommand(() =>
        {
            try
            {
                if (CurrentType == null) return;
                if (string.IsNullOrEmpty(CurrentType.TypeCode) || string.IsNullOrEmpty(CurrentType.TypeName))
                {
                    HYMessageBox.Show("型号信息不完善");
                    return;
                }
                //只更新当前选中相机的检测条码的信息
                if (CurrentCameraDetail?.DetectItems != null)
                {
                    foreach (var item in CurrentCameraDetail.DetectItems)
                    {
                        BLL.UpdateDetectItem(item.ConverTo());
                    }
                }
                bool ret = false;
                if (BLL.GetProductTypeByTypeCode(CurrentType.TypeCode) == null)
                {
                    ret = BLL.AddProductType((T1)CurrentType.ConvertTo());
                }
                else
                {
                    if (!IsReadOnly)
                    {
                        HYMessageBox.Show("该型号已存在!");
                        return;
                    }
                    ret = BLL.UpdateProductType((T1)CurrentType.ConvertTo());
                }
                Search(SearchText);
                IsReadOnly = true;
                if (ret)
                {
                    HYMessageBox.Show("保存成功");
                }
                else
                {
                    HYMessageBox.Show("保存失败");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString(), true);
           
            }
        });
        private List<T> _searchResult = new List<T>();
        /// <summary>
        /// 显示结果
        /// </summary>
        public virtual List<T> SearchResult
        {
            get { return _searchResult; }
            private set { _searchResult = value; RaisePropertyChanged(); }
        }

        private ICommand _replaceImageCmd;
        /// <summary>
        /// 替换图片
        /// </summary>
        public ICommand ReplaceImageCmd => _replaceImageCmd = _replaceImageCmd ?? new DelegateCommand(() =>
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "图片|*.jpg;*.png;*.jpeg;*.bmp;*.tiff|所有文件(*.*)|*.*"
            };
            if (System.IO.File.Exists(CurrentCameraDetail?.TemplateImagePath))
            {
                openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName
   (CurrentCameraDetail.TemplateImagePath);
            }
            if (openFileDialog.ShowDialog() == true)
            {
                if (CurrentType != null && CurrentCameraDetail != null)
                {
                    CurrentCameraDetail.TemplateImagePath = openFileDialog.FileName;
                    TypeImage = null;
                    TypeImage = ImageHelper.GetBitmapImage(_currentCameraDetail.TemplateImagePath);
                }
            }
        });
        private DetectItemModel _selectedDetectionItem;
        /// <summary>
        /// 当前选中的条目
        /// </summary>
        public DetectItemModel SelectedDetectionItem
        {
            get { return _selectedDetectionItem; }
            set
            {
                _selectedDetectionItem = value;
                RaisePropertyChanged();
            }
        }
        private ICommand _delDetectItemCmd;
        /// <summary>
        /// 删除目录
        /// </summary>
        public ICommand DelDetectItemCmd => _delDetectItemCmd = _delDetectItemCmd ?? new DelegateCommand<DetectItemModel>(data =>
        {
            if (HYMessageBox.Show("是否要删除选中数据", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (BLL.DeleteDetectItem(data.TypeCode, data.Guid.ToString()))
                {
                    CurrentCameraDetail.DetectItems.Remove(data);
                }
                else
                {

                }
            }
        });
        private ICommand _deteialsCmd;

        public ICommand DeteialsCmd => _deteialsCmd = _deteialsCmd ?? new DelegateCommand<DetectItemModel>(data =>
        {
            if (CurrentType != null)
            {
                var typeData = CurrentType.Details.FirstOrDefault(a => a.CameraName == data.CameraName).DetectItems?.FirstOrDefault(a => a.Guid == data.Guid);
                if (typeData?.DetectItemConfig != null)
                {
                    data.DetectItemConfig.AlgorithmConfig.AlgorithmUtilsItems = typeData.DetectItemConfig.AlgorithmConfig.AlgorithmUtilsItems;
                }

                new SetDetectItemUtilsView(data.DetectItemConfig.AlgorithmConfig.AlgorithmUtilsItems).ShowDialog();
            }
        });
        private ICommand _editRegionCmd;

        public ICommand EditRegionCmd => _editRegionCmd = _editRegionCmd ?? new DelegateCommand<DetectItemModel>(data =>
        {
            if (data != null)
            {
                SelectedDetectionItem = data;
            }
        });
        #region AbstractMembers
        protected abstract IProductTypeBLL<T1> BLL { get; }
        #endregion
    }
}
