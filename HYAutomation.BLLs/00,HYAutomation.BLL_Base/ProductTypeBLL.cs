
using HYAutomation.BLL;
using HYAutomation.DAL_Base;
using HYAutomation.Entity_Base;
using System;
using System.Collections.Generic;

namespace HYAutomation.BLL_Base
{
    public class ProductTypeBLL : IProductTypeBLL<ProductTypeInfo>
    {
        private readonly ProductTypeDAL<ProductTypeInfo> _typeDAL = new ProductTypeDAL<ProductTypeInfo>();
        private readonly DetectItemDAL _detectItemDAL = new DetectItemDAL();
        /// <summary>
        /// 获取检测条目库
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
        /// 添加条目
        /// </summary>
        /// <param name="detectionItem"></param>
        /// <returns></returns>
        public bool AddDetectItem(DetectItemInfo detectionItem)
        {
            try
            {
                if (detectionItem == null)
                    return false;
                return _detectItemDAL.InsertDetectionItem(detectionItem);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 更新条目
        /// </summary>
        /// <param name="detectionItem"></param>
        /// <returns></returns>
        public bool UpdateDetectItem(DetectItemInfo detectionItem)
        {
            try
            {
                if (detectionItem == null)
                    return false;
                return _detectItemDAL.UpdateDetectionItem(detectionItem);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 删除条目
        /// </summary>
        /// <param name="detectionItem"></param>
        /// <returns></returns>
        public bool DeleteDetectItem(string typecode, string guid)
        {
            try
            {
                if (string.IsNullOrEmpty(typecode) || string.IsNullOrEmpty(guid))
                    return false;
                return _detectItemDAL.DeleteDetectionItem(typecode, guid);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 获取所有型号
        /// </summary>
        /// <returns></returns>
        public List<ProductTypeInfo> GetProductTypes()
        {
            try
            {
                return _typeDAL.QueryProductTypes();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 搜索型号
        /// </summary>
        /// <returns></returns>
        public List<ProductTypeInfo> GetProductTypes(string searchTxt)
        {
            try
            {
                return _typeDAL.QueryProductTypes(searchTxt);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 根据编号搜索型号
        /// </summary>
        /// <returns></returns>
        public ProductTypeInfo GetProductTypeByTypeCode(string typeCode)
        {
            try
            {
                return _typeDAL.QueryProductTypeByTypeCode(typeCode);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        ///添加数据
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public bool AddProductType(ProductTypeInfo productType)
        {
            try
            {
                if (productType == null)
                {
                    return false;
                }
                return _typeDAL.InsertProductType(productType);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public bool UpdateProductType(ProductTypeInfo productType)
        {
            try
            {
                if (productType == null)
                {
                    return false;
                }
                return _typeDAL.UpdateProductType(productType);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 根据型号编号删除型号
        /// </summary>
        /// <param name="typecode"></param>
        /// <returns></returns>
        public bool DeleteProductTypeByTypeCode(string typecode)
        {
            try
            {
                return _typeDAL.DeleteProductType(typecode);
            }
            catch (Exception ex)
            {
                throw ex;
                throw;
            }
        }
    }
}
