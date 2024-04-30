using HYAutomation.BaseView;
using HYAutomation.BaseView.Utils;
using HYAutomation.BLL;
using HYAutomation.Core.Algorithm.Models;
using HYAutomation.Core.Views.Models;
using HYAutomation.Device;
using HYAutomation.Entity_Base;
using HYAutomation.Module;
using HYCommonUtils.EnvironmentUtils;
using HYCommonUtils.SerializationUtils;
using HYWindowUtils.WPF.CommonUtils;
using HYWindowUtils.WPF.VMBaseUtil;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace HYAutomation.Core.Views.ViewModels
{
    public abstract class BaseMainViewModel<TProduct, TProductType, TProductEntity, TProductTypeEntity> : NotifyPropertyObject where TProduct : ProductInfoModel, new() where TProductType : ProductTypeModel, new() where TProductEntity : ProductInfo where TProductTypeEntity : ProductTypeInfo
    {
        #region Constructor
        public BaseMainViewModel()
        {
            InitialData();
            Application.Current.MainWindow.Closing += MainWindow_Closing;
            Application.Current.MainWindow.Closed += MainWindow_Closed;
        }
        #endregion
        #region UserControls
        public virtual UIElement TitleAndIcon { get; } = new UserControls.TitleAndIcon();
        public virtual UIElement TopWidgets { get; }
        public virtual UIElement Footer { get; } = new UserControls.FooterContent();
        public virtual UIElement ProductUserControl { get; } = new UserControls.ProductBaseInfo();

        public virtual UIElement ProductLogUserControl { get; } = new UserControls.ProducRecordList();
        public virtual UIElement CameraImageUserControl { get; } = new UserControls.CameraImageView();
        public virtual UIElement DevicesUserControl { get; } = new UserControls.DevicesStatus();
        public virtual UIElement StatisticsUserControl { get; } = new UserControls.StatisticsView();
        public virtual UIElement RunLogUserControl { get; } = new UserControls.RunLogRecorder();
        #endregion
        #region RoutedEvents
        protected virtual void MainWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
            Environment.Exit(0);
        }
        protected virtual void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (HYMessageBox.Show("是否退出应用", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ProgressHelper.ShowProgress(new Task(() =>
                {
                    System.Threading.Thread.Sleep(2000);
                    foreach (IDevice item in WorkDevices)
                    {
                        item.DisConnect();
                    }
                    UnRegisterEvent();
                }), "Closing..");
                e.Cancel = false;

            }
            else
            {
                e.Cancel = true;
            }
        }
        #endregion
        #region Properties
        /// <summary>
        /// 顶部菜单
        /// </summary>
        public IEnumerable<IModule> TopMenu { get; set; }
        private IEnumerable<IDevice> _workDevices = new List<IDevice>();
        /// <summary>
        /// 设备列表
        /// </summary>
        public IEnumerable<IDevice> WorkDevices
        {
            get
            {
                return _workDevices;
            }
            set
            {
                _workDevices = value;
                RaisePropertyChanged();
            }
        }
        public int CameraImageCount => WorkDevices?.Count(a => a.DeviceEnum == DeviceEnum.AreaCamera || a.DeviceEnum == DeviceEnum.LinearCamera || a.DeviceEnum == DeviceEnum._3DCamera) * SnapTimes ?? 0;
        /// <summary>
        /// 日志信息
        /// </summary>
        public ObservableCollection<LogInfoModel> SysLogs { get; } = new ObservableCollection<LogInfoModel>();
        private List<DetectItemStatisticsModel> _detectItemStatistics;
        /// <summary>
        /// 检测配置
        /// </summary>
        public List<DetectItemStatisticsModel> DetectItemStatistics
        {
            get
            {
                return _detectItemStatistics;
            }
            set
            {
                _detectItemStatistics = value;
                RaisePropertyChanged();
            }
        }
        private DateTime _clockTime = DateTime.Now;
        /// <summary>
        ///钟表时间
        /// </summary>
        public DateTime ClockTime
        {
            get { return _clockTime; }
            set { _clockTime = value; RaisePropertyChanged(); }
        }
        protected readonly DispatcherTimer m_dispatcherTimer = new DispatcherTimer();
        /// <summary>
        /// 统计图模型
        /// </summary>
        public StatisticsViewModel StatisticsVM { get; } = new StatisticsViewModel();
        private TProduct _currentProduct;
        /// <summary>
        /// 当前检测工件
        /// </summary>
        public TProduct CurrentProduct
        {
            get { return _currentProduct; }
            set { _currentProduct = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 生产的工件
        /// </summary>
        public ObservableCollection<TProduct> ProductLogs { get; } = new ObservableCollection<TProduct>();
        /// <summary>
        /// 工作流中临时数据
        /// </summary>
        protected virtual TProductType TempProductType { get; set; }
        /// <summary>
        /// 工作流中临时数据
        /// </summary>
        protected virtual TProduct TempProductInfo { get; set; }
        protected virtual int SnapTimes { get; } = 1;
        protected virtual int TypeCodeLength { get; } = 9;
        protected ConcurrentQueue<AbstractCamera> p_Cameras = new ConcurrentQueue<AbstractCamera>();
        #endregion
        #region Commands
        private ICommand _minCmd;
        /// <summary>
        /// 最小化
        /// </summary>
        public ICommand MinCmd => _minCmd = _minCmd ?? new DelegateCommand<Window>(w =>
        {
            w.WindowState = WindowState.Minimized;
        });
        private ICommand _normalCmd;

        public ICommand NormalCmd => _normalCmd = _normalCmd ?? new DelegateCommand<Window>(w =>
        {
            if (w.WindowState == WindowState.Normal)
            {
                SystemCommands.MaximizeWindow(w);
            }
            else
            {
                SystemCommands.RestoreWindow(w);
            }
        });

        private ICommand _exitCmd;
        /// <summary>
        /// 退出
        /// </summary>
        public ICommand ExitCmd => _exitCmd = _exitCmd ?? new DelegateCommand<Window>(w =>
        {
            w.Close();
        });
        private ICommand _hyperlinkCmd;
        /// <summary>
        /// 超链
        /// </summary>
        public ICommand HyperlinkCmd => _hyperlinkCmd = _hyperlinkCmd ?? new DelegateCommand<Uri>(
            url =>
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url.AbsoluteUri));
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.AddLog(ex.ToString(), true);
                }
            });
        private ICommand _resetCmd;
        /// <summary>
        /// 流程复位
        /// </summary>
        public ICommand ResetCmd => _resetCmd = _resetCmd ?? new DelegateCommand(Reset);
        private ICommand _detailsCmd;
        /// <summary>
        /// 查看详情
        /// </summary>
        public ICommand DeteialsCmd => _detailsCmd = _detailsCmd ?? new DelegateCommand<ProductInfoModel>(data =>
        {
            try
            {
                ResultDetailView resultDetailView = new ResultDetailView(data);
                resultDetailView.ShowDialog();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString(), true);

            }

        });
        private ICommand _openFolderCmd;
        /// <summary>
        /// 查看文件
        /// </summary>
        public ICommand OpenFolderCmd => _openFolderCmd = _openFolderCmd ?? new DelegateCommand<ProductInfoModel>(data =>
        {
            try
            {
                if (System.IO.Directory.Exists(GlobalManager.GetProductFolder(data)))
                {
                    System.Diagnostics.Process.Start(GlobalManager.GetProductFolder(data));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString(), true);

            }

        });
        #endregion
        #region LocalMethods
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            ClockTime = DateTime.Now;
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        protected virtual void InitialData()
        {
            try
            {
                //时钟
                m_dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
                m_dispatcherTimer.Tick += DispatcherTimer_Tick;
                m_dispatcherTimer.Start();
                LogHelper.Instance.NewLogEvent += AddLog;
                GlobalManager.Instance.ProductTypeBLL = TypeBLL;
                ///顶部菜单
                TopMenu = ReflectionHelper.CreateAllInstancesOf<IModule>(this.GetType().Assembly).Where(a => a.IsAvailable).OrderBy(b => b.ModuleIndex);
                //加载设备
                InitialDevice();
                ///加载统计数据库数据
                Algorithm.AlgorithmConfig.Instance.RefreshDetecteItemsEvent += InitProductStatistics;
                InitProductStatistics(null, null);
                SystemUtil.SetAppAutoStart(GlobalConfig.Instance.IsAutoStart);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString(), true);
            }
        }
        /// <summary>
        /// 初始化设备
        /// </summary>
        protected virtual void InitialDevice()
        {
            if (DeviceAssembly != null)
            {
                GlobalManager.Instance.Devices = WorkDevices = new List<IDevice>(ReflectionHelper.CreateAllInstancesOf<IDevice>(DeviceAssembly).Where(a => a.IsAvailable).OrderBy(a => a.DeviceIndex));
            }
            SetImgRowsAndColumns();
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ProgressHelper.ShowProgress(new Task(() =>
                    {
                        Parallel.ForEach(WorkDevices, new ParallelOptions { MaxDegreeOfParallelism = 3 }, item =>
                        {
                            item.Connect();
                            item.PushResultEvent += Item_PushResultEvent;
                        });
                    }), "Initing..");
                });
            });
        }
        protected virtual void UnRegisterEvent()
        {
            Core.Algorithm.AlgorithmConfig.Instance.RefreshDetecteItemsEvent -= InitProductStatistics;
            foreach (var item in WorkDevices)
            {
                item.PushResultEvent -= Item_PushResultEvent;
            }
            LogHelper.Instance.NewLogEvent -= AddLog;
        }
        protected virtual void SetImgRowsAndColumns()
        {
            if (CameraImageCount < 2)
            {
                GlobalManager.Instance.ImgColumns = GlobalManager.Instance.ImgRows = 1;
            }
            else
            {
                GlobalManager.Instance.ImgRows = 2;
                GlobalManager.Instance.ImgColumns = CameraImageCount % 2 == 0 ? CameraImageCount / 2 : CameraImageCount / 2 + 1;
            }
        }
        //添加日志
        private void AddLog(object o, LogInfoModel loginfoModel)
        {
            Action _appendLogAction = new Action(() =>
            {
                while (SysLogs.Count > 1000)
                {
                    SysLogs.RemoveAt(SysLogs.Count - 1);
                }
                SysLogs.Insert(0, loginfoModel);
            });
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke((ThreadStart)delegate
                {
                    _appendLogAction?.Invoke();
                });
            }
            else
            {
                _appendLogAction?.Invoke();
            }
        }
        #endregion
        #region VirtualMethods
        protected virtual void Reset()
        {
            if (HYMessageBox.Show("是否要复位，如果有产品在检测,检测结果NG", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    if (CurrentProduct != null && !CurrentProduct.IsOK.HasValue)
                    {
                        if (CurrentProduct.CameraDatas != null)
                        {
                            foreach (var item in CurrentProduct.CameraDatas)
                            {
                                if (item != null && !item.CameraRetIsOK.HasValue)
                                {
                                    item.CameraImage = null;
                                    item.CameraRetIsOK = false;
                                }
                            }
                        }
                        CurrentProduct.IsOK = false;
                        CurrentProduct.TempLog.Append("人工复位");
                        SetProductLog(CurrentProduct);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            CurrentProduct?.CameraDatas?.Clear();
                            TempProductInfo?.CameraDatas?.Clear();
                        });
                        TempProductInfo = null;
                        CurrentProduct = null;
                        LogHelper.Instance.AddLog($"人工复位");
                    }
                    else if (TempProductInfo != null)
                    {
                        TempProductInfo.IsOK = false;
                        TempProductInfo.TempLog.Append("人工复位");
                        SetProductLog(TempProductInfo);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            TempProductInfo?.CameraDatas?.Clear();
                        });
                        TempProductInfo = null;
                        LogHelper.Instance.AddLog($"人工复位");
                    }
                    while (p_Cameras.TryDequeue(out AbstractCamera camera))
                    {
                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {

                    LogHelper.Instance.AddLog(ex.ToString(), true);
                }
            }
        }
        /// <summary>
        /// 初始化统计
        /// </summary>
        protected virtual void InitProductStatistics(object o, EventArgs e)
        {

            List<DetectItemStatisticsModel> detectItemStatistics = new List<DetectItemStatisticsModel>();
            Core.Algorithm.AlgorithmConfig.Instance.DetectItemConfigs?.ForEach(a => detectItemStatistics.Add(new DetectItemStatisticsModel { DetectItemConfig = a }));
            detectItemStatistics.Add(new DetectItemStatisticsModel { DetectItemConfig = new DetectItemConfigModel { DetectItemName = "ALL", DetectItemDesc = "总生产" } });
            detectItemStatistics.Add(new DetectItemStatisticsModel { DetectItemConfig = new DetectItemConfigModel { DetectItemName = "Unmaintained", DetectItemDesc = "未维护" } });
            DetectItemStatistics = detectItemStatistics;
            Task.Run(() =>
            {
                foreach (var item in DetectItemStatistics)
                {
                    GetProductStatisticsInfoByDetectItem(item);
                }
            });
        }
        /// <summary>
        /// 统计数据
        /// </summary>
        /// <param name="detectItemConfigModel"></param>
        protected virtual void GetProductStatisticsInfoByDetectItem(DetectItemStatisticsModel detectItemConfigModel)
        {
            try
            {
                List<TProductEntity> data = BLL.GetToDayProducts();
                List<ProductInfoModel> todayProducts = data.Select(a => new ProductInfoModel().ConvertFrom(a)).Where(a => !string.IsNullOrEmpty(a.TypeName)).ToList();
                if (detectItemConfigModel != null)
                {
                    if (detectItemConfigModel.DetectItemConfig.DetectItemName == "ALL")
                    {
                        detectItemConfigModel.ProductStatistics.TotalOutputs = data.Count;
                        detectItemConfigModel.ProductStatistics.OkOutputs = data.Where(a => a.IsOK).Count();
                    }
                    else if (detectItemConfigModel.DetectItemConfig.DetectItemName == "Unmaintained")
                    {
                        detectItemConfigModel.ProductStatistics.TotalOutputs = data.Where(a => a.IsOK && a.Note.Contains("未维护")).Count();
                        detectItemConfigModel.ProductStatistics.OkOutputs = detectItemConfigModel.ProductStatistics.TotalOutputs;
                    }
                    else
                    {
                        detectItemConfigModel.ProductStatistics.TotalOutputs = todayProducts.Where(a => a.CameraDatas.Any(b => b.DetectItems.FirstOrDefault(c => c.DetectItemConfig?.DetectItemName == detectItemConfigModel.DetectItemConfig.DetectItemName)?.RetIsOk != null))?.Count() ?? 0;
                        detectItemConfigModel.ProductStatistics.OkOutputs = todayProducts.Where(a => a.CameraDatas.Any(b => b.DetectItems.FirstOrDefault(c => c.DetectItemConfig?.DetectItemName == detectItemConfigModel.DetectItemConfig.DetectItemName)?.RetIsOk == true))?.Count() ?? 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString(), true);
            }
        }
        protected virtual void Procedure_Camera(IDevice sender, object e)
        {
            try
            {
                if (!(sender is AbstractCamera camera) || !(e is Bitmap bitmap))
                {
                    return;
                }
                LogHelper.Instance.AddLog($"线程ID{Thread.CurrentThread.ManagedThreadId}:收到{CurrentProduct?.Barcode}---{camera.DeviceDesc}数据");
                if (CurrentProduct == null || CurrentProduct.IsOK.HasValue)
                {
                    string dir = Path.Combine(GlobalConfig.Instance.CacheFolder, camera.DeviceName);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    string tempFilePath = dir + $"/{DateTime.Now:yyyyMMddHHmmssfff}.jpg";
                    if (bitmap != null)
                    {
                        ImageHelper.SaveImage(bitmap, tempFilePath, GlobalConfig.Instance.JpegQuality);
                        bitmap.Dispose();
                        bitmap = null;
                    }
                    return;
                }
                if (p_Cameras.Count(b => b.DeviceName == camera.DeviceName) >= SnapTimes)
                {
                    LogHelper.Instance.AddLog($"已存在{camera.DeviceDesc}相机数据");
                    string dir = Path.Combine(GlobalConfig.Instance.CacheFolder, camera.DeviceName);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    string tempFilePath = dir + $"/{DateTime.Now:yyyyMMddHHmmssfff}.jpg";
                    if (bitmap != null)
                    {
                        ImageHelper.SaveImage(bitmap, tempFilePath, GlobalConfig.Instance.JpegQuality);
                        bitmap.Dispose();
                        bitmap = null;
                    }
                    return;
                }
                p_Cameras.Enqueue(camera);
                if (p_Cameras.Count == CameraImageCount)
                {
                    var plc = WorkDevices.FirstOrDefault(a => a.DeviceEnum == DeviceEnum.PLC);
                    if (plc != null)
                    {
                        ((AbstractPLC)plc).SetLinearFinish();
                    }
                }
                string productdir = GlobalManager.GetProductFolder(CurrentProduct);
                if (!Directory.Exists(productdir))
                {
                    Directory.CreateDirectory(productdir);
                }
                string filePath = GlobalManager.GetProductFolder(CurrentProduct) + $"/{camera.DeviceName}_{CurrentProduct.CreateTime:yyyyMMddHHmmssfff}.jpg";
                if (bitmap != null)
                {
                    ImageHelper.SaveImage(bitmap, filePath, GlobalConfig.Instance.JpegQuality);
                    bitmap.Dispose();
                    bitmap = null;
                }
                CameraResultModel cameraData = new CameraResultModel();
                cameraData.CameraName = camera.DeviceName;
                cameraData.CameraDesc = camera.DeviceDesc;
                cameraData.CameraImagePath = filePath;
                BitmapSource bitmapImage = ImageHelper.GetBitmapImage(filePath);
                cameraData.CameraImage = bitmapImage;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CameraResultModel oldData = CurrentProduct.CameraDatas.FirstOrDefault(a => a.CameraName == camera.DeviceName);
                    if (oldData != null)
                    {
                        oldData.CameraImage = null;
                        CurrentProduct.CameraDatas.Remove(oldData);
                        oldData.Dispose();
                    }
                    List<AbstractCamera> _cameraOrders = p_Cameras.OrderBy(a => a.DeviceIndex).ToList();
                    int index = _cameraOrders.IndexOf(camera);
                    if (index < CurrentProduct.CameraDatas.Count)
                    {
                        CurrentProduct.CameraDatas.Insert(index, cameraData);
                    }
                    else
                    {
                        CurrentProduct.CameraDatas.Add(cameraData);
                    }
                });
                if (bitmapImage != null)
                {
                    if (TempProductType?.Details != null)
                    {
                        cameraData.DetectItems = TempProductType.Details.FirstOrDefault(c => c.CameraName == camera.DeviceName)?.DetectItems.Where(a => a.CameraName == cameraData.CameraName && a.IsUsing);
                        bool checkRet = true;
                        if (cameraData.DetectItems?.Count() > 0)
                        {
                            LogHelper.Instance.AddLog($"{cameraData.CameraName}开始检测");
                            ParallelLoopResult parallelLoopResult = Parallel.ForEach(cameraData.DetectItems, item =>
                            {
                                DetectItemModel data = item;
                                if (data != null)
                                {
                                    string detectItemPath = string.Empty;
                                    if (data.DetectItemRegion.IsEmpty)
                                    {
                                        detectItemPath = filePath;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            //裁剪图片
                                            BitmapSource bitmapsource = Core.ImageHelper.CutImage(bitmapImage, data.DetectItemRegion);
                                            detectItemPath = GlobalManager.GetProductFolder(CurrentProduct) + $"/{cameraData.CameraName}_{data.DetectItemConfig.DetectItemName}_{CurrentProduct.CreateTime:yyyyMMddHHmmssfff}.jpg";
                                            Core.ImageHelper.SaveImage(bitmapsource, detectItemPath, GlobalConfig.Instance.CutImgJpegQuality);
                                        }
                                        catch (Exception ex)
                                        {
                                            detectItemPath = filePath;
                                            LogHelper.Instance.AddLog(item.CameraName + item.DetectItemConfig.DetectItemDesc + "裁图异常" + data.DetectItemRegion.ToString());
                                        }

                                    }
                                    item.RetIsOk = ((AbstractVisualAlgorithm)WorkDevices.FirstOrDefault(a => a.DeviceEnum == DeviceEnum.Algorithm)).GetCheckRet(data, detectItemPath, out string ret, CurrentProduct.CreateTime);
                                    item.Result = ret;
                                    checkRet &= item.RetIsOk.Value;
                                }
                            });
                            while (!parallelLoopResult.IsCompleted)
                            {
                                Thread.Sleep(10);
                            }
                            string retFile = System.IO.Path.Combine(Path.GetDirectoryName(filePath) + $"\\{Path.GetFileNameWithoutExtension(filePath)}_Ret.jpg");
                            if (File.Exists(retFile))
                            {
                                cameraData.CameraImage = null;
                                cameraData.CameraImagePath = retFile;
                                cameraData.CameraImage = Core.ImageHelper.GetBitmapImage(retFile);
                            }
                        }
                        cameraData.CameraRetIsOK = checkRet;
                    }
                    else
                    {
                        cameraData.CameraRetIsOK = true;
                    }
                }
                else
                {
                    cameraData.CameraRetIsOK = false;
                }

                if (CurrentProduct.CameraDatas.Count == CameraImageCount && CurrentProduct.CameraDatas.All(a => a?.CameraRetIsOK != null))
                {
                    while (p_Cameras.TryDequeue(out camera))
                    {
                        Thread.Sleep(10);
                    }
                    CurrentProduct.IsOK = true;
                    CurrentProduct.CameraDatas.ToList().ForEach(a => CurrentProduct.IsOK &= a.CameraRetIsOK);
                    if (CurrentProduct.IsOK == false)
                    {
                        foreach (var item in CurrentProduct.CameraDatas)
                        {
                            if (!item.CameraRetIsOK.Value)
                            {
                                var data = item.DetectItems.Where(a => a.RetIsOk == false);
                                CurrentProduct.TempLog.Append($"{item.CameraName}-{JsonUtils.JsonSerialize(data.Select(a => a.DetectItemConfig.DetectItemName).ToList())}_检测有缺陷");
                            }
                        }
                    }
                    LogHelper.Instance.AddLog($"{CurrentProduct.Barcode}检测结束,结果{CurrentProduct.IsOK}");
                    SetProductLog(CurrentProduct);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString(), true);

                CurrentProduct.IsOK = false;
                CurrentProduct.TempLog.Append(ex.ToString());
                LogHelper.Instance.AddLog($"{CurrentProduct.Barcode}检测异常,结果{CurrentProduct.IsOK}");
                SetProductLog(CurrentProduct);
            }
            finally
            {
                GC.Collect();
            }
        }
        protected virtual void SetProductLog(TProduct productInfoModel)
        {
            try
            {
                if (productInfoModel == null) return;
                productInfoModel.WorkLog = productInfoModel.TempLog.Length > 0 ? productInfoModel.TempLog.ToString() : "产品合格";
                productInfoModel.TempLog.Clear();
                string productJson = JsonUtils.JsonSerialize(productInfoModel);
                if (Directory.Exists(GlobalManager.GetProductFolder(productInfoModel)))
                {
                    FileUtils.FileAppendText.AppendText(GlobalManager.GetProductFolder(productInfoModel) + $"/DetectRet_{productInfoModel.CreateTime.ToString("yyyyMMddHHmmssfff")}.data", $"{productJson}{Environment.NewLine}");
                }
                foreach (var item in DetectItemStatistics)
                {
                    if (item.DetectItemConfig.DetectItemName == "ALL")
                    {
                        item.ProductStatistics.TotalOutputs += 1;
                        AbstractPLC plc = WorkDevices.FirstOrDefault(a => a.DeviceEnum == DeviceEnum.PLC) as AbstractPLC;
                        if (plc != null)
                        {
                            if (!(productInfoModel.IsOK.Value ? plc.SetOK() : plc.SetNG()))
                            {
                                LogHelper.Instance.AddLog($"{productInfoModel.Barcode}----设置PLC结果有误", true);
                            }
                        }
                        try
                        {
                            if (!BLL.AddProductData((TProductEntity)productInfoModel.ConvertTo()))
                            {
                                LogHelper.Instance.AddLog(productInfoModel.Barcode + "----" + "插入数据库失败!", true);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Instance.AddLog(ex.ToString(), true);
                        }
                        Application.Current.Dispatcher.Invoke((ThreadStart)delegate
                        {
                            if (productInfoModel.IsOK == true)
                            {
                                item.ProductStatistics.OkOutputs += 1;
                            }
                            while (ProductLogs.Count > 14)
                            {
                                ProductLogs.RemoveAt(ProductLogs.Count - 1);
                            }
                            var data = JsonUtils.JsonDeserialize<TProduct>(productJson);
                            ProductLogs.Insert(0, data);
                        });
                    }
                    else if (item.DetectItemConfig.DetectItemName == "Unmaintained")
                    {
                        if (productInfoModel.IsOK==true && productInfoModel.WorkLog.Contains("未维护"))
                        {
                            item.ProductStatistics.TotalOutputs += 1;
                            item.ProductStatistics.OkOutputs += 1;
                        }
                    }
                    else
                    {
                        if (productInfoModel.CameraDatas != null)
                        {
                            bool flag = productInfoModel.CameraDatas.Any(b => b.DetectItems.FirstOrDefault(c => c.DetectItemConfig.DetectItemName == item.DetectItemConfig.DetectItemName)?.RetIsOk != null);
                            if (flag)
                            {
                                item.ProductStatistics.TotalOutputs += 1;
                                if (productInfoModel.CameraDatas.Any(b => b.DetectItems.FirstOrDefault(c => c.DetectItemConfig.DetectItemName == item.DetectItemConfig.DetectItemName)?.RetIsOk == true))
                                {
                                    item.ProductStatistics.OkOutputs += 1;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString(), true);
            }
        }
        protected virtual void Procedure_Scanner(IDevice sender, object e)
        {
            try
            {
                if (WorkDevices.Any(a => !a.IsOnline))
                {
                    LogHelper.Instance.AddLog("有设备未启动");
                    return;
                }
                else
                {
                    TempProductType = null;
                    string barcode = e.ToString();
                    LogHelper.Instance.AddLog($"扫码枪收到条码{barcode}");
                    if (barcode.Length > TypeCodeLength)
                    {
                        if (TempProductInfo != null)
                        {
                            LogHelper.Instance.AddLog($"已有产品条码在处理，新条码将其覆盖");
                        }
                        TempProductInfo = new TProduct
                        {
                            Barcode = barcode
                        };
                        var typeCode = barcode.Substring(0, TypeCodeLength);
                        TempProductInfo.TypeCode = typeCode;
                        //判断型号信息是否已维护
                        TempProductType = (TProductType)new TProductType().ConvertFrom(BLL.GetProductTypeByTypeCode(typeCode));
                        TempProductInfo.TypeName = TempProductType?.TypeName;
                        if (TempProductType == null)
                        {
                            TempProductInfo.IsOK = false;
                            TempProductInfo.TempLog.Append("不存在型号信息");
                            return;
                        }
                        TempProductInfo.TypeName = TempProductType.TypeName;
                    }
                    else
                    {
                        LogHelper.Instance.AddLog($"扫码枪无效数据{barcode}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString(), true);
            }

        }
        protected virtual void Procedure_PLC(IDevice sender, object e)
        {
            try
            {
                if (e is KeyValuePair<PlcSignalEnum, object> kv)
                {
                    switch (kv.Key)
                    {
                        case PlcSignalEnum.Arrival:
                            {
                                LogHelper.Instance.AddLog("【PLC】收到到位信号,开始处理条码:" + TempProductInfo?.Barcode);
                                ///没扫到码
                                if (TempProductInfo == null)
                                {
                                    LogHelper.Instance.AddLog($"没扫到条码，检测结束,结果NG");
                                    //发送给PLC
                                    var PLC = WorkDevices.FirstOrDefault(a => a.DeviceEnum == DeviceEnum.PLC);
                                    if (PLC is AbstractPLC plc && !plc.SetNG())
                                    {
                                        LogHelper.Instance.AddLog($"{TempProductInfo?.Barcode}----设置PLC结果有误", true);
                                    }
                                    LogHelper.Instance.AddLog($"没有扫到产品条码");
                                }
                                ///NG处理
                                else if (TempProductInfo != null && TempProductInfo.IsOK == false && TempProductType == null)
                                {
                                    LogHelper.Instance.AddLog(TempProductInfo.Barcode + "----" + "不存在型号信息,按NG处理", true);
                                    TempProductInfo.TempLog.Append("型号未维护");
                                    SetProductLog(TempProductInfo);
                                    TempProductInfo = null;
                                }
                                else
                                {
                                    if (CurrentProduct != null && !CurrentProduct.IsOK.HasValue)
                                    {
                                        CurrentProduct.IsOK = false;
                                        CurrentProduct.TempLog.Append("超时未处理，后产品进入");
                                        SetProductLog(CurrentProduct);
                                    }
                                    if (CurrentProduct?.CameraDatas != null)
                                    {
                                        foreach (var item in CurrentProduct.CameraDatas)
                                        {
                                            if (item != null)
                                            {
                                                item.CameraImage = null;
                                            }
                                        }
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            CurrentProduct.CameraDatas.Clear();
                                        });
                                    }
                                    CurrentProduct = null;
                                    GC.Collect();
                                    //设置曝光和增益
                                    try
                                    {
                                        WorkDevices?.Where(a => a.DeviceEnum == DeviceEnum.AreaCamera || a.DeviceEnum == DeviceEnum.LinearCamera)?.OfType<AbstractCamera>()?.ToList().ForEach(a =>
                                        {
                                            double? exp = TempProductType?.Details?.FirstOrDefault(b => b.CameraName == a.DeviceName)?.CameraExposure;
                                            double? gain = TempProductType?.Details?.FirstOrDefault(b => b.CameraName == a.DeviceName)?.CameraGain;
                                            if (exp.HasValue && exp.Value > 0)
                                            {
                                                a.SetExposureTime(exp.Value);
                                            }
                                            if (gain.HasValue && gain.Value > 0)
                                            {
                                                a.SetGain(gain.Value);
                                            }
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        LogHelper.Instance.AddLog(ex.ToString());
                                    }
                                    CurrentProduct = new TProduct { Barcode = TempProductInfo.Barcode, TypeCode = TempProductInfo.TypeCode, TypeName = TempProductInfo.TypeName, CameraDatas = new ObservableCollection<CameraResultModel>() };
                                    TempProductInfo = null;
                                    if (!((AbstractPLC)WorkDevices.FirstOrDefault(a => a.DeviceEnum == DeviceEnum.PLC)).SetLinearStart())
                                    {
                                        LogHelper.Instance.AddLog($"{TempProductInfo?.Barcode}----设置PLC结果有误", true);
                                    }
                                }

                            }
                            break;
                        case PlcSignalEnum.ConveyAutoStop:
                            {

                            }
                            break;
                        case PlcSignalEnum.Alarm:
                            {

                            }
                            break;
                        default:
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString(), true);
            }

        }
        #endregion
        #region AbstractMembers
        protected abstract System.Reflection.Assembly DeviceAssembly { get; }
        protected abstract void Item_PushResultEvent(object sender, object e);
        protected abstract IMainBLL<TProductEntity, TProductTypeEntity> BLL { get; }
        protected abstract IProductTypeBLL TypeBLL { get; }
        #endregion
    }
}
