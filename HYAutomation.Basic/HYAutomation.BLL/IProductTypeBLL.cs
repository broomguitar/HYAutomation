using HYAutomation.Entity_Base;
using System.Collections.Generic;

namespace HYAutomation.BLL
{
    public interface IProductTypeBLL
    {

        /// <summary>
        /// 获取检测条目库
        /// </summary>
        /// <returns></returns>
        List<DetectItemInfo> GetDetectionItems(string typecode);
        /// <summary>
        /// 添加条目
        /// </summary>
        /// <param name="detectionItem"></param>
        /// <returns></returns>
        bool AddDetectItem(DetectItemInfo detectionItem);
        /// <summary>
        /// 更新条目
        /// </summary>
        /// <param name="detectionItem"></param>
        /// <returns></returns>
        bool UpdateDetectItem(DetectItemInfo detectionItem);
        /// <summary>
        /// 删除条目
        /// </summary>
        /// <param name="detectionItem"></param>
        /// <returns></returns>
        bool DeleteDetectItem(string typecode, string guid);
        /// <summary>
        /// 根据型号编号删除型号
        /// </summary>
        /// <param name="typecode"></param>
        /// <returns></returns>
        bool DeleteProductTypeByTypeCode(string typecode);
    }
    public interface IProductTypeBLL<T> : IProductTypeBLL where T : ProductTypeInfo
    {
        /// <summary>
        /// 获取所有型号
        /// </summary>
        /// <returns></returns>
        List<T> GetProductTypes();
        /// <summary>
        /// 搜索型号
        /// </summary>
        /// <returns></returns>
        List<T> GetProductTypes(string searchTxt);
        /// <summary>
        /// 根据编号搜索型号
        /// </summary>
        /// <returns></returns>
        T GetProductTypeByTypeCode(string typeCode);
        /// <summary>
        ///添加数据
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        bool AddProductType(T productType);
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        bool UpdateProductType(T productType);
    }
}
