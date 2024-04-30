using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace HYAutomation.Core.Views.Utils
{
    public class WriteableBitmapUtils
    {
        public string CameraName { get; set; }
        public WriteableBitmap WriteableBitmap { get; private set; }
        private void updateWritableBitmapData(byte[] byt, int Width, int Height, System.Drawing.Imaging.PixelFormat pixelFormat)
        {
            Action DoAction = delegate ()
            {
                try
                {
                    if (WriteableBitmap == null)
                    {
                        InitialWriteableBitmap(Width, Height, pixelFormat);
                    }
                    //锁住内存
                    WriteableBitmap.Lock();
                    if (byt == null)
                    {
                        return;
                    }
                    Marshal.Copy(byt, 0, WriteableBitmap.BackBuffer, byt.Length);
                    //指定更改位图的区域
                    WriteableBitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
                }
                finally
                {
                    WriteableBitmap.Unlock();
                }
            };
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                UIThreadInvoke(delegate
                {
                    DoAction();
                });
            }
            else
            {
                DoAction();
            }
        }
        private void UIThreadInvoke(Action Code)
        {
            try
            {
                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.Invoke(Code);
                    return;
                }
                Code.Invoke();
            }
            catch
            {
                /*仅捕获、不处理！*/
            }
        }

        /// <summary>
        /// 异步执行 注：外层Try Catch语句不能捕获Code委托中的错误
        /// </summary>
        private void UIThreadBeginInvoke(Action Code)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(Code);
                return;
            }
            Code.Invoke();
        }
        /// <summary>  
        /// Bitmap转换层RGB32  
        /// </summary>  
        /// <param name="Source">Bitmap图片</param>  
        /// <returns></returns>  
        private byte[] ConvertBitmap(System.Drawing.Bitmap BpSource, ref bool Is_Error)
        {
            byte[] pRrgaByte;
            try
            {
                int PicWidth = BpSource.Width;
                int PicHeight = BpSource.Height;
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, PicWidth, PicHeight);
                System.Drawing.Imaging.BitmapData bmp_Data = BpSource.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, BpSource.PixelFormat);
                IntPtr iPtr = bmp_Data.Scan0;
                int picSize = PicWidth * PicHeight * GetChanel(BpSource.PixelFormat);
                pRrgaByte = new byte[picSize];
                Marshal.Copy(iPtr, pRrgaByte, 0, picSize);
                BpSource.UnlockBits(bmp_Data);
                Is_Error = true;
                return pRrgaByte;
            }
            catch (Exception ex)
            {
                pRrgaByte = null;
                Is_Error = false;
            }
            return null;
        }
        private int GetChanel(System.Drawing.Imaging.PixelFormat pixelFormat)
        {
            int channel = 1;
            switch (pixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Indexed:
                    break;
                case System.Drawing.Imaging.PixelFormat.Gdi:
                    break;
                case System.Drawing.Imaging.PixelFormat.Alpha:
                    break;
                case System.Drawing.Imaging.PixelFormat.PAlpha:
                    break;
                case System.Drawing.Imaging.PixelFormat.Extended:
                    break;
                case System.Drawing.Imaging.PixelFormat.Canonical:
                    break;
                case System.Drawing.Imaging.PixelFormat.Undefined:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format1bppIndexed:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format4bppIndexed:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb555:
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb565:
                case System.Drawing.Imaging.PixelFormat.Format16bppArgb1555:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    channel = 3;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                    channel = 4;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format48bppRgb:
                    channel = 6;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format64bppPArgb:
                    channel = 8;
                    break;
                case System.Drawing.Imaging.PixelFormat.Max:
                    break;
                default:
                    break;
            }
            return channel;
        }
        /// <summary>
        /// 初始化WriteableBitmap
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="pixelFormat"></param>
        public void InitialWriteableBitmap(int Width, int Height, System.Drawing.Imaging.PixelFormat pixelFormat)
        {
            WriteableBitmap = new WriteableBitmap(Width, Height,
                            96, 96, ImageHelper.ConvertBmpPixelFormat(pixelFormat),
                          BitmapPalettes.Gray256);
        }
        /// <summary>
        /// 回调函数，获取图片
        /// </summary>
        /// <param name="bitmap"></param>
        public void GetImage(System.Drawing.Bitmap bp)
        {
            bool isbool = false;
            try
            {
                updateWritableBitmapData(ConvertBitmap(bp, ref isbool), bp.Width, bp.Height, bp.PixelFormat);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [DllImport("kernel32.dll")]
        private static extern void CopyMemory(IntPtr destination, IntPtr source, uint length);

    }
}
