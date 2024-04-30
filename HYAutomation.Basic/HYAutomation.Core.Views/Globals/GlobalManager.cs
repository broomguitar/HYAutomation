using HYAutomation.BLL;
using HYAutomation.Core.Algorithm.Models;
using HYAutomation.Core.Views.Extensions;
using HYAutomation.Core.Views.Models;
using HYAutomation.Device;
using HYAutomation.Entity_Base;
using HYWindowUtils.WPF.VMBaseUtil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HYAutomation.Core.Views
{
    public class GlobalManager : NotifyPropertyObject
    {
        protected static readonly object _lockObj = new object();
        protected static GlobalManager _instance;
        public static GlobalManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new GlobalManager();
                        }
                    }
                }
                return _instance;
            }
        }
        protected GlobalManager() { }
        /// <summary>
        /// 所有设备
        /// </summary>
        public IEnumerable<IDevice> Devices { get; set; }
        public static string GetDateFolder(DateTime dateTime)
        {
            string dateFolder = System.IO.Path.Combine(GlobalConfig.Instance.CacheFolder, dateTime.ToString("yyyyMMdd")).Trim();
            if (!System.IO.Directory.Exists(dateFolder))
            {
                System.IO.Directory.CreateDirectory(dateFolder);
            }
            return dateFolder.Trim();
        }
        public static string GetProductFolder(ProductInfoModel productInfo)
        {
            if (string.IsNullOrEmpty(productInfo.Barcode) || string.IsNullOrEmpty(productInfo.TypeCode))
            {
                return string.Empty;
            }
            string dateFolder = System.IO.Path.Combine(GlobalConfig.Instance.CacheFolder, productInfo.CreateTime.ToString("yyyyMMdd")).Trim();
            if (!System.IO.Directory.Exists(dateFolder))
            {
                System.IO.Directory.CreateDirectory(dateFolder);
            }
            string folder = productInfo.TypeCode;
            if (!string.IsNullOrEmpty(productInfo.TypeName))
            {
                folder = Core.FileUtils.FilePathUtil.RemoveChineseCharacter(productInfo.TypeName);
            }
            string typeFolder = System.IO.Path.Combine(dateFolder, folder).Trim();
            if (!System.IO.Directory.Exists(typeFolder))
            {
                System.IO.Directory.CreateDirectory(typeFolder);
            }
            return System.IO.Path.Combine(typeFolder, productInfo.Barcode).Trim();
        }
        /// <summary>
        /// 根据型号获取检测项内容
        /// </summary>
        /// <param name="typecode"></param>
        /// <returns></returns>
        public IEnumerable<DetectItemModel> GetDetectionItems<T>(string typecode) where T : Entity_Base.ProductTypeInfo
        {
            try
            {
                return ProductTypeBLL.GetDetectionItems(typecode).Select(a => new DetectItemModel().ConverFrom<T>(a));
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString());
                return null;
            }
        }
        private int _imgColumns;

        public int ImgColumns
        {
            get { return _imgColumns; }
            set { _imgColumns = value; RaisePropertyChanged(); }
        }

        private int _imgRows;

        public int ImgRows
        {
            get { return _imgRows; }
            set { _imgRows = value; RaisePropertyChanged(); }
        }

        public IProductTypeBLL ProductTypeBLL { get; set; }
    }
}
