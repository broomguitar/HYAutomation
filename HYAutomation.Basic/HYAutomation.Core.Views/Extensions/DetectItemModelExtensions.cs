using HYAutomation.BLL;
using HYAutomation.Core.Algorithm.Models;
using HYAutomation.Entity_Base;
using HYCommonUtils.SerializationUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace HYAutomation.Core.Views.Extensions
{
    public static class DetectItemModelExtension
    {
        /// <summary>
        /// 检测条目转换  业务model 转换数据Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_"></param>
        /// <param name="detectionItem"></param>
        /// <returns></returns>
        public static DetectItemModel ConverFrom<T>(this DetectItemModel _, DetectItemInfo detectionItem) where T : ProductTypeInfo
        {
            if (detectionItem == null)
            {
                return null;
            }
            DetectItemModel ret = new DetectItemModel
            {
                Guid = Guid.Parse(detectionItem.Guid),
                CameraName = detectionItem.CameraName,
                DetectItemRegion = Int32Rect.Parse(detectionItem.DetectitemRegion),
                DetectItemLocation = Point.Parse(detectionItem.DetectItemLocation),
                TypeCode = detectionItem.TypeCode
            };
            var productTypeInfo = ((IProductTypeBLL<T>)GlobalManager.Instance.ProductTypeBLL).GetProductTypeByTypeCode(detectionItem.TypeCode);
            Dictionary<KeyValuePair<string, bool>, Dictionary<string, string>> keyValuePairs = new Dictionary<KeyValuePair<string, bool>, Dictionary<string, string>>();
            var data = JsonUtils.JsonDeserialize<KeyValuePair<string, Dictionary<string, Dictionary<string, string>>>>(productTypeInfo.AlgorithmUtilsContent);
            foreach (var item in data.Value)
            {
                try
                {
                    keyValuePairs.Add(JsonUtils.JsonDeserialize<KeyValuePair<string, bool>>(item.Key), item.Value);
                }
                catch
                {
                    keyValuePairs.Add(new KeyValuePair<string, bool>(item.Key, true), item.Value);
                }
            }
            var detectItem = Algorithm.AlgorithmConfig.Instance.DetectItemConfigs.FirstOrDefault(c => c.DetectItemName == detectionItem.DetectItemName);
            if (detectItem != null)
            {
                foreach (var item1 in detectItem.AlgorithmConfig.AlgorithmUtilsItems)
                {
                    var configValue = keyValuePairs.Keys.FirstOrDefault(a => a.Key == detectionItem.Guid);
                    if (!string.IsNullOrEmpty(configValue.Key))
                    {
                        ret.IsUsing = configValue.Value;
                        if (keyValuePairs[configValue].ContainsKey(item1.AlgorithmUtilsName))
                        {
                            item1.AlgorithmUtilsValue = keyValuePairs[configValue][item1.AlgorithmUtilsName];
                        }
                    }
                }
            }
            ret.DetectItemConfig = detectItem;
            return ret;
        }
        public static DetectItemInfo ConverTo(this DetectItemModel detectionItemModel)
        {
            return new DetectItemInfo
            {
                Guid = detectionItemModel.Guid.ToString(),
                DetectItemName = detectionItemModel.DetectItemConfig.DetectItemName,
                CameraName = detectionItemModel.CameraName,
                DetectitemRegion = detectionItemModel.DetectItemRegion.ToString(),
                DetectItemLocation = detectionItemModel.DetectItemLocation.ToString(),
                TypeCode = detectionItemModel.TypeCode
            };
        }
    }
}
