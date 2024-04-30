using HYAutomation.BLL;
using HYAutomation.DAL_Base;
using HYAutomation.Entity_Base;
using System;
using System.Collections.Generic;

namespace HYAutomation.BLL_Base
{
    public class MainBLL : IMainBLL<ProductInfo, ProductTypeInfo>
    {
        private readonly ProductTypeDAL<ProductTypeInfo> _typeDAL = new ProductTypeDAL<ProductTypeInfo>();
        private readonly ProductInfoDAL<ProductInfo> _InfoDAL = new ProductInfoDAL<ProductInfo>();
        private readonly DetectItemDAL _detectItemDAL = new DetectItemDAL();
        /// <summary>
        /// 查询检测项目库
        /// </summary>
        /// <returns></returns>
        public List<DetectItemInfo> GetDetectionItems(string typecode)
        {
            try
            {
                return _detectItemDAL.QueryDetectionItems(typecode);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 根据型号编号查询型号数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public ProductTypeInfo GetProductTypeByTypeCode(string typeCode)
        {
            try
            {
                if (string.IsNullOrEmpty(typeCode))
                {
                    return null;
                }
                return _typeDAL.QueryProductTypeByTypeCode(typeCode);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 添加生产数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool AddProductData(ProductInfo data)
        {
            try
            {
                if (data == null)
                {
                    return false;
                }
                return _InfoDAL.InsertProduct(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 根据条码获取产品数据
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public ProductInfo GetProduct(string barcode)
        {
            try
            {
                if (string.IsNullOrEmpty(barcode))
                {
                    return null;
                }
                return _InfoDAL.QueryProduct(barcode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 今日生产数据
        /// </summary>
        /// <returns></returns>
        public List<ProductInfo> GetToDayProducts()
        {
            try
            {
                return _InfoDAL.QueryProducts(DateTime.Now.Date, DateTime.Now.Date.AddDays(1), string.Empty, string.Empty, string.Empty, string.Empty);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
