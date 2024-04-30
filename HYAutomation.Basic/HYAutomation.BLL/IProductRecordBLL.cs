using HYAutomation.Entity_Base;
using System;
using System.Collections.Generic;

namespace HYAutomation.BLL
{
    public interface IProductRecordBLL<T> where T : ProductInfo
    {
        /// <summary>
        /// 获取生产数据
        /// </summary>
        /// <returns></returns>
        List<T> GetProducts();
        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="barcode"></param>
        /// <param name="typeCode"></param>
        /// <param name="typeName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        List<T> GetProducts(DateTime start, DateTime end, string barcode, string typeCode, string typeName, string filter);
    }
}
