using HY.Devices.Camera;
using HYAutomation.Core;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace HYAutomation.Device
{
    public abstract class AbstractCamera : AbstractDevice
    {
        #region 相机配置
        public abstract bool IslinearCamera { get; set; }
        public abstract string CameraFactory { get; set; }
        public virtual CameraConnectTypes CameraConnType { get; set; } = CameraConnectTypes.GigE;
        // GigE
        public virtual BoardInfoClasss BoardClass { get; set; } = BoardInfoClasss.CameraLink;
        public abstract string CameraSN { get; set; }
        public virtual string CameraConfig { get; set; }
        public virtual uint BoardIndex { get; set; } = 0;
        public virtual uint CameraIndex { get; set; } = uint.MaxValue;
        public virtual uint BufferFrameCount { get; set; } = 5;
        public virtual bool CanSetTrigger { get; set; } = false;
        public virtual TriggerMode TriggerMode { get; set; } = TriggerMode.OFF;
        public virtual TriggerSources TriggerSource { get; set; } = TriggerSources.Line0;
        #endregion
        public override event EventHandler<object> PushResultEvent;
        public override DeviceEnum DeviceEnum
        {
            get
            {
                return IslinearCamera ? DeviceEnum.LinearCamera : DeviceEnum.AreaCamera;
            }
        }
        protected IHYCamera _camera;
        public virtual bool CanGrabAfterConnect { get; set; } = true;
        public bool isGrabbing => _camera.IsGrabbing;
        public override bool Connect()
        {
            try
            {
                if (base.Connect())
                {
                    return true;
                }
                if (_camera != null)
                {
                    _camera.NewImageEvent -= HasNewData;
                    if (_camera.IsGrabbing)
                    {
                        _camera.StopGrab();
                    }
                    _camera.Dispose();
                    _camera = null;
                }
                Initialization();
                if (_camera != null)
                {
                    if (_camera.Open())
                    {
                        if (CanSetTrigger)
                        {
                            _camera.SetTriggerModel(TriggerMode);
                            _camera.SetTriggerSource(TriggerSource);
                        }
                        if (CanGrabAfterConnect)
                        {
                            _camera.ContinousGrab();
                        }
                    }
                    IsOnline = _camera.IsConnected;

                }
                return IsOnline;
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
                return false;
            }
        }
        public override bool DisConnect()
        {
            if (_camera != null)
            {
                try
                {
                    _camera.NewImageEvent -= HasNewData;
                    if (_camera.IsGrabbing)
                    {
                        _camera.StopGrab();
                    }
                    _camera.Close();
                    IsOnline = false;
                }
                catch (Exception ex)
                {
                    addDeviceLog(ex.ToString());
                    return false;
                }
            }
            base.Dispose();
            return true;
        }
        public override void Dispose()
        {
            try
            {
                DisConnect();
                _camera?.Dispose();
                _camera = null;
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
            }
            base.Dispose();
        }
        private Task _monitorTask;
        public virtual Task Initialization()
        {
            var tcs = new TaskCompletionSource<bool>();
            try
            {
                if (DeviceEnum == DeviceEnum.LinearCamera)
                {
                    if (CameraFactory == "Hik")
                    {
                        _camera = new HYCamera_HIKLinear(CameraConnType, CameraSN);
                    }
                    else if (CameraFactory == "IKap")
                    {
                        if (!string.IsNullOrEmpty(CameraSN))
                        {
                            _camera = new HYCamera_IKapLinear_Net(CameraSN);
                        }
                        else if(CameraIndex!=uint.MaxValue)
                        {
                            _camera = new HYCamera_IKapLinear_Net(CameraIndex);
                        }
                        if(!((AbstractHYCamera_IKap_Net)_camera).SetBufferCountOfStream(BufferFrameCount))
                        {
                            addDeviceLog("设置数据缓冲区有误");
                        }
                    }
                    else if (CameraFactory == "IKap_Borad")
                    {
                        if (!string.IsNullOrEmpty(CameraSN))
                        {
                            _camera = new HYCamera_IKapLinear_BoardPCIE_New(CameraSN, BoardIndex, CameraConfig, BoardClass);
                        }
                        else if (CameraIndex != uint.MaxValue)
                        {
                            _camera = new HYCamera_IKapLinear_BoardPCIE_New(CameraIndex, BoardIndex, CameraConfig, BoardClass);

                        }
                        if (!((AbstractHYCamera_IKap_BoardPCIE_New)_camera).SetBufferCountOfStream(BufferFrameCount))
                        {
                            addDeviceLog("设置数据缓冲区有误");
                        }
                    }
                    else if (CameraFactory == "Dalsa")
                    {
                        _camera = new HYCamera_DalsaLinear(CameraSN, "", 1);
                    }
                    else if (CameraFactory == "Basler")
                    {
                    }
                }
                else
                {
                    if (CameraFactory == "Hik")
                    {
                        _camera = new HYCamera_HIKArea(CameraConnType, CameraSN);
                    }
                    else if (CameraFactory == "IKap")
                    {
                        _camera = new HYCamera_IkapArea_Net(CameraSN);
                    }
                    else if (CameraFactory == "Dalsa")
                    {
                    }
                    else if (CameraFactory == "Basler")
                    {
                        _camera = new HYCamera_BaslerArea(CameraSN);
                    }
                }
                _camera.NewImageEvent -= HasNewData;
                _camera.NewImageEvent += HasNewData;
                if (_monitorTask == null || _monitorTask.IsCompleted)
                {
                    _monitorTask = Task.Factory.StartNew(() =>
                    {
                        while (_camera != null)
                        {
                            try
                            {
                                IsOnline = _camera.IsConnected;
                                System.Threading.Thread.Sleep(2000);
                            }
                            catch (Exception ex)
                            {
                                addDeviceLog(ex.ToString());
                            }
                        }
                    }, TaskCreationOptions.LongRunning);
                }
            }
            catch (Exception ex)
            {
                _camera = null;
                addDeviceLog(ex.ToString());
            }
            finally
            {
                tcs.SetResult(true);
            }
            return tcs.Task;
        }
        private void HasNewData(object sender, Bitmap newImg)
        {
            try
            {
                if (newImg == null)
                {
                    addDeviceLog($"{DeviceDesc}收到无效数据");
                    PushResultEvent?.Invoke(this, null);
                    return;
                }
                //addLog($"收到图片数据{newImg.Width}×{newImg.Height}",false);
                PushResultEvent?.Invoke(this, newImg);
            }
            catch(Exception ex)
            {
                addDeviceLog(ex.ToString());
            }
        }
        public bool SetExposureTime(double exposureTime)
        {
            try
            {
                if (_camera?.IsConnected != true)
                {
                    addDeviceLog($"未连接");
                    return false;
                }
                return _camera.SetExposureTime(exposureTime);
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
                return false;
            }
        }
        public bool SetGain(double gain)
        {
            try
            {
                if (_camera?.IsConnected != true)
                {
                    addDeviceLog($"未连接", true);
                    return false;
                }
                return _camera.SetGain(gain);
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
                return false;
            }
        }
        public Bitmap GrabOne()
        {
            try
            {
                if (_camera?.IsConnected != true)
                {
                    addDeviceLog($"未连接");
                    return null;
                }
                if (!_camera.GrabOne(out Bitmap bitmap))
                {
                    addDeviceLog($"拍照失败", true);
                }
                return bitmap;
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
                return null;
            }
        }
        public void Grab()
        {
            try
            {
                if (_camera?.IsConnected != true)
                {
                    addDeviceLog($"未连接");
                    return;
                }
                if (!_camera.IsGrabbing)
                {
                    if (!_camera.ContinousGrab())
                    {
                        addDeviceLog($"实时采集失败");
                    }
                }
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
            }
        }
        public void StopGrab()
        {
            try
            {
                if (_camera?.IsConnected != true)
                {
                    addDeviceLog($"未连接");
                    return;
                }
                if (_camera.IsGrabbing)
                {
                    if (!_camera.StopGrab())
                    {
                        addDeviceLog($"停止实时采集失败");
                    }
                }
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
            }
        }
        public void SoftTrigger()
        {

            try
            {
                if (_camera?.IsConnected != true)
                {
                    addDeviceLog($"未连接");
                    return;
                }
                if (!_camera.SoftWareTrigger())
                {
                    addDeviceLog($"软触发失败");
                }
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
            }
        }
        public bool SetImageWidth(uint width)
        {
            try
            {
                if (_camera?.IsConnected != true)
                {
                    addDeviceLog($"未连接", true);
                    return false;
                }
                return _camera.SetImageWidth(width);
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
                return false;
            }
        }
        public bool SetImageHeight(uint height)
        {
            try
            {
                if (_camera?.IsConnected != true)
                {
                    addDeviceLog($"未连接", true);
                    return false;
                }
                return _camera.SetImageHeight(height);
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
                return false;
            }
        }
    }
}
