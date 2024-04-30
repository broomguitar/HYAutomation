using HYAutomation.BaseView;
using HYAutomation.BLL;
using HYAutomation.Core.Algorithm.Models;
using HYAutomation.Core.Views.Models;
using HYAutomation.Entity_Base;
using HYWindowUtils.WPF.VMBaseUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace HYAutomation.Core.Views.ViewModels
{
    public abstract class BaseProductRecordViewModel<T, T1> : NotifyPropertyObject where T : ProductInfoModel, new() where T1 : ProductInfo
    {
        public BaseProductRecordViewModel()
        {
            InitialData();
        }
        private ICommand _closeCmd;

        public ICommand CloseCmd => _closeCmd = _closeCmd ?? new DelegateCommand<Window>(w =>
        {
            if (CurrentProduct?.CameraDatas != null)
            {
                foreach (var item in CurrentProduct.CameraDatas)
                {
                    item.CameraImage = null;
                    item.Dispose();
                }
                GC.Collect();
            }
            w.Close();
        });
        #region 搜索条件
        public string Barcode { get; set; }
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now.Date;
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1).Date;
        public string Filter { get; set; }
        #endregion
        private ICommand _searchCmd;

        public ICommand SearchCmd => _searchCmd = _searchCmd ?? new DelegateCommand(() =>
        {
            GetProductInfoByDetectItem(SelectedDetectItem, Filter);
        });
        private ICommand _openFolderCmd;
        /// <summary>
        /// 查看文件
        /// </summary>
        public ICommand OpenFolderCmd => _openFolderCmd = _openFolderCmd ?? new DelegateCommand<T>(data =>
        {
            try
            {
                if (System.IO.Directory.Exists(Core.Views.GlobalManager.GetProductFolder(data)))
                {
                    System.Diagnostics.Process.Start(Core.Views.GlobalManager.GetProductFolder(data));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString(), true);
           
            }

        });
        public List<DetectItemConfigModel> DetectItemConfigs { get; set; }
        private DetectItemConfigModel _selectedDetectItem;
        public DetectItemConfigModel SelectedDetectItem
        {
            get
            {
                return _selectedDetectItem;
            }
            set
            {
                _selectedDetectItem = value;
            }
        }
        private List<T> _productLogs;
        private T _currentProduct;
        /// <summary>
        /// 日常统计信息
        /// </summary> 
        public ProductStatisticsModel ProductStatisticsInfo { get; } = new ProductStatisticsModel();
        #region VirtualMembers
        protected virtual List<T1> OriginalData { get; set; }
        public virtual List<T> ProductLogs
        {
            get { return _productLogs; }
            set
            {
                _productLogs = value;
                if (_productLogs != null)
                {
                    ProductStatisticsInfo.TotalOutputs = _productLogs.Count;
                    if (SelectedDetectItem == null)
                    {
                        ProductStatisticsInfo.OkOutputs = _productLogs.Where(a => a.IsOK == true).Count();
                    }
                    else
                    {
                        if (SelectedDetectItem.DetectItemName == "ALL")
                        {
                            ProductStatisticsInfo.OkOutputs = _productLogs.Where(a => a.IsOK == true).Count();
                        }
                        else
                        {
                            ProductStatisticsInfo.OkOutputs = _productLogs.Where(a => !a.WorkLog.Contains(SelectedDetectItem.DetectItemName)).Count();
                            //ProductStatisticsInfo.OkOutputs = _productLogs.Where(a => a.CameraDatas.Any(b => b.DetectItems.FirstOrDefault(c => c.DetectItemConfig.DetectItemName == SelectedDetectItem.DetectItemName)?.RetIsOk == true)).Count();
                        }

                    }
                }
                RaisePropertyChanged();
            }
        }
        public virtual T CurrentProduct
        {
            get { return _currentProduct; }
            set
            {
                if (_currentProduct != value)
                {
                    if (_currentProduct != null)
                    {
                        foreach (var item in _currentProduct.CameraDatas)
                        {
                            if (item != null)
                            {
                                item.CameraImage = null;
                            }
                        }
                        GC.Collect();
                    }
                    _currentProduct = value;
                    if (_currentProduct != null)
                    {
                        if (_currentProduct.CameraDatas == null)
                        {
                            _currentProduct.CameraDatas = ProductInfoModel.GetCameraResults(_currentProduct.CameraImageInfos);
                        }
                        foreach (var item in _currentProduct.CameraDatas)
                        {
                            //LogHelper.Instance.AddLog(item?.CameraImagePath);
                            //LogHelper.Instance.AddLog((System.IO.File.Exists(item?.CameraImagePath)).ToString());
                            if (System.IO.File.Exists(item?.CameraImagePath))
                            {
                                item.CameraImage = ImageHelper.GetBitmapImage(item.CameraImagePath);
                            }
                        }
                    }
                    RaisePropertyChanged();
                }
            }
        }
        protected virtual void InitialData()
        {
            try
            {
                OriginalData = new List<T1>();
                List<DetectItemConfigModel> detectItemConfigModels = new List<DetectItemConfigModel> { new DetectItemConfigModel { DetectItemName = "ALL", DetectItemDesc = "全部" } };
                if (Core.Algorithm.AlgorithmConfig.Instance.DetectItemConfigs != null)
                {
                    detectItemConfigModels.AddRange(Core.Algorithm.AlgorithmConfig.Instance.DetectItemConfigs);
                }
                DetectItemConfigs = detectItemConfigModels;
                Task.Run(() =>
                {
                    ProductLogs = OriginalData.Select(a => (T)new T().ConvertFromNoDatas(a)).ToList();
                });
            }
            catch (Exception ex)
            {

                LogHelper.Instance.AddLog(ex.ToString());
            }
        }
        protected virtual void GetProductInfoByDetectItem(DetectItemConfigModel detectItemConfigModel, string filter)
        {
            if (detectItemConfigModel != null)
            {
                if (detectItemConfigModel.DetectItemName == "ALL")
                {
                    var products = BLL.GetProducts(StartDate, EndDate, Barcode, TypeCode, TypeName, filter);
                    List<T> ls = products.Select(a => (T)new T().ConvertFromNoDatas(a)).ToList();
                    ProductLogs = ls;
                }
                else
                {
                    var products = BLL.GetProducts(StartDate, EndDate, Barcode, TypeCode, TypeName, "");
                    List<T> ls = products.Select(a => (T)new T().ConvertFromNoDatas(a)).ToList();
                    if (filter == "OK")
                    {
                        ProductLogs = ls.Where(a => !a.WorkLog.ToString().Contains(detectItemConfigModel.DetectItemName)&& !a.WorkLog.ToString().Contains("超时未处理")).ToList();
                        //ProductLogs = ls.Where(a => a.CameraDatas.Any(b => b.DetectItems.FirstOrDefault(c => c.DetectItemConfig.DetectItemName == detectItemConfigModel.DetectItemName)?.RetIsOk == true)).ToList();
                    }
                    else if (filter == "NG")
                    {
                        ProductLogs = ls.Where(a => a.WorkLog.ToString().Contains(detectItemConfigModel.DetectItemName)).ToList();
                        //ProductLogs = ls.Where(a => a.CameraDatas.Any(b => b.DetectItems.FirstOrDefault(c => c.DetectItemConfig.DetectItemName == detectItemConfigModel.DetectItemName)?.RetIsOk == false)).ToList();
                    }
                    else
                    {
                        ProductLogs = ls;
                        //ProductLogs = ls.Where(a => a.CameraDatas.Any(b => b.DetectItems.FirstOrDefault(c => c.DetectItemConfig.DetectItemName == detectItemConfigModel.DetectItemName)?.RetIsOk != null)).ToList();
                    }
                }
            }
        }
        #endregion
        #region AbstractMembers
        protected abstract IProductRecordBLL<T1> BLL { get; }
        #endregion

        
    }
}
