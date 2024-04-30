using HYAutomation.Core.Algorithm.Models;
using HYAutomation.Entity_Base;
using HYWindowUtils.WPF.VMBaseUtil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace HYAutomation.Core.Views.Models
{
    public class ProductTypeModel : NotifyPropertyObject
    {
        /// <summary>
        /// 型号ID
        /// </summary>
        public int Id { get; set; }
        private string _typeCode;
        /// <summary>
        /// 型号编号
        /// </summary>
        public string TypeCode
        {
            get { return _typeCode; }
            set { _typeCode = value; RaisePropertyChanged(); }
        }
        /// 型号名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 缩略图 
        /// </summary>
        //public BitmapImage Thumbnail
        //{
        //    get; set;
        //}
        /// <summary>
        /// 检测详细
        /// </summary>
        public List<ProductTypeDetailModel> Details { get; set; } = new List<ProductTypeDetailModel>();
        public virtual ProductTypeModel ConvertFrom(ProductTypeInfo productTypeInfo)
        {
            if (productTypeInfo == null)
            {
                return null;
            }
            var data = HYCommonUtils.SerializationUtils.JsonUtils.JsonDeserialize<KeyValuePair<string, Dictionary<string, Dictionary<string, string>>>>(productTypeInfo.AlgorithmUtilsContent);
            string cameraConfigs = data.Key;
            Dictionary<KeyValuePair<string, bool>, Dictionary<string, string>> keyValuePairs = new Dictionary<KeyValuePair<string, bool>, Dictionary<string, string>>();

            foreach (var item in data.Value)
            {
                try
                {
                    keyValuePairs.Add(HYCommonUtils.SerializationUtils.JsonUtils.JsonDeserialize<KeyValuePair<string, bool>>(item.Key), item.Value);
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.AddLog("ConvertFrom转换失败" + ex.ToString());
                    keyValuePairs.Add(new KeyValuePair<string, bool>(item.Key, true), item.Value);
                }
            }
            //Parallel.ForEach(data.Value, item =>
            //{
            //    try
            //    {
            //        keyValuePairs.Add(HYCommonUtils.SerializationUtils.JsonUtils.JsonDeserialize<KeyValuePair<string, bool>>(item.Key), item.Value);
            //    }
            //    catch(Exception ex) {
            //        LogHelper.Instance.AddLog("ConvertFrom转换失败" + ex.ToString()) ;
            //        keyValuePairs.Add(new KeyValuePair<string, bool>(item.Key, true), item.Value);
            //    }
            //});
            var details = HYCommonUtils.SerializationUtils.JsonUtils.JsonDeserialize<List<ProductTypeDetailModel>>(cameraConfigs);
            var TypeCodeDetectionItems = GlobalManager.Instance.GetDetectionItems<ProductTypeInfo>(productTypeInfo.TypeCode);
            foreach (var item in details)
            {
                item.DetectItems = new ObservableCollection<DetectItemModel>(TypeCodeDetectionItems.Where(a => a.CameraName == item.CameraName && !keyValuePairs.FirstOrDefault(b => b.Key.Key == a.Guid.ToString()).Equals(default(KeyValuePair<string, bool>))));
            }
            return new ProductTypeModel
            {
                Id = productTypeInfo.Id,
                TypeCode = productTypeInfo.TypeCode,
                TypeName = productTypeInfo.TypeName,
                //Thumbnail = Core.ImageHelper.GetBitmapImage(details.FirstOrDefault()?.TemplateImagePath, 100),
                Details = details
            };
        }
        public virtual ProductTypeInfo ConvertTo()
        {
            string cameraConfigs = HYCommonUtils.SerializationUtils.JsonUtils.JsonSerialize(Details);
            Dictionary<string, Dictionary<string, string>> keyValuePairs = new Dictionary<string, Dictionary<string, string>>();
            foreach (var type in Details)
            {
                foreach (var item in type.DetectItems)
                {
                    string key = HYCommonUtils.SerializationUtils.JsonUtils.JsonSerialize(new KeyValuePair<string, bool>(item.Guid.ToString(), item.IsUsing));
                    keyValuePairs[key] = new Dictionary<string, string>();
                    foreach (var item1 in item.DetectItemConfig.AlgorithmConfig.AlgorithmUtilsItems)
                    {
                        keyValuePairs[key].Add(item1.AlgorithmUtilsName, item1.AlgorithmUtilsValue);
                    }
                }
            }
            KeyValuePair<string, Dictionary<string, Dictionary<string, string>>> keyValue = new KeyValuePair<string, Dictionary<string, Dictionary<string, string>>>(cameraConfigs, keyValuePairs);
            return new ProductTypeInfo
            {
                Id = Id,
                TypeCode = TypeCode,
                TypeName = TypeName,
                AlgorithmUtilsContent = HYCommonUtils.SerializationUtils.JsonUtils.JsonSerialize(keyValue)
            };
        }
    }
    public class ProductTypeModel<T> : ProductTypeModel
    {
        private T _data;

        public T Data
        {
            get { return _data; }
            set { _data = value; RaisePropertyChanged(); }
        }

    }
    public class ProductTypeDetailModel : NotifyPropertyObject
    {
        /// <summary>
        /// 相机名称
        /// </summary>
        public string CameraName { get; set; }
        /// <summary>
        /// 相机曝光
        /// </summary>
        public double CameraExposure { get; set; }
        /// <summary>
        /// 相机增益
        /// </summary>
        public double CameraGain { get; set; }
        private string _templateImagePath;
        /// <summary>
        /// 模板图片
        /// </summary>
        public string TemplateImagePath
        {
            get
            {
                return _templateImagePath;
            }
            set
            {
                _templateImagePath = value;
                RaisePropertyChanged();
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        /// <summary>
        /// 检测目录
        /// </summary>
        public ObservableCollection<DetectItemModel> DetectItems { get; set; }
    }
}
