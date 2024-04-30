using HYAutomation.BLL;
using HYAutomation.DAL_Base;
using HYAutomation.Entity_Base;
using System;
using System.Collections.Generic;

namespace HYAutomation.BLL_Base
{
    public class ProductRecordBLL : IProductRecordBLL<ProductInfo>
    {
        private readonly ProductInfoDAL<ProductInfo> _productInfoDAL = new ProductInfoDAL<ProductInfo>();
        /// <summary>
        /// 获取生产数据
        /// </summary>
        /// <returns></returns>
        public List<ProductInfo> GetProducts()
        {
            try
            {
                return _productInfoDAL.QueryProducts();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
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
        public List<ProductInfo> GetProducts(DateTime start, DateTime end, string barcode, string typeCode, string typeName, string filter)
        {
            try
            {
                return _productInfoDAL.QueryProducts(start, end, barcode, typeCode, typeName, filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
