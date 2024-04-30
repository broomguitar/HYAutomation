using HYAutomation.Core;
using HYAutomation.Core.Web;
using HYAutomation.Devices.HiEdgeMind;
using HYAutomation.Entity_Base;
using HYCommonUtils.SerializationUtils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HYAutomation.Projects.HiEdgeMind
{
    internal class ServerConnector
    {
        /// <summary>
        /// 设备心跳
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Online(string url)
        {
            try
            {
                return WebApiHelper.HttpGet(url, null, 15000);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// 获取硬件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="machineID"></param>
        /// <returns></returns>
        public static HardwareRoot DownloadHardwares(string url, string machineID)
        {
            try
            {
                Dictionary<string, object> dicdata = new Dictionary<string, object>();
                dicdata.Add("deviceNo", machineID);
                dicdata.Add("fileOrJson", 2);
                string parameterJson = JsonUtils.JsonSerialize(dicdata);
                string ret = WebApiHelper.HttpJsonPost(url, parameterJson, 15000);
                if (!string.IsNullOrEmpty(ret))
                {
                    var retData = JsonUtils.JsonDeserialize<WebApiDataAccess_HiEdgeMind<HardwareRoot>>(ret);
                    if (retData.success)
                    {
                        return retData.data;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// 获取工作流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="machineID"></param>
        /// <returns></returns>
        public static WorkFlowRoot DownloadWorkFlow(string url, string machineID)
        {
            try
            {
                Dictionary<string, object> dicdata = new Dictionary<string, object>();
                dicdata.Add("deviceNo", machineID);
                dicdata.Add("fileOrJson", 2);
                string parameterJson = JsonUtils.JsonSerialize(dicdata);
                string ret = WebApiHelper.HttpJsonPost(url, parameterJson, 15000);
                if (!string.IsNullOrEmpty(ret))
                {
                    var retData = JsonUtils.JsonDeserialize<WebApiDataAccess_HiEdgeMind<WorkFlowRoot>>(ret);
                    if (retData.success)
                    {
                        return retData.data;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// 获取算法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="machineID"></param>
        /// <returns></returns>
        public static List<AlgorithmConfigItem> DownloadAlgorithms(string url, string machineID)
        {
            try
            {
                Dictionary<string, object> dicdata = new Dictionary<string, object>();
                dicdata.Add("deviceNo", machineID);
                dicdata.Add("fileOrJson", 2);
                string parameterJson = JsonUtils.JsonSerialize(dicdata);
                string ret = WebApiHelper.HttpJsonPost(url, parameterJson, 15000);
                if (!string.IsNullOrEmpty(ret))
                {
                    var retData = JsonUtils.JsonDeserialize<WebApiDataAccess_HiEdgeMind<List<AlgorithmConfigItem>>>(ret);
                    if (retData.success)
                    {
                        return retData.data;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// 获取检测项
        /// </summary>
        /// <param name="url"></param>
        /// <param name="machineID"></param>
        /// <returns></returns>
        public static List<CheckItemData> DownloadDetectItems(string url, string machineID)
        {
            try
            {
                Dictionary<string, object> dicdata = new Dictionary<string, object>();
                dicdata.Add("deviceNo", machineID);
                dicdata.Add("fileOrJson", 2);
                string parameterJson = JsonUtils.JsonSerialize(dicdata);
                string ret = WebApiHelper.HttpJsonPost(url, parameterJson, 15000);
                if (!string.IsNullOrEmpty(ret))
                {
                    var retData = JsonUtils.JsonDeserialize<WebApiDataAccess_HiEdgeMind<List<CheckItemData>>>(ret);
                    if (retData.success)
                    {
                        return retData.data;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// 获取型号信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="machineID"></param>
        /// <param name="typecode"></param>
        /// <returns></returns>
        public static ProductTypeData QueryTypeInfo(string url, string machineID, string typecode)
        {
            try
            {
                Dictionary<string, object> dicdata = new Dictionary<string, object>();
                dicdata.Add("deviceNo", machineID);
                dicdata.Add("typeCode", typecode);
                dicdata.Add("fileOrJson", 2);
                string parameterJson = JsonUtils.JsonSerialize(dicdata);
                string ret = WebApiHelper.HttpJsonPost(url, parameterJson, 15000);
                if (!string.IsNullOrEmpty(ret))
                {
                    var retData = JsonUtils.JsonDeserialize<WebApiDataAccess_HiEdgeMind<ProductTypeData>>(ret);
                    if (retData.success)
                    {
                        return retData.data;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="machineID"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool UploadImage(string url, string machineID, string fileName)
        {
            try
            {
                string ret = WebApiHelper.UploadImageFile(url, fileName);
                if (!string.IsNullOrEmpty(ret))
                {
                    var retData = JsonUtils.JsonDeserialize<WebApiDataAccess_HiEdgeMind<string>>(ret);
                    if (retData.success)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.Instance.AddLog(ex.ToString()); ;
            }
            return false;
        }
        public static bool UploadImage(string url, string machineID, Bitmap bitmap, string fileName)
        {
            try
            {
                string ret = WebApiHelper.UploadImageFile($"{url}?deviceNo={machineID}", ImageHelper.BitmapToBytesByJpg(bitmap), fileName);
                if (!string.IsNullOrEmpty(ret))
                {
                    var retData = JsonUtils.JsonDeserialize<WebApiDataAccess_HiEdgeMind<string>>(ret);
                    if (retData.success)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.Instance.AddLog(ex.ToString()); ;
            }
            return false;
        }
        /// <summary>
        /// 上传生产结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="machineID"></param>
        /// <param name="productInfo"></param>
        /// <returns></returns>
        public static bool UploadResult(string url, string machineID, ProductInfo productInfo)
        {
            try
            {
                string productJson = JsonUtils.JsonSerialize(new ProductInfo_HiEdge { Id = productInfo.Id, Barcode = productInfo.Barcode, DeviceNo = machineID, TypeCode = productInfo.TypeCode, TypeName = productInfo.TypeName, CameraImageInfos = productInfo.CameraImageInfos, CreateTime = productInfo.CreateTime, IsOK = productInfo.IsOK, Note = productInfo.Note });
                string ret = WebApiHelper.HttpJsonPost(url, productJson);
                if (!string.IsNullOrEmpty(ret))
                {
                    var retData = JsonUtils.JsonDeserialize<WebApiDataAccess_HiEdgeMind<string>>(ret);
                    if (retData.success)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString()); ;
            }
            return false;
        }
        /// <summary>
        /// 下载模型文件
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool DownLoadModelFile(List<string> urls)
        {
            try
            {
                List<Task> list = new List<Task>();
                foreach (string url in urls) 
                {
                    list.Add(Task.Run(() => { WebApiHelper.HttpDownLoadFile(url); }));
                }
                Task.WaitAll(list.ToArray());
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString());
                return false;
            }
        }
    }
    class ProductInfo_HiEdge
    {
        /// <summary>
        /// 唯一ID值
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 条码数据
        /// </summary>
        public string Barcode { get; set; }
        public string DeviceNo { get; set; }
        /// <summary>
        /// 型号编号
        /// </summary>
        public string TypeCode { get; set; }
        /// <summary>
        /// 型号名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 相机图片信息
        /// </summary>
        public string CameraImageInfos { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 检测结果是否合格
        /// </summary>
        public bool IsOK { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
    }
    public class WebApiDataAccess_HiEdgeMind<T>
    {
        public string errCode { get; set; }
        public string errMessage { get; set; }
        public bool success { get; set; }
        public T data { get; set; }
    }
}
