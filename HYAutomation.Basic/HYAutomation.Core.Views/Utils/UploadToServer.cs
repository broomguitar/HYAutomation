using HYCommonUtils.SerializationUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HYAutomation.Core.Views.Utils
{
    public class UploadToServer
    {
        private static readonly string apiUrl = GlobalConfig.ConfigManager.GetValue("RepositoryAddr");
        private static readonly string baseDir = GlobalConfig.ConfigManager.GetValue("CacheFolder");
        private static readonly string bucketUnique = GlobalConfig.ConfigManager.GetValue("BucketUnique");
        private static readonly string projectId = GlobalConfig.ConfigManager.GetValue("ProjectId");
        private static readonly string deviceId = GlobalConfig.ConfigManager.GetValue("DeviceId");
        static bool isUploading = false;
        private static Task work;
        private static bool isWorking;
        public static void Work()
        {
            isWorking = true;
            work = new Task(() =>
            {
                while (isWorking)
                {
                    UploadToRemote();
                    System.Threading.Thread.Sleep(100);
                }
            }, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() =>
            {
                while (isWorking)
                {
                    System.Threading.Thread.Sleep(1000);
                    if (isWorking && work.Status != TaskStatus.Running)
                    {
                        work.Start();
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }
        public static void StopWork()
        {
            isWorking = false;
            int times = 5;
            while (isUploading && times > 0)
            {
                System.Threading.Thread.Sleep(100);
                times--;
            }
        }
        private static void UploadToRemote()
        {
            isUploading = true;
            try
            {
                if (Directory.Exists(baseDir))
                {
                    string uploadDir = GetEarlistDirectory(baseDir);
                    if (GetDirSize(uploadDir) > 0)
                    {
                        string barcode = Path.GetFileNameWithoutExtension(uploadDir);

                        string retfile = Directory.GetFiles(uploadDir, "*.data").FirstOrDefault();
                        bool b = true;
                        if (!File.Exists(retfile))
                        {
                            return;
                        }
                        var strs = File.ReadAllLines(retfile);
                        string productJson = strs.FirstOrDefault(a => a.Contains(barcode));
                        if (productJson == null)
                        {
                            return;
                        }
                        var product = JsonUtils.JsonDeserialize<Models.ProductInfoModel>(productJson);

                        foreach (var item in product.CameraDatas)
                        {
                            string imgFile = item.CameraImagePath;
                            if (string.IsNullOrEmpty(imgFile))
                            {
                                continue;
                            }
                            Dictionary<string, object> dicParameters = new Dictionary<string, object>();
                            dicParameters.Add("bucketUnique", bucketUnique);
                            dicParameters.Add("projectid", projectId);
                            dicParameters.Add("deviceid", deviceId);
                            dicParameters.Add("barcode", product.Barcode);
                            dicParameters.Add("img", Core.ImageHelper.ImgToBase64StringFromFile(imgFile, GlobalConfig.Instance.JpegQuality));
                            dicParameters.Add("result", product.IsOK == true ? 1 : 2);
                            dicParameters.Add("dataTime", product.CreateTime.ToString("yyyy-MM-dd HH-mm-ss"));
                            dicParameters.Add("module", "Left");
                            dicParameters.Add("product", "洗衣机");
                            dicParameters.Add("type", product.TypeCode);
                            dicParameters.Add("log", File.ReadAllText(retfile).Replace(productJson, string.Empty));
                            dicParameters.Add("total", product.CameraDatas.Count());
                            dicParameters.Add("fileSuffix", Path.GetExtension(imgFile));
                            string paramData = JsonUtils.JsonSerialize(dicParameters);
                            string ret = Core.Web.WebApiHelper.HttpFormPost(apiUrl, paramData);
                            if (!string.IsNullOrEmpty(ret))
                            {
                                var retData = JsonUtils.JsonDeserialize<Core.Web.WebApiDataAccess_Upload<string>>(ret);
                                if (retData.code != 0 || !retData.success)
                                {
                                    b &= false;
                                }
                            }
                        }
                        if (b)
                        {
                            Directory.Delete(uploadDir, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString());
            }
            finally
            {
                isUploading = false;
            }
        }
        public static void UploadToRemote(Models.ProductInfoModel product)
        {
            if (product == null)
            {
                return;
            }
            bool b = true;
            foreach (var item in product.CameraDatas)
            {
                string imgFile = item.CameraImagePath;
                if (string.IsNullOrEmpty(imgFile))
                {
                    continue;
                }
                Dictionary<string, object> dicParameters = new Dictionary<string, object>();
                dicParameters.Add("bucketUnique", bucketUnique);
                dicParameters.Add("projectid", projectId);
                dicParameters.Add("deviceid", deviceId);
                dicParameters.Add("barcode", product.Barcode);
                dicParameters.Add("img", Core.ImageHelper.ImgToBase64StringFromFile(imgFile, GlobalConfig.Instance.JpegQuality));
                dicParameters.Add("result", product.IsOK == true ? 1 : 2);
                dicParameters.Add("dataTime", product.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                dicParameters.Add("module", item.CameraName);
                dicParameters.Add("product", "洗衣机");
                dicParameters.Add("type", product.TypeCode);
                dicParameters.Add("log", product.WorkLog);
                dicParameters.Add("total", product.CameraDatas.Count());
                dicParameters.Add("fileSuffix", Path.GetExtension(imgFile));
                string paramData = JsonUtils.JsonSerialize(dicParameters);
                string ret = Web.WebApiHelper.HttpJsonPost(apiUrl, paramData);
                if (!string.IsNullOrEmpty(ret))
                {
                    var retData = JsonUtils.JsonDeserialize<Web.WebApiDataAccess_Upload<string>>(ret);
                    if (retData.code != 0 || !retData.success)
                    {
                        b &= false;
                    }
                }
            }
            if (b)
            {
                try
                {
                    //Directory.Delete(uploadDir, true);
                }
                catch
                {

                }
            }

        }
        /// <summary>
        /// 获取最早文件夹
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static string GetEarlistDirectory(string dir)
        {
            if (string.IsNullOrEmpty(dir))
            {
                return string.Empty;
            }
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            var dirs = dirInfo.GetDirectories();
            if (dirs.Length < 1)
            {
                return dir;
            }
            else
            {
                var data = dirs.OrderByDescending(a => a.CreationTime).FirstOrDefault();
                return GetEarlistDirectory(data.FullName);
            }
        }
        /// <summary>
        /// 获取文件夹大小
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static long GetDirSize(string dir)
        {
            try
            {
                long dirSize = 0;
                DirectoryInfo dirInfo = new DirectoryInfo(dir);

                DirectoryInfo[] dirs = dirInfo.GetDirectories();
                FileInfo[] files = dirInfo.GetFiles();

                foreach (var item in dirs)
                {
                    dirSize += GetDirSize(item.FullName);
                }

                foreach (var item in files)
                {
                    dirSize += item.Length;
                }
                return dirSize;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString());
                return 0;
            }

        }
    }
}
