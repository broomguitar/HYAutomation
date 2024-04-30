using HYAutomation.Entity_Base;
using System.Collections.Generic;

namespace HYAutomation.BLL
{
    public interface IMainBLL
    {
        /// <summary>
        /// 查询检测项目库
        /// </summary>
        /// <returns></returns>
        List<DetectItemInfo> GetDetectionItems(string typecode);
    }
    public interface IMainBLL<T1, T2> : IMainBLL where T1 : ProductInfo where T2 : ProductTypeInfo
    {
        /// <summary>
        /// 根据型号编号查询型号数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        T2 GetProductTypeByTypeCode(string typeCode);
        /// <summary>
        /// 添加生产数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool AddProductData(T1 data);
        /// <summary>
        /// 根据条码获取产品数据
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        T1 GetProduct(string barcode);
        /// <summary>
        /// 今日生产数据
        /// </summary>
        /// <returns></returns>
        List<T1> GetToDayProducts();
    }
}
