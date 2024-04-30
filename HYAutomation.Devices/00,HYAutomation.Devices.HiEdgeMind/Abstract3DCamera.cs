using HYAutomation.Device;
using HYAutomation.Devices.HiEdgeMind.Utils;
using HYWindowUtils.WPF.IconfontUtil;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using ColorMap = HYAutomation.Devices.HiEdgeMind.Utils.ColorMap;

namespace HYAutomation.Devices.HiEdgeMind
{
    public abstract class Abstract3DCamera : AbstractDevice
    {
        #region Field

        const double cam_z_pitch = 0.00001;//HeightData的Int32数值 除以10万 等于实际mm数

        SsznCam m_Cam = new SsznCam();

        System.Timers.Timer ErrT = new System.Timers.Timer(1000);//无限循环模式下用    //实例化Timer类，设置间隔时间为1000毫秒；

        SHOW_COLOR_MAP m_Color = SHOW_COLOR_MAP.SSZN_COLOR;//GRAY

        int mCurDispType = 0;//0 高度图 1 灰度图

        //Int32[] HeightData = new Int32[15000 * 3200];

        //PointCloudHead mPcHead = new PointCloudHead();

        #endregion
        #region 相机配置
        public abstract int DeviceID { get; set; }
        public abstract string IP { get; set; }
        #endregion
        public override object Icon
        {
            get
            {
                try
                {
                    return IconfontEnum.LinearCamera.GetIconFontKey();
                }
                catch
                {
                    return null;
                }
            }
        }
        public override UIElement UserInterface => throw new NotImplementedException();

        public override DeviceEnum DeviceEnum { get; } = DeviceEnum._3DCamera;

        public override event EventHandler<object> PushResultEvent;
        public override bool Connect()
        {
            try
            {
                if (base.Connect())
                {
                    return true;
                }
                if (m_Cam.init(DeviceID) == 0)
                {
                    IsOnline = m_Cam.connect(IP, 0) == 0;
                }
                if (IsOnline)
                {
                    IsOnline = m_Cam.start() == 0;
                }
                if (IsOnline)
                {
                    m_Cam.HasNewData += M_Cam_HasNewData;
                }
                return IsOnline;
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
                return false;
            }
        }

        private void M_Cam_HasNewData(object sender, EventArgs e)
        {
            try
            {
                int camNum = 1;
                int errGC = m_Cam.getCamBOnline(out camNum);

                try
                {
                    int mBatchWidth = SR7LinkFunc.SR7IF_ProfileDataWidth(0, new IntPtr()) / camNum;
                    int mBatchPoint = SR7LinkFunc.SR7IF_ProfilePointSetCount(0, new IntPtr());
                    addDeviceLog($"收到数据{mBatchWidth}*{mBatchPoint}", false);
                    if (mCurDispType == 0)
                    {
                        double heightRange = 0.0;

                        if (0 != m_Cam.getCamHeight(out heightRange))
                        {
                            addDeviceLog("GetCamHeight Err.");
                            return;
                        }

                        int Min = (int)(-heightRange / cam_z_pitch);
                        int Max = (int)(heightRange / cam_z_pitch);

                        BatchDataShow(m_Cam.HeightData[0], Min, Max, mBatchWidth, mBatchPoint);

                    }
                    else if (mCurDispType == 1)
                    {
                        GrayDataShow(m_Cam.GrayData[0], mBatchWidth, mBatchPoint);
                    }
                }
                catch (Exception ex)
                {
                    addDeviceLog(ex.ToString());
                }

            }
            catch (Exception ex)
            {
                addDeviceLog("ImageDisplayFunc Err." + ex.Message);
                return;
            }

        }

        public override bool DisConnect()
        {
            try
            {
                return m_Cam.disconnect() == 0;
                m_Cam.HasNewData -= M_Cam_HasNewData;
                //批处理停止
                int errS = m_Cam.stop();
                addDeviceLog("停止批处理" + (errS == 0 ? "成功!" : "失败!"), false);

                //关闭设备
                int errD = m_Cam.disconnect();
                addDeviceLog("设备关闭" + (errD == 0 ? "成功!" : "失败!"), false);


                int errU = m_Cam.uninit();

            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString(), true);
                return false;
            }
        }
        // 保存高度图
        public void SaveHeightData()
        {

            if (m_Cam.HeightData[0] == null)
            {
                addDeviceLog(" 高度数据为空！", false);
                return;
            }
            OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Ply (*.tiff)|*.tiff|Ecd (*.ecd)|*.ecd|Pcd (*.pcd)|*.pcd|Ply (*.ply)|*.ply"
            };
            if (ofd.ShowDialog() == true)
            {

                //string strFile = HeightDataSaveDlg.SafeFileName;
                string str = Path.GetFullPath(ofd.FileName);

                //获取文件的扩展名       
                string strExt = Path.GetExtension(ofd.FileName);

                PointCloudHead pcHead = new PointCloudHead();

                pcHead._height = SR7LinkFunc.SR7IF_ProfilePointSetCount(0, new IntPtr());
                pcHead._width = SR7LinkFunc.SR7IF_ProfileDataWidth(0, new IntPtr());
                int errG = m_Cam.getParam(0, 1, (int)SR7IF_SETTING_ITEM.X_PITCH, out pcHead._xInterval);

                //yInterval 需根据情况填写, yInterval = 细化点数 * 每脉冲mm数 
                pcHead._yInterval = pcHead._xInterval;
                switch (strExt)
                {
                    case ".tiff":
                        {
                            EcdClass.WriteTif(str, m_Cam.HeightData[0], pcHead);
                        }
                        break;
                    case "*.ecd":
                        {
                            EcdClass.WriteEcd(str, m_Cam.HeightData[0], pcHead);
                        }
                        break;
                    case ".pcd":
                        {

                            EcdClass.WritePcd(str, m_Cam.HeightData[0], pcHead);
                        }
                        break;
                    case ".ply":
                        {

                            EcdClass.WritePly(str, m_Cam.HeightData[0], pcHead);
                        }
                        break;
                    default: break;
                }

                addDeviceLog("保存完成！", false);

            }

        }
        // 保存高度图
        public void SaveHeightData(string filePath)
        {


            //string strFile = HeightDataSaveDlg.SafeFileName;
            string str = Path.GetFullPath(filePath);

            //获取文件的扩展名       
            string strExt = Path.GetExtension(filePath);

            PointCloudHead pcHead = new PointCloudHead();

            pcHead._height = SR7LinkFunc.SR7IF_ProfilePointSetCount(0, new IntPtr());
            pcHead._width = SR7LinkFunc.SR7IF_ProfileDataWidth(0, new IntPtr());
            int errG = m_Cam.getParam(0, 1, (int)SR7IF_SETTING_ITEM.X_PITCH, out pcHead._xInterval);

            //yInterval 需根据情况填写, yInterval = 细化点数 * 每脉冲mm数 
            pcHead._yInterval = pcHead._xInterval;
            switch (strExt)
            {
                case ".tiff":
                    {
                        EcdClass.WriteTif(str, m_Cam.HeightData[0], pcHead);
                    }
                    break;
                case "*.ecd":
                    {
                        EcdClass.WriteEcd(str, m_Cam.HeightData[0], pcHead);
                    }
                    break;
                case ".pcd":
                    {

                        EcdClass.WritePcd(str, m_Cam.HeightData[0], pcHead);
                    }
                    break;
                case ".ply":
                    {

                        EcdClass.WritePly(str, m_Cam.HeightData[0], pcHead);
                    }
                    break;
                default: break;
            }

        }
        // 保存亮度图
        public void SaveGrayData()
        {
            try
            {
                if (m_Cam.GrayData[0] == null)
                {
                    addDeviceLog("灰度数据为空！", false);
                    return;
                }
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.Filter = "图像文件 (*.bmp)|*.bmp";

                if (ofd.ShowDialog() == true)
                {
                    string fileEx = Path.GetExtension(ofd.FileName);    //获取扩展名   

                    if (fileEx == ".bmp")
                    {

                        int imgW = 0;
                        int imgH = 0;

                        m_Cam.getParam(0, 1, (int)SR7IF_SETTING_ITEM.X_PIXEL, out imgW);
                        m_Cam.getParam(0, 1, (int)SR7IF_SETTING_ITEM.BATCH_POINT, out imgH);

                        Bitmap tmpBmp = null;
                        m_Color = SHOW_COLOR_MAP.GRAY;
                        data2Bmp2(m_Cam.GrayData[0], imgW, imgH, ref tmpBmp, m_Color);
                        tmpBmp.Save(ofd.FileName, ImageFormat.Bmp);
                        addDeviceLog("保存完成！", false);
                    }
                }
            }
            catch (Exception ex)
            {
                addDeviceLog("保存失败!异常!" + ex.Message);
                return;

            }
        }
        public void SaveGrayData(string filePath)
        {
            try
            {
                if (m_Cam.GrayData[0] == null)
                {
                    addDeviceLog("灰度数据为空！", false);
                    return;
                }
                string fileEx = Path.GetExtension(filePath);    //获取扩展名   

                if (fileEx == ".bmp")
                {

                    int imgW = 0;
                    int imgH = 0;

                    m_Cam.getParam(0, 1, (int)SR7IF_SETTING_ITEM.X_PIXEL, out imgW);
                    m_Cam.getParam(0, 1, (int)SR7IF_SETTING_ITEM.BATCH_POINT, out imgH);

                    Bitmap tmpBmp = null;
                    m_Color = SHOW_COLOR_MAP.GRAY;
                    data2Bmp2(m_Cam.GrayData[0], imgW, imgH, ref tmpBmp, m_Color);
                    tmpBmp.Save(filePath, ImageFormat.Bmp);
                }
            }
            catch (Exception ex)
            {
                addDeviceLog("保存失败!异常!" + ex.ToString());
                return;

            }
        }
        // 保存编码器数
        public void SaveEncoderDatak()
        {
            try
            {
                if (m_Cam.EncoderData[0] == null)
                {
                    addDeviceLog("编码器数据为空！", false);
                    return;
                }

                int camNum = 1;
                int BatchPoint = 0;

                m_Cam.getCamBOnline(out camNum);
                m_Cam.getParam(0, 1, (int)SR7IF_SETTING_ITEM.BATCH_POINT, out BatchPoint);
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.Filter = "文本文件 (*.txt)|*.txt";
                if (ofd.ShowDialog() == true)
                {
                    using (StreamWriter sw = new StreamWriter(ofd.FileName, false, Encoding.GetEncoding("utf-16")))
                    {
                        for (int i = 0; i < BatchPoint; i++)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendFormat("{0}\t", m_Cam.EncoderData[0][i]);
                            sw.WriteLine(sb);
                        }

                    }
                    addDeviceLog("保存完成！", false);
                }
            }
            catch (Exception ex)
            {
                addDeviceLog("保存失败!异常!" + ex.Message);
                return;
            }


        }
        public bool SetExposureTime(int expTime)
        {
            try
            {
                if (expTime < (int)SR7IF_EXP_TIME.T10US || expTime > (int)SR7IF_EXP_TIME.T9800US)
                {
                    addDeviceLog($"参数无效，曝光级值范围[{SR7IF_EXP_TIME.T10US+1},{SR7IF_EXP_TIME.T9800US+1}]");
                    return false;
                }
                int ret = m_Cam.setParam(0, 1, (int)SR7IF_SETTING_ITEM.EXP_TIME, expTime);
                if (ret == 0)
                {
                    return true;
                }
                else
                {
                    addDeviceLog($"设置曝光失败,返回值{ret},错误信息:{m_Cam.str_Err}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 高度数据转换为图像显示----数据全部显示
        /// </summary>
        /// <param name="_BatchData"></param>     高度数据
        /// <param name="max_height"></param>     高度显示区域最大值
        /// <param name="min_height"></param>     高度显示区域最小值
        /// <param name="_ColorMax"></param>      灰度级数 ---- 256
        /// <param name="img_w"></param>          图像宽度
        /// <param name="img_h"></param>          图像高度
        private void BatchDataShow(int[] _BatchData, int img_w, int img_h)
        {
            int show_w = 800;
            int show_h = 500;
            byte[] dataByte = new byte[img_w * img_h];
            byte[] dataZoom = new byte[show_w * show_h];

            double heightRange = 0.0;

            if (0 != m_Cam.getCamHeight(out heightRange))
            {
                addDeviceLog("GetCamHeight Err.", false);
                return;
            }

            Int32 InMin = Int32.MaxValue;
            Int32 InMax = Int32.MinValue;
            Int32 limit_min = -9900 * 100000;
            Int32 limit_max = 9900 * 100000;
            MinMaxRange(_BatchData, limit_min, limit_max, out InMin, out InMax);

            int errT = TransImg(ref _BatchData, ref dataByte, InMin, InMax, 0, 255, img_w, img_h);

            int errZ = ZoomImg(dataByte, ref dataZoom, img_w, img_h, show_w, show_h);
            // 由于BMP图像对于行是倒置的，即图像显示的第一行是最后一行数据，所以要倒置
            //int errM = MirrorImg(ref dataZoom, show_w, show_h);

            Bitmap tmpBmp = null;

            int errB = data2Bmp(dataZoom, show_w, show_h, ref tmpBmp, m_Color);
            PushResultEvent?.Invoke(this, tmpBmp);
            GC.Collect();

        }

        private void BatchDataShow(int[] _BatchData, int InMin, int InMax, int img_w, int img_h)
        {

            int show_w = 800;
            int show_h = 500;
            byte[] dataByte = new byte[img_w * img_h];
            byte[] dataZoom = new byte[show_w * show_h];

            int errT = TransImg(ref _BatchData, ref dataByte, InMin, InMax, 0, 255, img_w, img_h);
            int errZ = ZoomImg(dataByte, ref dataZoom, img_w, img_h, show_w, show_h);
            // 由于BMP图像对于行是倒置的，即图像显示的第一行是最后一行数据，所以要倒置
            int errM = MirrorImg(ref dataZoom, show_w, show_h);

            Bitmap tmpBmp = null;

            int errB = data2Bmp(dataZoom, show_w, show_h, ref tmpBmp, m_Color);
            //????  比例
            PushResultEvent?.Invoke(this, tmpBmp);
            GC.Collect();

        }

        private void GrayDataShow(byte[] _grayData, int img_w, int img_h)
        {
            // 由于BMP图像对于行是倒置的，即图像显示的第一行是最后一行数据，所以要倒置
            //int errM = MirrorImg(ref _grayData, img_w, img_h);
            Bitmap tmpBmp = null;
            m_Color = SHOW_COLOR_MAP.GRAY;
            int errB = data2Bmp(_grayData, img_w, img_h, ref tmpBmp, m_Color);
            PushResultEvent?.Invoke(this, tmpBmp);
            GC.Collect();

        }

        //base func
        private int data2Bmp(byte[] data, int img_w, int img_h, ref Bitmap tmpBmp, SHOW_COLOR_MAP e_Color)
        {

            try
            {
                tmpBmp = new Bitmap(img_w, img_h, PixelFormat.Format8bppIndexed);
                // 256 调色板
                ColorPalette tmpPalette = tmpBmp.Palette;
                Color[] entries = tmpPalette.Entries;

                ColorMap.SetLut(e_Color);

                for (int i = 0; i < 256; i++)
                {
                    entries[i] = Color.FromArgb(ColorMap.nowTable[i, 2],
                                        ColorMap.nowTable[i, 1], ColorMap.nowTable[i, 0]);
                }

                tmpBmp.Palette = tmpPalette;

                BitmapData bmpData = tmpBmp.LockBits(
                    new Rectangle(0, 0, tmpBmp.Width, tmpBmp.Height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format8bppIndexed);

                Marshal.Copy(data, 0, bmpData.Scan0, tmpBmp.Width * tmpBmp.Height);
                tmpBmp.UnlockBits(bmpData);

            }
            catch (Exception ex)
            {

                addDeviceLog(ex.Message);
                return -1;
            }
            return 0;
        }

        private int data2Bmp2(byte[] data, int img_w, int img_h, ref Bitmap tmpBmp, SHOW_COLOR_MAP e_Color)
        {

            try
            {
                tmpBmp = new Bitmap(img_w, img_h, PixelFormat.Format8bppIndexed);
                // 256 调色板
                ColorPalette monoPalette = tmpBmp.Palette;
                Color[] entries = monoPalette.Entries;
                for (int i = 0; i < 256; i++)
                    entries[i] = Color.FromArgb(i, i, i);

                tmpBmp.Palette = monoPalette;


                BitmapData bmpData = tmpBmp.LockBits(
                    new Rectangle(0, 0, tmpBmp.Width, tmpBmp.Height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format8bppIndexed);

                Marshal.Copy(data, 0, bmpData.Scan0, tmpBmp.Width * tmpBmp.Height);
                tmpBmp.UnlockBits(bmpData);

            }
            catch (Exception ex)
            {

                addDeviceLog(ex.Message);
                return -1;
            }

            return 0;

        }

        private int TransImg(ref int[] dataInt, ref byte[] dataByte, Int32 InMin, Int32 InMax, byte OutMin, byte OutMax, int img_w, int img_h)
        {
            try
            {
                // 颜色区间与高度区间比例
                double fScale = ((double)(OutMax - OutMin)) / ((double)(InMax - InMin));
                int dataCount = img_w * img_h;

                for (int i = 0; i < dataCount; i++)
                {
                    if (dataInt[i] < InMin)
                        dataByte[i] = OutMin;
                    else if (dataInt[i] > InMax)
                        dataByte[i] = OutMax;
                    else
                    {
                        dataByte[i] = Convert.ToByte((dataInt[i] - InMin) * fScale + OutMin);
                    }
                }
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.Message);
                return -1;
            }

            return 0;
        }

        private int MirrorImg(ref byte[] data, int img_w, int img_h)
        {
            try
            {
                byte[] tmpData = new byte[img_w];

                for (int i = 0; i < img_h / 2; i++)
                {
                    Array.Copy(data, i * img_w, tmpData, 0, img_w);
                    Array.Copy(data, (img_h - i - 1) * img_w, data, i * img_w, img_w);
                    Array.Copy(tmpData, 0, data, (img_h - i - 1) * img_w, img_w);
                }

            }
            catch (Exception ex)
            {
                addDeviceLog(ex.Message);
                return -1;
            }

            return 0;

        }

        private int ZoomImg(byte[] data, ref byte[] outData, Int32 img_w, Int32 img_h, Int32 out_img_w, Int32 out_img_h)
        {
            double zoom_x = (1.0 * img_w) / (1.0 * out_img_w);
            double zoom_y = (1.0 * img_h) / (1.0 * out_img_h);

            for (int h = 0; h < out_img_h; h++)
            {
                for (int w = 0; w < out_img_w; w++)
                {
                    int w_in = (int)((double)w * zoom_x);
                    int h_in = (int)((double)h * zoom_y);

                    outData[h * out_img_w + w] = data[w_in + h_in * img_w];
                }
            }

            return 0;

        }

        //MinMax
        public int MinMaxRange(Int32[] array, Int32 limit_min, Int32 limit_max, out Int32 val_min, out Int32 val_max)
        {
            val_min = Int32.MaxValue;
            val_max = Int32.MinValue;

            try
            {
                if (array == null)
                {
                    return -2;
                }

                foreach (int x in array)
                {
                    if (x < val_min && x > limit_min) val_min = x;
                    if (x > val_max && x < limit_max) val_max = x;
                }

            }
            catch (Exception ex)
            {
                addDeviceLog(ex.Message);
                return -1;
            }

            return 0;
        }


        //buffer 拆分和合并        

        // 定时器---错误码获取 ???? 无限模式下用
        public void theout(object source, System.Timers.ElapsedEventArgs e)
        {
            /*****测试代码******/
            System.Console.Write("重新开始!\n");
            /*****测试代码******/
            //获取错误码
            int[] pbyErrCnt = new int[1];
            int[] pwErrCode = new int[1024];

            using (PinnedObject pin_pbyErrCnt = new PinnedObject(pbyErrCnt))
            {
                using (PinnedObject pin_pwErrCode = new PinnedObject(pwErrCode))
                {
                    SR7LinkFunc.SR7IF_GetBatchRollError(0, pin_pbyErrCnt.Pointer, pin_pwErrCode.Pointer);
                }
            }

            pwErrCode = null;
            pbyErrCnt = null;
            GC.Collect();

        }
    }
}
