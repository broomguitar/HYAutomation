using HYAutomation.BaseView;
using HYAutomation.BaseView.Utils;
using HYAutomation.BLL;
using HYAutomation.BLL_Base;
using HYAutomation.Core;
using HYAutomation.Core.Algorithm;
using HYAutomation.Core.Algorithm.Models;
using HYAutomation.Core.Views;
using HYAutomation.Core.Views.Models;
using HYAutomation.Core.Views.ViewModels;
using HYAutomation.Core.Web;
using HYAutomation.Device;
using HYAutomation.Devices.HiEdgeMind;
using HYAutomation.Entity_Base;
using HYCommonUtils.EnvironmentUtils;
using HYCommonUtils.SerializationUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace HYAutomation.Projects.HiEdgeMind
{
    public class MainViewModel : BaseMainViewModel<ProductInfoModel, ProductTypeModel, ProductInfo, ProductTypeInfo>
    {
        private ConfigManagerUtil configManager = ConfigManagerUtil.GetInstance(System.Reflection.Assembly.GetExecutingAssembly());
        private HardwareRoot hardwareRoot = new HardwareRoot();
        private WorkFlowRoot workFlowRoot = new WorkFlowRoot();
        private List<AlgorithmConfigItem> algorithmConfigList = new List<AlgorithmConfigItem>();
        private List<CheckItemData> detectItems = new List<CheckItemData>();
        // "192.168.110.51"; "111.198.15.60";
        private string ip = "platform.hiyantech.com";
        private string uuid = "test";// UUIDHelper.GetUUID();
        private void LoadConfig()
        {
            hardwareRoot = ServerConnector.DownloadHardwares($"http://{ip}:8848/device/download-hardware-config-file/", uuid);
            workFlowRoot = ServerConnector.DownloadWorkFlow($"http://{ip}:8848/device/download-flow-config-file/", uuid);
            algorithmConfigList = ServerConnector.DownloadAlgorithms($"http://{ip}:8848/device/download-device-algorithm-config-file", uuid);
            detectItems = ServerConnector.DownloadDetectItems($"http://{ip}:8848/device/download-check-item-config-file2", uuid);
            Task.Factory.StartNew(() => {
                while (true)
                {
                    ServerConnector.Online($"http://{ip}:8848/device-status/update-device-status/{uuid}");
                    Thread.Sleep(3000);
                }
            },TaskCreationOptions.LongRunning);
        }
        protected override void InitialDevice()
        {
            LoadConfig();
            List<IDevice> devices = new List<IDevice>();
            foreach (var item in hardwareRoot?.hardwareList)
            {
                IDevice device = null;
                if (item.type == HardwareTypeEnum.PLC)
                {
                    device = new PLC();
                    if (device is PLC plc)
                    {
                        plc.PLCFactory = item.brand;
                        plc.IP_PLC = item.ip;
                        plc.Port_PLC = item.port;
                        plc.Addr_Arrive = item.Addr_Arrive;
                        plc.Addr_OK = item.Addr_OK;
                        plc.Addr_NG = item.Addr_NG;
                        plc.Addr_CommOK = item.Addr_CommOK;
                        plc.Addr_GrabFinish = item.Addr_GrabFinish;
                        plc.Addr_LinearStart = item.Addr_LinearStart;
                    }
                }
                else if (item.type == HardwareTypeEnum.Camera)
                {
                    if (item.cameraType == 3)
                    {
                        device = new _3DCamera();
                        if (device is Abstract3DCamera camera)
                        {
                            camera.IP = item.ip;
                            camera.DeviceID = int.Parse(item.deviceId);
                        }
                    }
                    else
                    {
                        device = new Camera();
                        if (device is Camera camera)
                        {
                            camera.CameraFactory = item.brand;
                            camera.IslinearCamera = item.cameraType == 2;
                            camera.CameraConnType = item.connetType==ConnectEnum.Tcp? HY.Devices.Camera.CameraConnectTypes.GigE:HY.Devices.Camera.CameraConnectTypes.CameraLink;
                            if(item.trigger_type==1)
                            {
                                camera.TriggerMode = HY.Devices.Camera.TriggerMode.OFF;
                            }
                            else if(item.trigger_type==2)
                            {
                                camera.TriggerMode = HY.Devices.Camera.TriggerMode.ON;
                                camera.TriggerSource = HY.Devices.Camera.TriggerSources.Soft;
                            }
                            else if (item.trigger_type == 3)
                            {
                                camera.TriggerMode = HY.Devices.Camera.TriggerMode.ON;
                                camera.TriggerSource = HY.Devices.Camera.TriggerSources.Line0;
                            }
                            camera.CameraSN = item.sn;
                        }
                    }
                }
                else if (item.type == HardwareTypeEnum.Scanner)
                {
                    device = new Scanner();
                    if (device is Scanner scanner)
                    {
                        scanner.IsSerailPort = item.connetType == ConnectEnum.SerialPort;
                        scanner.IP_Scanner = item.ip;
                        scanner.Port_Scanner = item.port;
                    }
                }
                else if (item.type == HardwareTypeEnum.IOBoard)
                {
                }
                else if (item.type == HardwareTypeEnum.Robot)
                {
                    device = new Robot();
                    if (device is Robot robot)
                    {
                        robot.IP_Server = IPAddress.TryParse(item.ip, out IPAddress ip) ? ip : IPAddress.Any;
                        robot.Port_Server = item.port;
                    }
                }
                device.DeviceId = item.deviceId;
                device.DeviceIndex = item.sort_index;
                device.DeviceName = item.name;
                device.DeviceDesc = item.memo;
                devices.Add(device);
            }
            if (algorithmConfigList?.Count > 0)
            {
                VisualAlgorithm algorithm = new VisualAlgorithm();
                algorithm.AlgorithmConfigList = algorithmConfigList;
                algorithm.DeviceIndex = hardwareRoot.hardwareList.Count;
                devices.Add(algorithm);
            }
            GlobalManager.Instance.Devices = WorkDevices = devices.Where(a => a.IsAvailable).OrderBy(a => a.DeviceIndex);
            SetImgRowsAndColumns();
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ProgressHelper.ShowProgress(new Task(() =>
                    {
                        if (algorithmConfigList != null)
                        {
                            foreach (var item in algorithmConfigList)
                            {
                                if (item.algorithmVersionId.ToString() == configManager.GetValue(item.algorithmVersionId.ToString()))
                                {
                                    break;
                                }
                                if (item.initParams != null&&item.initParams.Count>0)
                                {
                                    ServerConnector.DownLoadModelFile(item.initParams.Where(a => a.type == "file" && CheckURLValid(a.url)).Select(a => a.url).ToList());
                                }
                                configManager.SaveValue(new Dictionary<string, string> { {item.algorithmVersionId.ToString(),item.version.ToString()} });
                            }
                        }
                        Parallel.ForEach(WorkDevices, new ParallelOptions { MaxDegreeOfParallelism = 3 }, item =>
                        {
                            item.Connect();
                            item.PushResultEvent += Item_PushResultEvent;
                        });
                    }), "Initing..");
                });
            }).ContinueWith(t => { Task.Run(Work); });
        }
        private bool CheckURLValid(string source)
        {
            Uri uriResult;
            return Uri.TryCreate(source, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;
        }
        protected override void InitProductStatistics(object o, EventArgs e)
        {

            List<DetectItemStatisticsModel> detectItemStatistics = new List<DetectItemStatisticsModel>();
            detectItems.ForEach(a => detectItemStatistics.Add(new DetectItemStatisticsModel { DetectItemConfig = new DetectItemConfigModel { DetectItemName = a.checkItemName, DetectItemDesc = a.checkItemName } }));
            //Core.Algorithm.AlgorithmConfig.Instance.DetectItemConfigs?.ForEach(a => detectItemStatistics.Add(new DetectItemStatisticsModel { DetectItemConfig = a }));
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
        protected override void GetProductStatisticsInfoByDetectItem(DetectItemStatisticsModel detectItemConfigModel)
        {
            try
            {
                List<ProductInfo> data = BLL.GetToDayProducts();
                List<ProductInfoModel> todayProducts = data.Select(a => GetProduct(a)).Where(a => !string.IsNullOrEmpty(a.TypeName)).ToList();
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
        private ProductInfoModel GetProduct(ProductInfo productInfo)
        {
            if (productInfo == null)
            {
                return null;
            }
            ProductInfoModel data = new ProductInfoModel
            {
                CreateTime = productInfo.CreateTime,
                Barcode = productInfo.Barcode,
                TypeCode = productInfo.TypeCode,
                TypeName = productInfo.TypeName,
                IsOK = productInfo.IsOK,
                CameraDatas = GetCameraResults(productInfo.CameraImageInfos),
                CameraImageInfos = productInfo.CameraImageInfos,
                WorkLog = productInfo.Note
            };
            return data;
        }
        private  IList<CameraResultModel> GetCameraResults(string productResultDetailsJson)
        {
            List<CameraResultModel> cameraResultModels = new List<CameraResultModel>();
            try
            {
                if (!string.IsNullOrEmpty(productResultDetailsJson))
                {

                    List<ProductResultDetails> productResultDetails;
                    productResultDetails = HYCommonUtils.SerializationUtils.JsonUtils.JsonDeserialize<List<ProductResultDetails>>(productResultDetailsJson);
                    foreach (var productResultDetail in productResultDetails)
                    {
                        CameraResultModel cameraResultModel = new CameraResultModel();
                        if (productResultDetails != null)
                        {
                            cameraResultModel.CameraName = productResultDetail.CameraName;
                            cameraResultModel.CameraDesc = productResultDetail.CameraDesc;
                            cameraResultModel.CameraImagePath = productResultDetail.ImagePath;
                            cameraResultModel.CameraRetIsOK = productResultDetail.Result;
                            foreach (var item in productResultDetail.DetailResults)
                            {
                                var data = new DetectItemModel { Result = item.RetStr, RetIsOk = item.RetIsOk };
                                data.DetectItemConfig = DetectItemStatistics?.FirstOrDefault(a => a.DetectItemConfig?.DetectItemName == item.DetectItemName).DetectItemConfig;
                                ((List<DetectItemModel>)cameraResultModel.DetectItems).Add(data);
                            }
                        }
                        cameraResultModels.Add(cameraResultModel);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString());
            }
            return cameraResultModels;
        }
        protected override void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (HYMessageBox.Show("是否退出应用", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                isWorking = false;
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
        protected override Assembly DeviceAssembly => null;

        protected override IMainBLL<ProductInfo, ProductTypeInfo> BLL => new MainBLL();

        protected override IProductTypeBLL TypeBLL => new ProductTypeBLL();

        protected override void Item_PushResultEvent(object sender, object e)
        {
            try
            {
                Task.Run(() =>
                {
                if (sender is IDevice device)
                {
                    device.DataQueue.Enqueue(e);
                }
                });
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString());
            }
        }
        bool isWorking = true;
        private void Work()
        {
            Task.Factory.StartNew(() =>
            {
                while (isWorking)
                {
                    var start = workFlowRoot.nodeList.First(a => a.nodeType == 1);
                    work(start);
                    Thread.Sleep(10);
                    foreach (var item in workFlowRoot.nodeList)
                    {
                        item.isFinish = false;
                    }

                }
            }, TaskCreationOptions.LongRunning);
        }
        private void work(NodeListItem node)
        {
            switch (node.nodeType)
            {
                case 1://开始
                    {
                        var nodeItems = workFlowRoot.nodeList.Where(a => node.targetRefs.Select(b => b.targetRef).Contains(a.nodeId));
                        node.isFinish = true;
                        if (nodeItems?.Count() > 0)
                        {
                            Parallel.ForEach(nodeItems, item => { work(item); });
                        }
                        else
                        {
                            LogHelper.Instance.AddLog("没有下节点");
                        }
                    }
                    break;
                case 2://结束节点
                    {

                    }
                    break;
                case 3://等待节点
                    {
                        var nodeItems = workFlowRoot.nodeList.Where(a => node.targetRefs.Select(b => b.targetRef).Contains(a.nodeId));
                        var device = WorkDevices.FirstOrDefault(a => a.DeviceName == node.hardwareName);
                        LogHelper.Instance.AddLog($"{node.nodeName}-{node.hardwareName}等待");
                        if (device != null)
                        {
                            while (isWorking && device.DataQueue.Count < 1)
                            {

                                Thread.Sleep(50);
                            }
                        }
                        node.isFinish = true;
                        if (nodeItems?.Count() > 0)
                        {
                            Parallel.ForEach(nodeItems, item => { work(item); });
                        }
                        else
                        {
                            LogHelper.Instance.AddLog("没有下节点");
                        }
                    }
                    break;
                case 4://任务节点
                    {
                        var nodeItems = workFlowRoot.nodeList.Where(a => node.targetRefs.Select(b => b.targetRef).Contains(a.nodeId));
                        if (node.taskType == null)
                        {

                        }
                        //创建对象
                        else if (node.taskType == 1)
                        {
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
                                CurrentProduct = new ProductInfoModel { Barcode = TempProductInfo.Barcode, TypeCode = TempProductInfo.TypeCode, TypeName = TempProductInfo.TypeName, CameraDatas = new ObservableCollection<CameraResultModel>() };
                                TempProductInfo = null;
                            }
                        }
                        //等待产品到位
                        else if (node.taskType == 2)
                        {
                            var device = WorkDevices.FirstOrDefault(a => a.DeviceName == node.hardwareName);
                            if (device != null)
                            {
                                switch (device.DeviceEnum)
                                {
                                    case DeviceEnum.Scanner:
                                        break;
                                    case DeviceEnum.AreaCamera:
                                        break;
                                    case DeviceEnum.LinearCamera:
                                        break;
                                    case DeviceEnum._3DCamera:
                                        break;
                                    case DeviceEnum.PLC:
                                        {
                                            while (isWorking && !device.DataQueue.TryDequeue(out object state))
                                            {
                                                System.Threading.Thread.Sleep(50);
                                            }
                                        }
                                        break;
                                    case DeviceEnum.Printer:
                                        break;
                                    case DeviceEnum.Robot:
                                        break;
                                    case DeviceEnum.Laser:
                                        break;
                                    case DeviceEnum.GPIO:
                                        break;
                                    case DeviceEnum.Server:
                                        break;
                                    case DeviceEnum.Algorithm:
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        //查询产品信息
                        else if (node.taskType == 3)
                        {
                            ProductTypeData data = ServerConnector.QueryTypeInfo($"http://{ip}:8848/device/get-type-code-config", uuid, CurrentProduct.TypeCode);
                            if (data != null)
                            {
                                TempProductType = new ProductTypeModel();
                                TempProductType.TypeCode = data.TypeCode;
                                TempProductType.TypeName = data.TypeName;
                                List<ProductTypeDetailModel> details = new List<ProductTypeDetailModel>();
                                foreach (var item in data.hardwareList)//相机列表
                                {
                                    ProductTypeDetailModel detailModel = new ProductTypeDetailModel();
                                    detailModel.CameraName = item.hardwareName;
                                    if (item.extraCfg != null)
                                    {
                                        detailModel.CameraExposure = double.Parse(item.extraCfgData["exposure"].ToString());
                                        detailModel.CameraGain = double.Parse(item.extraCfgData["gain"].ToString());
                                    }
                                    detailModel.TemplateImagePath = item.imgPath;
                                    var detects = new List<DetectItemModel>();
                                    foreach (var item1 in item.checkItemList)//该相机检测项
                                    {
                                        DetectItemModel detectItemModel = new DetectItemModel();
                                        if (item1.xmin.HasValue && item1.ymin.HasValue && item1.xmax.HasValue && item1.ymax.HasValue)
                                        {
                                            detectItemModel.DetectItemLocation = new System.Windows.Point(item1.xmin.Value, item1.ymin.Value);
                                            detectItemModel.DetectItemRegion = new Int32Rect(item1.xmin.Value, item1.ymin.Value, item1.xmax.Value - item1.xmin.Value, item1.ymax.Value - item1.ymin.Value);
                                        }
                                        detectItemModel.CameraName = item.hardwareName;
                                        detectItemModel.IsUsing = item1.uses == 1;
                                        detectItemModel.TypeCode = data.TypeCode;
                                        var d = detectItems.FirstOrDefault(a => a.checkItemId == item1.checkItemId);
                                        var algorithm = algorithmConfigList.FirstOrDefault(a => a.algorithmVersionId == d.algorithmVersionId);
                                        var algorithmConfig = new AlgorithmConfigModel
                                        {
                                            AlgorithmUrl = algorithm.servicePath,
                                            AlgorithmName = algorithm.algorithmName,
                                            AlgorithmType = algorithm.AlgorithmType.Value
                                        };
                                        foreach (var item2 in item1.assistParam)
                                        {
                                            AlgorithmUtilsModel a = new AlgorithmUtilsModel();
                                            a.AlgorithmUtilsName = item2.auxKey;
                                            a.AlgorithmUtilsValue = item2.auxValue;
                                            a.IsStandardValue = item2.standard == "1";
                                            algorithmConfig.AlgorithmUtilsItems.Add(a);
                                        }
                                        detectItemModel.DetectItemConfig = new DetectItemConfigModel
                                        {
                                            DetectItemName = d.checkItemName,
                                            DetectItemDesc = d.checkItemName,
                                            MarkerBorderBrushStr = d.color,
                                            AlgorithmConfig = algorithmConfig
                                        };
                                        detects.Add(detectItemModel);
                                    }
                                    detailModel.DetectItems = new ObservableCollection<DetectItemModel>(detects);
                                    details.Add(detailModel);
                                }
                                TempProductType.Details = details;
                            }
                            CurrentProduct.TypeName = TempProductType?.TypeName;
                        }
                        //设置相机参数
                        else if (node.taskType == 4)
                        {
                            var camera = (AbstractCamera)WorkDevices.Where(d => d is AbstractCamera).FirstOrDefault(a => a.DeviceName == node.hardwareName);
                            double? exp = TempProductType?.Details?.FirstOrDefault(b => b.CameraName == camera.DeviceName)?.CameraExposure;
                            double? gain = TempProductType?.Details?.FirstOrDefault(b => b.CameraName == camera.DeviceName)?.CameraGain;
                            if (exp.HasValue && exp.Value > 0)
                            {
                                camera?.SetExposureTime(exp.Value);
                            }
                            if (gain.HasValue && gain.Value > 0)
                            {
                                camera?.SetGain(gain.Value);
                            }
                            //camera?.SoftTrigger();
                            node.isFinish = true;
                        }
                        //执行算法检测项
                        else if (node.taskType == 5)
                        {
                            try
                            {
                                var camera = (AbstractCamera)WorkDevices.Where(d => d is AbstractCamera).FirstOrDefault(a => a.DeviceName == node.hardwareName);
                                if (!camera.DataQueue.TryDequeue(out object e) || !(e is Bitmap bitmap))
                                {
                                    return;
                                }
                                if (CurrentProduct == null || CurrentProduct.IsOK.HasValue)
                                {
                                    LogHelper.Instance.AddLog($"误触发信号");
                                    //string dir = Path.Combine(GlobalConfig.Instance.CacheFolder, camera.DeviceName);
                                    //if (!Directory.Exists(dir))
                                    //{
                                    //    Directory.CreateDirectory(dir);
                                    //}
                                    //string tempFilePath = dir + $"/{DateTime.Now:yyyyMMddHHmmssfff}.jpg";
                                    //if (bitmap != null)
                                    //{
                                    //    ImageHelper.SaveImage(bitmap, tempFilePath, GlobalConfig.Instance.JpegQuality);
                                    //    bitmap.Dispose();
                                    //}
                                    return;
                                }
                                if (p_Cameras.Count(b => b.DeviceName == camera.DeviceName) >= SnapTimes)
                                {
                                    LogHelper.Instance.AddLog($"已存在{camera.DeviceDesc}相机数据");
                                    //string dir = Path.Combine(GlobalConfig.Instance.CacheFolder, camera.DeviceName);
                                    //if (!Directory.Exists(dir))
                                    //{
                                    //    Directory.CreateDirectory(dir);
                                    //}
                                    //string tempFilePath = dir + $"/{DateTime.Now:yyyyMMddHHmmssfff}.jpg";
                                    //if (bitmap != null)
                                    //{
                                    //    ImageHelper.SaveImage(bitmap, tempFilePath, GlobalConfig.Instance.JpegQuality);
                                    //    bitmap.Dispose();
                                    //}
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
                                string filePath = GlobalManager.GetProductFolder(CurrentProduct) + $"/{CurrentProduct.Barcode}_{camera.DeviceName}_{CurrentProduct.CreateTime:yyyyMMddHHmmssfff}.jpg";
                                if (bitmap != null)
                                {
                                    ImageHelper.SaveImage(bitmap, filePath, GlobalConfig.Instance.JpegQuality);
                                    bitmap.Dispose();
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
                                        cameraData.DetectItems = TempProductType.Details.FirstOrDefault(c => c.CameraName == camera.DeviceName).DetectItems.Where(a => a.CameraName == cameraData.CameraName);
                                        bool checkRet = true;
                                        if (cameraData.DetectItems.Count() > 0)
                                        {
                                            LogHelper.Instance.AddLog($"{cameraData.CameraDesc}开始检测");
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
                                                            //裁剪图片 通过算法中间件处理
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
                                node.isFinish = true;
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
                        //合并结果
                        else if (node.taskType == 6)
                        {
                            if (CurrentProduct.CameraDatas.Count == CameraImageCount && CurrentProduct.CameraDatas.All(a => a?.CameraRetIsOK != null))
                            {
                                while (p_Cameras.TryDequeue(out AbstractCamera camera))
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
                                            CurrentProduct.TempLog.Append($"({item.CameraDesc}—{JsonUtils.JsonSerialize(data.Select(a => a.DetectItemConfig.DetectItemDesc).ToList())}—NG)");

                                        }
                                    }
                                }
                                LogHelper.Instance.AddLog($"{CurrentProduct.Barcode}检测结束,结果{CurrentProduct.IsOK}");
                            }
                            else
                            {
                                return;
                            }
                        }
                        //设置结果
                        else if (node.taskType == 7)
                        {
                            var device = WorkDevices.FirstOrDefault(a => a.DeviceName == node.hardwareName);
                            if (device != null)
                            {
                                switch (device.DeviceEnum)
                                {
                                    case DeviceEnum.Scanner:
                                        break;
                                    case DeviceEnum.AreaCamera:
                                        break;
                                    case DeviceEnum.LinearCamera:
                                        break;
                                    case DeviceEnum._3DCamera:
                                        break;
                                    case DeviceEnum.PLC:
                                        {
                                            var items = node.sourceRefs;
                                            foreach (var item in items)
                                            {
                                                if (item.condition == "0")
                                                {
                                                    ((AbstractPLC)device).SetNG();
                                                }
                                                else
                                                {
                                                    ((AbstractPLC)device).SetOK();
                                                }
                                            }
                                        }
                                        break;
                                    case DeviceEnum.Printer:
                                        break;
                                    case DeviceEnum.Robot:
                                        break;
                                    case DeviceEnum.Laser:
                                        break;
                                    case DeviceEnum.GPIO:
                                        break;
                                    case DeviceEnum.Server:
                                        break;
                                    case DeviceEnum.Algorithm:
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        //数据留存
                        else if (node.taskType == 8)
                        {

                            SpeakerHelper.Speak_Voice($"{CurrentProduct.Barcode}检测结果{(CurrentProduct.IsOK == true ? "OK" : "NG")}", 3);
                            ServerConnector.UploadResult($"http://{ip}:8848/device/download-check-item-config-file2", uuid,CurrentProduct.ConvertTo());
                            SetProductLog(CurrentProduct);
                        }
                        //图片上传
                        else if (node.taskType == 9)
                        {

                            var camera = (AbstractCamera)WorkDevices.Where(d => d is AbstractCamera).FirstOrDefault(a => a.DeviceName == node.hardwareName);
                            LogHelper.Instance.AddLog($"线程ID{Thread.CurrentThread.ManagedThreadId}:收到{CurrentProduct?.Barcode}---{camera.DeviceDesc}数据,图片上传");
                            if (!camera.DataQueue.TryPeek(out object e) || !(e is Bitmap bitmap))
                            {
                                return;
                            }
                            Bitmap saveBitmap = (Bitmap)bitmap.Clone();
                            string saveFileName = $"{CurrentProduct.Barcode}_{camera.DeviceName}_{CurrentProduct.CreateTime.ToString("yyyyMMddHHmmssfff")}.jpg";
                            Task.Run(() =>
                            {
                                try
                                {
                                    if (ServerConnector.UploadImage($"http://{ip}:8848/data-keep/save-image", uuid, saveBitmap, saveFileName))
                                    {
                                        LogHelper.Instance.AddLog($"{saveFileName}图片上传成功");
                                    }
                                    else
                                    {
                                        LogHelper.Instance.AddLog($"{saveFileName}图片上传失败");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.Instance.AddLog($"{saveFileName}图片上传异常：" + ex.ToString());
                                }
                                finally
                                {
                                    saveBitmap.Dispose();
                                }
                            });
                        }
                        node.isFinish = true;
                        if (nodeItems?.Count() > 0)
                        {
                            LogHelper.Instance.AddLog($"{node.nodeName}---任务完成");
                            Parallel.ForEach(nodeItems, item => { work(item); });
                        }
                        else
                        {
                            LogHelper.Instance.AddLog("没有下节点");
                        }
                    }
                    break;
                case 5://判断节点
                    {
                        var nodeItems = workFlowRoot.nodeList.Where(a => node.targetRefs.Select(b => b.targetRef).Contains(a.nodeId));
                        var device = WorkDevices.FirstOrDefault(a => a.DeviceName == node.hardwareName);
                        if (device != null)
                        {
                            switch (device.DeviceEnum)
                            {
                                case DeviceEnum.Scanner:
                                    {
                                        if (device.DataQueue.TryDequeue(out object e))
                                        {
                                            try
                                            {
                                                string barcode = e.ToString();
                                                LogHelper.Instance.AddLog($"扫码枪收到条码{barcode}");
                                                if (barcode.Length > TypeCodeLength)
                                                {
                                                    if (TempProductInfo != null)
                                                    {
                                                        LogHelper.Instance.AddLog($"已有产品条码在处理，新条码将其覆盖");
                                                    }
                                                    TempProductInfo = new ProductInfoModel
                                                    {
                                                        Barcode = barcode,
                                                        TypeCode = barcode.Substring(0, TypeCodeLength)
                                                    };
                                                    node.isFinish = true;
                                                    var items = workFlowRoot.nodeList.Where(a => node.targetRefs.Where(b => b.condition == "1").Select(c => c.targetRef).Contains(a.nodeId));
                                                    if (items?.Count() > 0)
                                                    {
                                                        Parallel.ForEach(items, item => { work(item); });
                                                    }
                                                }
                                                else
                                                {
                                                    LogHelper.Instance.AddLog($"扫码枪无效数据{barcode}");
                                                    node.isFinish = true;
                                                    var items = workFlowRoot.nodeList.Where(a => node.targetRefs.Where(b => b.condition == "0").Select(c => c.targetRef).Contains(a.nodeId));
                                                    if (items?.Count() > 0)
                                                    {
                                                        Parallel.ForEach(items, item => { work(item); });
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                LogHelper.Instance.AddLog(ex.ToString(), true);
                                            }
                                        }
                                    }
                                    break;
                                case DeviceEnum.AreaCamera:
                                    break;
                                case DeviceEnum.LinearCamera:
                                    break;
                                case DeviceEnum._3DCamera:
                                    break;
                                case DeviceEnum.PLC:
                                    {
                                        try
                                        {
                                            //判断型号信息是否已维护
                                            ProductTypeData data = ServerConnector.QueryTypeInfo($"http://{ip}:8848/device/get-type-code-config", uuid, CurrentProduct.TypeCode);
                                            if (data == null)
                                            {
                                                TempProductInfo.IsOK = false;
                                                TempProductInfo.TempLog.Append("不存在型号信息");
                                                node.isFinish = true;
                                                var items = workFlowRoot.nodeList.Where(a => node.targetRefs.Where(b => b.condition == "0").Select(c => c.targetRef).Contains(a.nodeId));
                                                if (items?.Count() > 0)
                                                {
                                                    Parallel.ForEach(items, item => { work(item); });
                                                }
                                            }
                                            else
                                            {
                                                node.isFinish = true;
                                                var items = workFlowRoot.nodeList.Where(a => node.targetRefs.Where(b => b.condition == "1").Select(c => c.targetRef).Contains(a.nodeId));
                                                if (items?.Count() > 0)
                                                {
                                                    Parallel.ForEach(items, item => { work(item); });
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            LogHelper.Instance.AddLog(ex.ToString(), true);
                                        }
                                    }
                                    break;
                                case DeviceEnum.Printer:
                                    break;
                                case DeviceEnum.Robot:
                                    break;
                                case DeviceEnum.Laser:
                                    break;
                                case DeviceEnum.GPIO:
                                    break;
                                case DeviceEnum.Server:
                                    break;
                                case DeviceEnum.Algorithm:
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            if (node.nodeName.Contains("是否维护"))
                            {
                                try
                                {
                                    //判断型号信息是否已维护
                                    ProductTypeData data = ServerConnector.QueryTypeInfo($"http://{ip}:8848/device/get-type-code-config", uuid, CurrentProduct.TypeCode);
                                    if (data == null)
                                    {
                                        if (TempProductInfo != null)
                                        {
                                            TempProductInfo.IsOK = false;
                                            TempProductInfo.TempLog.Append("不存在型号信息");
                                        }
                                        else if (CurrentProduct != null)
                                        {
                                            CurrentProduct.IsOK = false;
                                            CurrentProduct.TempLog.Append("不存在型号信息");
                                        }
                                        node.isFinish = true;
                                        var items = workFlowRoot.nodeList.Where(a => node.targetRefs.Where(b => b.condition == "0").Select(c => c.targetRef).Contains(a.nodeId));
                                        if (items?.Count() > 0)
                                        {
                                            Parallel.ForEach(items, item => { work(item); });
                                        }
                                    }
                                    else
                                    {
                                        node.isFinish = true;
                                        var items = workFlowRoot.nodeList.Where(a => node.targetRefs.Where(b => b.condition == "1").Select(c => c.targetRef).Contains(a.nodeId));
                                        if (items?.Count() > 0)
                                        {
                                            Parallel.ForEach(items, item => { work(item); });
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.Instance.AddLog(ex.ToString(), true);
                                }
                            }
                            else if (node.nodeName.Contains("结果判断"))
                            {
                                try
                                {
                                    if (CurrentProduct.IsOK == true)
                                    {
                                        node.isFinish = true;
                                        var items = workFlowRoot.nodeList.Where(a => node.targetRefs.Where(b => b.condition == "1").Select(c => c.targetRef).Contains(a.nodeId));
                                        if (items?.Count() > 0)
                                        {
                                            Parallel.ForEach(items, item => { work(item); });
                                        }
                                    }
                                    else
                                    {
                                        node.isFinish = true;
                                        var items = workFlowRoot.nodeList.Where(a => node.targetRefs.Where(b => b.condition == "0").Select(c => c.targetRef).Contains(a.nodeId));
                                        if (items?.Count() > 0)
                                        {
                                            Parallel.ForEach(items, item => { work(item); });
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
                    break;
                case 6://并行节点
                    {
                        Thread.Sleep(10);
                        if (!node.isFinish)
                        {
                            node.isFinish = true;
                            var datas = workFlowRoot.nodeList.Where(a => node.sourceRefs.Select(b => b.sourceRef).Contains(a.nodeId));
                            while (!datas.All(a => a.isFinish))
                            {
                                Thread.Sleep(50);
                            }
                            var nodeItems = workFlowRoot.nodeList.Where(a => node.targetRefs.Select(b => b.targetRef).Contains(a.nodeId));
                            if (nodeItems?.Count() > 0)
                            {
                                Parallel.ForEach(nodeItems, item => { work(item); });
                            }
                            else
                            {
                                LogHelper.Instance.AddLog("没有下节点");
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        protected override void SetProductLog(ProductInfoModel productInfoModel)
        {
            try
            {
                if (productInfoModel == null) return;
                productInfoModel.WorkLog = productInfoModel.TempLog.Length > 0 ? productInfoModel.TempLog.ToString() : "产品合格";
                productInfoModel.TempLog.Clear();
                string productJson = JsonUtils.JsonSerialize(productInfoModel);
                if (Directory.Exists(GlobalManager.GetProductFolder(productInfoModel)))
                {
                    Core.FileUtils.FileAppendText.AppendText(GlobalManager.GetProductFolder(productInfoModel) + $"/DetectRet_{productInfoModel.CreateTime.ToString("yyyyMMddHHmmssfff")}.data", $"{productJson}{Environment.NewLine}");
                }
                foreach (var item in DetectItemStatistics)
                {
                    if (item.DetectItemConfig.DetectItemName == "ALL")
                    {
                        item.ProductStatistics.TotalOutputs += 1;
                        try
                        {
                            if (!BLL.AddProductData(productInfoModel.ConvertTo()))
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
                            var data = JsonUtils.JsonDeserialize<ProductInfoModel>(productJson);
                            ProductLogs.Insert(0, data);
                        });
                    }
                    else if (item.DetectItemConfig.DetectItemName == "Unmaintained")
                    {
                        if (productInfoModel.IsOK == true && productInfoModel.WorkLog.Contains("未维护"))
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
        protected override void Reset()
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
                AbstractPLC plc = WorkDevices.FirstOrDefault(a => a.DeviceEnum == DeviceEnum.PLC) as AbstractPLC;
                if (plc != null)
                {
                    if (! plc.SetNG())
                    {
                        LogHelper.Instance.AddLog($"复位PLC有误", true);
                    }
                }
            }
        }
    }
}
