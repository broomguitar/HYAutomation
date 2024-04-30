using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace HYAutomation.Core
{
    public class ImageHelper
    {
        /// <summary>
        /// Bitmap-->BitmapImage
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            try
            {
                if (bitmap == null)
                {
                    return null;
                }
                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                }
                return bitmapImage;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// Bitmap-->BitmapFrame
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapFrame BitmapToBitmapFrame(Bitmap bitmap)
        {
            try
            {
                if (bitmap == null)
                {
                    return null;
                }
                BitmapFrame bf = null;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    bf = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                }
                return bf;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        public static BitmapSource BitmapToBitmapSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();

            try
            {
                BitmapSource source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                return source;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }

        public static BitmapSource CompressBitmap(Bitmap bitmap, int quality)
        {
            BitmapSource bitmapSource = null;
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // 将位图保存为JPEG格式，并将其写入内存流
                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                    ImageCodecInfo jpegEncoder = GetEncoder(ImageFormat.Jpeg);
                    bitmap.Save(memoryStream, jpegEncoder, encoderParameters);

                    // 使用内存流中的JPEG数据创建BitmapSource
                    memoryStream.Position = 0;
                    BitmapDecoder bitmapDecoder = BitmapDecoder.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    bitmapSource = new TransformedBitmap(bitmapDecoder.Frames[0], new ScaleTransform(1, 1));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return bitmapSource;
        }
        /// <summary>
        /// Bitmap---->byte[]
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static byte[] BitmapToBytes(Bitmap bitmap, System.Drawing.Imaging.ImageFormat imageFormat=null)
        {
            MemoryStream ms = null;
            try
            {
                if(imageFormat== null)
                {
                    imageFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                }
                ms = new MemoryStream();
                bitmap.Save(ms,imageFormat);
                byte[] byteImage = new byte[ms.Length];
                byteImage = ms.ToArray();
                return byteImage;
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            finally
            {
                ms.Close();
            }
        }
        public static Bitmap BitmapSourceToBitmap(BitmapSource m)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(m.PixelWidth, m.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(
            new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            m.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride); 
            bmp.UnlockBits(data);
            return bmp;
        }
        public static byte[] BitmapToBytesByJpg(Bitmap bitmap)
        {
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream();
                ImageCodecInfo myImageCodecInfo;
                System.Drawing.Imaging.Encoder myEncoder;
                EncoderParameter myEncoderParameter;
                EncoderParameters myEncoderParameters;
                myImageCodecInfo = GetEncoderInfo("image/jpeg");
                myEncoder = System.Drawing.Imaging.Encoder.Quality;
                myEncoderParameters = new EncoderParameters(1);
                myEncoderParameter = new EncoderParameter(myEncoder, 80L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                bitmap.Save(ms, myImageCodecInfo,myEncoderParameters);
                byte[] byteImage = new byte[ms.Length];
                byteImage = ms.ToArray();
                return byteImage;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally { ms.Close(); ms.Dispose(); }
        }
        public static ImageCodecInfo GetEncoderInfo(String mimeType)
 
         {
             int j;
             ImageCodecInfo[] encoders;
             encoders = ImageCodecInfo.GetImageEncoders();
             for (j = 0; j < encoders.Length; ++j)
             {
                 if (encoders[j].MimeType == mimeType)
                     return encoders[j];
             }
             return null;
         }
        public static byte[] BitmapToBytes(Bitmap bmp, System.Drawing.Imaging.PixelFormat format)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            byte[] bits = null;
            try
            {
                BitmapData bmpdata = bmp.LockBits(rect, ImageLockMode.ReadWrite, format);
                bits = new byte[bmpdata.Stride * bmpdata.Height];
                System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, bits, 0, bits.Length);
                bmp.UnlockBits(bmpdata);
            }
            catch
            {
                return null;
            }
            return bits;
        }
        /// <summary>
        /// BitmapImage---->Bitmap
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        public static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage, bool isPng = false)
        {
            try
            {
                Bitmap bitmap;
                using (MemoryStream outStream = new MemoryStream())
                {
                    BitmapEncoder enc = new BmpBitmapEncoder();
                    if (isPng)
                    {
                        enc = new PngBitmapEncoder();
                    }
                    enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                    enc.Save(outStream);
                    bitmap = new Bitmap(outStream);
                    return bitmap;
                }
            }
            catch (Exception ex) { throw ex; }
        }
        /// <summary>
        /// BitmapImage---->byte[]
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        public static byte[] BitmapImageToBytes(BitmapImage bitmapImage)
        {
            byte[] ByteArray = null;

            try
            {
                Stream stream = bitmapImage.StreamSource;
                if (stream != null && stream.Length > 0)
                {
                    stream.Position = 0;
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        ByteArray = br.ReadBytes((int)stream.Length);
                    }
                }
            }
            catch (Exception ex) { throw ex; }

            return ByteArray;
        }
        /// <summary>
        /// ImageSource---->Bitmap
        /// </summary>
        /// <param name="imageSource"></param>
        /// <returns></returns>
        public static Bitmap ImageSourceToBitmap(ImageSource imageSource)
        {
            try
            {
                BitmapSource m = (BitmapSource)imageSource;
                Bitmap bmp = new Bitmap(m.PixelWidth, m.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                System.Drawing.Imaging.BitmapData data = bmp.LockBits(
                new Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                m.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride); bmp.UnlockBits(data);
                bmp.UnlockBits(data);
                return bmp;
            }
            catch (Exception ex) { throw ex; }
        }
        /// <summary>
        /// byte[]---->Bitmap
        /// </summary>
        /// <param name="Bytes"></param>
        /// <returns></returns>
        public static Bitmap BytesToBitmap(byte[] Bytes)
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(Bytes);
                return new Bitmap(stream);
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
        /// <summary>
        /// 裁剪图像
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <param name="cut"></param>
        /// <returns></returns>
        public static BitmapSource CutImage(BitmapSource bitmapSource, Int32Rect cut)
        {
            try
            {
                return new CroppedBitmap(bitmapSource, cut);
                ////计算Stride
                //var stride = bitmapSource.Format.BitsPerPixel * cut.Width / 8;
                ////声明字节数组
                //byte[] data = new byte[cut.Height * stride];
                ////调用CopyPixels
                //bitmapSource.CopyPixels(cut, data, stride, 0);
                //return BitmapSource.Create(cut.Width, cut.Height, 0, 0, bitmapSource.Format, null, data, stride);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 裁剪图像
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static Bitmap CutImage(Bitmap src, Int32Rect cut)
        {
            try
            {
                Rectangle cropRect = new Rectangle(cut.X, cut.Y, cut.Width, cut.Height);
                Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                          cropRect,
                          GraphicsUnit.Pixel);
                }
                return target;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool CropImage(Bitmap sourceFile, string targetFile, int x, int y, int width, int height)
        {
            bool success = false;
            try
            {
                // 创建目标 Bitmap 对象
                using (Bitmap targetImage = new Bitmap(width, height))
                {
                    // 创建 Graphics 对象并设置裁剪区域
                    using (Graphics graphics = Graphics.FromImage(targetImage))
                    {
                        graphics.DrawImage(sourceFile, new Rectangle(0, 0, width, height), x, y, width, height, GraphicsUnit.Pixel);
                        //graphics.DrawImage(sourceFile, new Rectangle(0, 0, width, height), new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
                    }
                    targetImage.Save(targetFile, ImageFormat.Jpeg);
                    // 保存目标图像到文件
                    success = true;
                }

            }
            catch (Exception ex)
            {
                success = false;
                throw ex;
            }
            return success;

        }
        public static bool CropImageAndSaveAsGrayscale(string sourceFile, string targetFile, int x, int y, int width, int height)
        {
            bool success = false;

            try
            {
                // 创建源 Bitmap 对象
                using (Bitmap sourceImage = new Bitmap(sourceFile))
                {
                    // 创建目标 Bitmap 对象，并设置裁剪区域
                    using (Bitmap targetImage = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed))
                    {
                        // 创建 ColorPalette，并设置为灰度颜色
                        ColorPalette grayPalette = targetImage.Palette;
                        for (int i = 0; i < 256; i++)
                        {
                            grayPalette.Entries[i] = System.Drawing.Color.FromArgb(255, i, i, i);
                        }
                        targetImage.Palette = grayPalette;

                        // 使用 Graphics 类的 DrawImage 方法从源图像中裁剪出一个矩形，并绘制到目标图像中去
                        using (Graphics graphics = Graphics.FromImage(targetImage))
                        {
                            graphics.DrawImage(sourceImage, new Rectangle(0, 0, width, height), x, y, width, height, GraphicsUnit.Pixel);

                            // 将目标图像转换为灰度图像
                            ColorMatrix colorMatrix = new ColorMatrix(new float[][]{
                        new float[] {0.299f, 0.299f, 0.299f, 0, 0},
                        new float[] {0.587f, 0.587f, 0.587f, 0, 0},
                        new float[] {0.114f, 0.114f, 0.114f, 0, 0},
                        new float[] {0,      0,      0,      1, 0},
                        new float[] {0,      0,      0,      0, 1}
                    });
                            using (ImageAttributes attributes = new ImageAttributes())
                            {
                                attributes.SetColorMatrix(colorMatrix);
                                graphics.DrawImage(targetImage, new Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel, attributes);
                            }
                        }

                        // 保存目标图像到文件
                        targetImage.Save(targetFile, System.Drawing.Imaging.ImageFormat.Jpeg);
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                success = false;
            }

            return success;
        }


        /// <summary>
        /// filepath---->bitmapImage
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <param name="decodePixelHeight">大于0压缩生效，默认0（不压缩）</param>
        /// <returns></returns>
        public static BitmapImage GetBitmapImage(string imagePath, int decodePixelHeight = 0)
        {
            if (File.Exists(imagePath))
            {
                try
                {
                    BitmapImage bim;
                    byte[] buf = null;
                    using (BinaryReader loader = new BinaryReader(File.Open(imagePath, FileMode.Open)))
                    {
                        FileInfo fd = new FileInfo(imagePath);
                        int Length = (int)fd.Length;
                        buf = new byte[Length];
                        buf = loader.ReadBytes((int)fd.Length);
                        loader.Close();
                        //开始加载图像
                        bim = new BitmapImage();
                        bim.BeginInit();
                        bim.CacheOption = BitmapCacheOption.OnLoad;
                        if (decodePixelHeight > 0)
                        {
                            bim.DecodePixelHeight = decodePixelHeight;
                        }
                        bim.StreamSource = new MemoryStream(buf);
                        bim.EndInit();
                        bim.Freeze();
                        loader.Dispose();
                    }
                    return bim;

                }
                catch (Exception ex) { throw ex; }
            }
            return null;
        }
        /// <summary>
        /// filpath--->bitmapsource
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public static BitmapSource GetBitmapSource(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                try
                {
                    byte[] buffer = File.ReadAllBytes(imagePath);
                    var ms = new System.IO.MemoryStream(buffer);
                    var bmp = new System.Drawing.Bitmap(ms);
                    var source = BitmapToBitmapSource(bmp);
                    ms.Close();
                    ms.Dispose();
                    bmp.Dispose();
                    return source;
                }
                catch (Exception ex) { throw ex; }
            }
            return null;
        }
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="inputBitmapSource"></param>
        /// <param name="filePath"></param>
        public static void SaveImage(BitmapSource inputBitmapSource, string filePath, int qualityLevel = 75)
        {
            try
            {
                if (qualityLevel < 1)
                {
                    qualityLevel = 1;
                }
                else if (qualityLevel > 100)
                {
                    qualityLevel = 100;
                }
                BitmapEncoder encoder = null;
                var extName = System.IO.Path.GetExtension(filePath).ToLower();
                switch (extName)
                {
                    case ".png":
                        {
                            encoder = new PngBitmapEncoder();
                        }
                        break;
                    case ".tiff":
                        {
                            encoder = new TiffBitmapEncoder { Compression = TiffCompressOption.Lzw };
                        }
                        break;
                    case ".jpg":
                        {
                            encoder = new JpegBitmapEncoder { QualityLevel = qualityLevel };
                        }
                        break;
                    case ".bmp": encoder = new BmpBitmapEncoder(); break;
                    default:
                        break;
                }
                using (FileStream stream = new FileStream(filePath, FileMode.Create))

                {
                    encoder.Frames.Add(BitmapFrame.Create(inputBitmapSource));
                    encoder.Save(stream);
                    encoder = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void SaveImage(Bitmap img, string filePath, long quality = 75L)
        {
            Bitmap bitmap = (Bitmap)img.Clone();
            try
            {
                if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    SaveGrayImage_Old(bitmap, filePath, (int)quality);
                }
                else
                {
                    SaveRGBImage(bitmap, filePath, quality);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                bitmap.Dispose();
            }
        }
        public static void SaveRGBImage(Bitmap bitmap, string filePath, long quality = 75L)
        {
            try
            {
                bitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                var extName = System.IO.Path.GetExtension(filePath).ToLower();
                switch (extName)
                {
                    case ".png":
                        {
                            EncoderParameters encoderParams = new EncoderParameters(1);
                            // 设置压缩参数
                            encoderParams.Param[0] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionLZW);
                            bitmap.Save(filePath, GetEncoder(ImageFormat.Png), encoderParams);
                        }
                        break;
                    case ".jpg":
                        {
                            EncoderParameters encoderParams = new EncoderParameters(1);
                            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                            bitmap.Save(filePath, GetEncoder(ImageFormat.Jpeg), encoderParams);
                        }; break;
                    case ".tiff":
                        {
                            EncoderParameters encoderParams = new EncoderParameters(1);
                            // 设置压缩参数
                            encoderParams.Param[0] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionLZW);
                            // 将图像保存为 TIFF 格式
                            bitmap.Save(filePath, GetEncoder(ImageFormat.Tiff), encoderParams);
                        }
                        break;
                    case ".bmp":
                        {
                            bitmap.Save(filePath, ImageFormat.Bmp);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex) { throw ex; }
        }
        private static System.Drawing.Imaging.ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format)
        {
            System.Drawing.Imaging.ImageCodecInfo[] codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();

            foreach (System.Drawing.Imaging.ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }
        public static void SaveGrayImage(Bitmap bitmap, string filePath, int qualityLevel = 75)
        {
            try
            {
                if (qualityLevel < 1)
                {
                    qualityLevel = 1;
                }
                else if (qualityLevel > 100)
                {
                    qualityLevel = 100;
                }
                // 将图片格式设置为 Format8bppIndexed
                bitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                //设置调色板，将其设为灰度调色板
                ColorPalette palette = bitmap.Palette;
                for (int i = 0; i < palette.Entries.Length; i++)
                {
                    palette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
                }
                bitmap.Palette = palette;
                var extName = System.IO.Path.GetExtension(filePath).ToLower();
                //using (Graphics g = Graphics.FromImage(bitmap))
                //{
                //    g.DrawImage(bitmap, 0, 0);
                //}
                switch (extName)
                {
                    case ".png":
                        {
                            EncoderParameters encoderParams = new EncoderParameters(1);
                            // 设置压缩参数
                            encoderParams.Param[0] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionLZW);
                            bitmap.Save(filePath, GetEncoder(ImageFormat.Png), encoderParams);
                        }
                        break;
                    case ".jpg":
                        {
                            EncoderParameters encoderParams = new EncoderParameters(1);
                            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, qualityLevel);
                            bitmap.Save(filePath, GetEncoder(ImageFormat.Jpeg), encoderParams);
                        };
                        break;
                    case ".tiff":
                        {
                            EncoderParameters encoderParams = new EncoderParameters(1);
                            encoderParams.Param[0] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionLZW);
                            bitmap.Save(filePath, GetEncoder(ImageFormat.Tiff), encoderParams);
                        }
                        break;
                    case ".bmp":
                        {
                            bitmap.Save(filePath, ImageFormat.Bmp);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex) { throw ex; }
        }
        /// <summary>
        /// 保存单通道图片
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="filePath"></param>
        public static void SaveGrayImage_Old(Bitmap bitmap, string filePath, int qualityLevel = 75)
        {
            try
            {
                if (qualityLevel < 1)
                {
                    qualityLevel = 1;
                }
                else if (qualityLevel > 100)
                {
                    qualityLevel = 100;
                }
                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
                System.Windows.Media.PixelFormat pixelFormats = ConvertBmpPixelFormat(bitmap.PixelFormat);
                BitmapSource source = BitmapSource.Create(bitmap.Width,
                                                          bitmap.Height,
                                                          bitmap.HorizontalResolution,
                                                          bitmap.VerticalResolution,
                                                          pixelFormats,
                                                          null,
                                                          bitmapData.Scan0,
                                                          bitmapData.Stride * bitmap.Height,
                                                          bitmapData.Stride);

                bitmap.UnlockBits(bitmapData);
                //new File Stream
                using (FileStream stream = new FileStream(filePath, FileMode.Create))

                {   //save Tiff Bitmap 
                    BitmapEncoder encoder = null;
                    var extName = System.IO.Path.GetExtension(filePath).ToLower();
                    switch (extName)
                    {
                        case ".png":
                            {
                                encoder = new PngBitmapEncoder();
                            }
                            break;
                        case ".tiff":
                            {
                                encoder = new TiffBitmapEncoder { Compression = TiffCompressOption.Lzw };
                            }
                            break;
                        case ".jpg":
                            {
                                encoder = new JpegBitmapEncoder { QualityLevel = qualityLevel };
                            }
                            break;
                        case ".bmp": encoder = new BmpBitmapEncoder(); break;
                        default:
                            break;
                    }
                    encoder.Frames.Add(BitmapFrame.Create(source));
                    encoder.Save(stream);
                    encoder = null;
                    stream.Close();
                    stream.Dispose();
                }
            }
            catch (Exception ex) { throw ex; }
        }
        /// <summary>
        /// 三通道转换单通道
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap TransForm24to8(Bitmap bmp)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                IntPtr ptr = bmpData.Scan0;
                int bytes = bmpData.Stride * bmpData.Height;
                byte[] rgbValues = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                Rectangle rect2 = new Rectangle(0, 0, bmp.Width, bmp.Height);
                Bitmap RetBmp = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                System.Drawing.Imaging.BitmapData bmpData2 = RetBmp.LockBits(rect2, System.Drawing.Imaging.ImageLockMode.ReadWrite, RetBmp.PixelFormat);
                IntPtr ptr2 = bmpData2.Scan0;
                int bytes2 = bmpData2.Stride * bmpData2.Height;
                byte[] rgbValues2 = new byte[bytes2];
                System.Runtime.InteropServices.Marshal.Copy(ptr2, rgbValues2, 0, bytes2);
                double colorTemp = 0;
                for (int i = 0; i < bmpData.Height; i++)
                {
                    for (int j = 0; j < bmpData.Width * 3; j += 3)
                    {
                        colorTemp = rgbValues[i * bmpData.Stride + j + 2] * 0.299 + rgbValues[i * bmpData.Stride + j + 1] * 0.578 + rgbValues[i * bmpData.Stride + j] * 0.114;
                        rgbValues2[i * bmpData2.Stride + j / 3] = (byte)colorTemp;
                    }
                }
                System.Runtime.InteropServices.Marshal.Copy(rgbValues2, 0, ptr2, bytes2);
                bmp.UnlockBits(bmpData);
                ColorPalette tempPalette = RetBmp.Palette;
                for (int i = 0; i < 256; i++)
                {
                    tempPalette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
                }
                RetBmp.Palette = tempPalette;
                RetBmp.UnlockBits(bmpData2);
                return RetBmp;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// 单通道转换三通道
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap TransForm8to24(Bitmap bmp)
        {
            try
            {
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);

                System.Drawing.Imaging.BitmapData bitmapData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

                //计算实际8位图容量
                int size8 = bitmapData.Stride * bmp.Height;
                byte[] grayValues = new byte[size8];

                //// 申请目标位图的变量，并将其内存区域锁定  
                Bitmap RetBmp = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                BitmapData TempBmpData = RetBmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);


                //// 获取图像参数以及设置24位图信息 
                int stride = TempBmpData.Stride;  // 扫描线的宽度  
                int offset = stride - RetBmp.Width;  // 显示宽度与扫描线宽度的间隙  
                IntPtr iptr = TempBmpData.Scan0;  // 获取bmpData的内存起始位置  
                int scanBytes = stride * RetBmp.Height;// 用stride宽度，表示这是内存区域的大小  

                //// 下面把原始的显示大小字节数组转换为内存中实际存放的字节数组  

                byte[] pixelValues = new byte[scanBytes];  //为目标数组分配内存  
                System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, grayValues, 0, size8);
                for (int i = 0; i < bmp.Height; i++)
                {

                    for (int j = 0; j < bitmapData.Stride; j++)
                    {

                        if (j >= bmp.Width)
                            continue;


                        int indexSrc = i * bitmapData.Stride + j;
                        int realIndex = i * TempBmpData.Stride + j * 3;

                        pixelValues[realIndex] = grayValues[indexSrc];
                        pixelValues[realIndex + 1] = grayValues[indexSrc];
                        pixelValues[realIndex + 2] = grayValues[indexSrc];

                    }

                }
                System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, iptr, scanBytes);
                RetBmp.UnlockBits(TempBmpData);
                bmp.UnlockBits(bitmapData);
                return RetBmp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Convert Bmp Pixel Format to Media Pixel Format
        public static System.Windows.Media.PixelFormat ConvertBmpPixelFormat(System.Drawing.Imaging.PixelFormat pixelformat)
        {
            System.Windows.Media.PixelFormat pixelFormats = PixelFormats.Default;

            switch (pixelformat)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    pixelFormats = PixelFormats.Bgra32;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    pixelFormats = PixelFormats.Bgr32;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                    pixelFormats = PixelFormats.Rgba64;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    pixelFormats = PixelFormats.Bgr24;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format48bppRgb:
                    pixelFormats = PixelFormats.Rgb48;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    pixelFormats = PixelFormats.Gray8;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                    pixelFormats = PixelFormats.Gray16;
                    break;
            }
            return pixelFormats;
        }
        public static TransformedBitmap GetTransformedBitmap(BitmapSource bitmapSource, double angle)
        {
            try
            {
                if (bitmapSource is null)
                {
                    return null;
                }

                TransformedBitmap tb = new TransformedBitmap();
                tb.BeginInit();
                tb.Source = bitmapSource;
                RotateTransform transform = new RotateTransform(angle);
                tb.Transform = transform;
                tb.EndInit();
                return tb;
            }
            catch (Exception ex) { throw ex; }
        }
        /// <summary>
        /// 图片文件base64序列化
        /// </summary>
        /// <param name="imgpath">图片路径</param>
        /// <returns></returns>
        public static string ImgToBase64StringFromFile(string imgpath, long quality = 75L)
        {
            return ImgToBase64String(new Bitmap(imgpath), quality);
        }
        public static string ImgToBase64String(Bitmap bmp, long quality = 75L)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    System.Drawing.Imaging.ImageCodecInfo jpgEncoder = GetEncoder(System.Drawing.Imaging.ImageFormat.Jpeg);
                    System.Drawing.Imaging.Encoder myEncoder =
                        System.Drawing.Imaging.Encoder.Quality;
                    System.Drawing.Imaging.EncoderParameters myEncoderParameters = new System.Drawing.Imaging.EncoderParameters(1);

                    System.Drawing.Imaging.EncoderParameter myEncoderParameter = new System.Drawing.Imaging.EncoderParameter(myEncoder, quality);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    bmp.Save(ms, jpgEncoder, myEncoderParameters);
                    byte[] arr = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr, 0, (int)ms.Length);
                    ms.Close();
                    string pic = Convert.ToBase64String(arr);
                    return pic;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 根据角度旋转位图
        /// </summary>
        /// <param name="bitmap"></param>
        public static Bitmap BitmapRotate(Bitmap bitmap, float angle)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            System.Drawing.Drawing2D.Matrix mtrx = new System.Drawing.Drawing2D.Matrix();
            mtrx.RotateAt(angle, new PointF((width / 2), (height / 2)), MatrixOrder.Append);
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, width, height));
            RectangleF rct = path.GetBounds(mtrx);
            Bitmap devImage = new Bitmap((int)(rct.Width), (int)(rct.Height));
            Graphics g = Graphics.FromImage(devImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            System.Drawing.Point Offset = new System.Drawing.Point((int)(rct.Width - width) / 2, (int)(rct.Height - height) / 2);
            Rectangle rect = new Rectangle(Offset.X, Offset.Y, (int)width, (int)height);
            System.Drawing.Point center = new System.Drawing.Point((int)(rect.X + rect.Width / 2), (int)(rect.Y + rect.Height / 2));
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(angle);
            g.TranslateTransform(-center.X, -center.Y);
            g.DrawImage(bitmap, rect);
            g.ResetTransform();
            g.Save();
            g.Dispose();
            path.Dispose();
            return devImage;
        }
        public static Bitmap CombineImage(Bitmap bitmap1, Bitmap bitmap2)
        {
            try
            {
                var width = Math.Max(bitmap1.Width, bitmap2.Width);
                var height = bitmap1.Height + bitmap2.Height;
                Bitmap bitMap = new Bitmap(width, height);
                Graphics g1 = Graphics.FromImage(bitMap);
                g1.FillRectangle(System.Drawing.Brushes.White, new Rectangle(0, 0, width, height));
                g1.DrawImage(bitmap1, 0, 0, bitmap1.Width, bitmap1.Height);
                g1.DrawImage(bitmap2, 0, bitmap1.Height, bitmap2.Width, bitmap2.Height);
                bitmap1.Dispose();
                bitmap2.Dispose();
                return bitMap;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static Bitmap CombineImage(IEnumerable<Bitmap> bitmaps)
        {
            try
            {
                var width = bitmaps.Max(a => a.Width);
                var height = bitmaps.Sum(a => a.Height);
                Bitmap bitmap = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(bitmap);
                g.FillRectangle(System.Drawing.Brushes.White, new Rectangle(0, 0, width, height));
                int currentHeight = 0;
                int bitmapCount = bitmaps.Count();
                for (int i = 0; i < bitmapCount; i++)
                {
                    g.DrawImage(bitmaps.ElementAt(i), 0, currentHeight, bitmaps.ElementAt(i).Width, bitmaps.ElementAt(i).Height);
                    currentHeight += bitmaps.ElementAt(i).Height;
                    bitmaps.ElementAt(i).Dispose();
                }
                //if (bitmaps.ElementAt(0).PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                //{
                //    bitmap = TransForm24to8(bitmap);
                //}
                return bitmap;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /**
         * 内存法复制图片
         * */
        public static Bitmap CopyBitmap(Bitmap bmpSrc)
        {
            Bitmap dstBitmap = null;
            using (MemoryStream mStream = new MemoryStream())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bf.Serialize(mStream, bmpSrc);
                mStream.Seek(0, SeekOrigin.Begin);
                dstBitmap = (Bitmap)bf.Deserialize(mStream);
                mStream.Close();
            }
            return dstBitmap;
        }

    }
}
