using HYAutomation.Core.Algorithm;
using HYAutomation.Core.Algorithm.Models;
using HYAutomation.Entity_Base;
using HYCommonUtils.SerializationUtils;
using HYWindowUtils.WPF.VMBaseUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HYAutomation.Core.Views.Models
{
    public class ProductInfoModel : NotifyPropertyObject
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
        private string _barcode;
        /// <summary>
        /// 条码
        /// </summary>
        public string Barcode
        {
            get { return _barcode; }
            set { _barcode = value; RaisePropertyChanged(); }
        }
        private string _typeCode;
        /// <summary>
        /// 型号编码
        /// </summary>
        public string TypeCode
        {
            get { return _typeCode; }
            set { _typeCode = value; RaisePropertyChanged(); }
        }
        private string _typeName;
        /// <summary>
        /// 型号名称
        /// </summary>
        public string TypeName
        {
            get { return _typeName; }
            set { _typeName = value; RaisePropertyChanged(); }
        }
        private IList<CameraResultModel> _cameraDatas;
        /// <summary>
        /// 相机数据
        /// </summary>
        public IList<CameraResultModel> CameraDatas
        {
            get { return _cameraDatas; }
            set { _cameraDatas = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 相机数据
        /// </summary>
        private string _cameraImageInfos = null;
        public string CameraImageInfos {
            get { return _cameraImageInfos; }
            set { _cameraImageInfos = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 结果是否合格
        /// </summary>
        private bool? _isOK = null;
        /// <summary>
        /// 检测结果
        /// </summary>
        public bool? IsOK
        {
            get { return _isOK; }
            set { _isOK = value; RaisePropertyChanged(); }
        }
        public StringBuilder TempLog { get; } = new StringBuilder();
        /// <summary>
        /// 工作日志记录
        /// </summary>
        private string _workLog;

        public string WorkLog
        {
            get { return _workLog; }
            set { _workLog = value; RaisePropertyChanged(); }
        }
        public  ProductInfoModel ConvertFromNoDatas(ProductInfo productInfo)
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
                //CameraDatas = GetCameraResults(productInfo.CameraImageInfos),
                CameraImageInfos=productInfo.CameraImageInfos,
                WorkLog = productInfo.Note
            };
            return data;
        }

        public virtual ProductInfoModel ConvertFrom(ProductInfo productInfo)
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
        public virtual ProductInfo ConvertTo()
        {
            ProductInfo productInfo = new ProductInfo
            {
                CreateTime = CreateTime,
                Barcode = Barcode,
                TypeCode = TypeCode,
                TypeName = TypeName,
                IsOK = IsOK == true,
                Note = WorkLog,
                CameraImageInfos = GetProductResultDetailsJson(this)
            };
            return productInfo;
        }
        public static IList<CameraResultModel> GetCameraResults(string productResultDetailsJson)
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
                                data.DetectItemConfig = AlgorithmConfig.Instance.DetectItemConfigs?.FirstOrDefault(a => a.DetectItemName == item.DetectItemName);
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
        protected string GetProductResultDetailsJson(ProductInfoModel productinfoModel)
        {
            string productResultDetailsJson = string.Empty;
            try
            {
                if (productinfoModel?.CameraDatas == null)
                {
                    return productResultDetailsJson;
                }
                List<ProductResultDetails> ls = new List<ProductResultDetails>();
                foreach (var item in productinfoModel.CameraDatas)
                {
                    if (item != null)
                    {
                        ProductResultDetails prd = new ProductResultDetails { CameraName = item.CameraName, CameraDesc = item.CameraDesc, ImagePath = item.CameraImagePath, Result = item.CameraRetIsOK == true };
                        prd.DetailResults = new List<ProductResultDetails.DetailResult>();
                        foreach (var item1 in item.DetectItems)
                        {
                            
                            ((List<ProductResultDetails.DetailResult>)prd.DetailResults).Add(new ProductResultDetails.DetailResult { DetectItemName = item1.DetectItemConfig.DetectItemName, AlgorithmType = item1.DetectItemConfig.AlgorithmConfig.AlgorithmType, RetIsOk = item1.RetIsOk == true, RetStr = item1.Result });
                        }
                        ls.Add(prd);
                    }
                    else
                    {
                        ls.Add(null);
                    }
                }
                productResultDetailsJson = HYCommonUtils.SerializationUtils.JsonUtils.JsonSerialize(ls);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString());
            }
            return productResultDetailsJson;

        }

    }
    public class ProductInfoModel<T> : ProductInfoModel
    {
        private T _data;

        public T Data
        {
            get { return _data; }
            set { _data = value; RaisePropertyChanged(); }
        }
    }
    public class ProductResultDetails
    {
        public string CameraName { get; set; }
        public string CameraDesc { get; set; }
        public string ImagePath { get; set; }
        public bool Result { get; set; }
        public IEnumerable<DetailResult> DetailResults { get; set; }
        public class DetailResult
        {
            public string DetectItemName { get; set; }
            public AlgorithmTypeEnum AlgorithmType { get; set; }
            public bool RetIsOk { get; set; }
            public string RetStr { get; set; }
        }
    }
}
