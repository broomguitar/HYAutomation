using Dapper;
using HYAutomation.Entity_Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HYAutomation.DAL_Base
{
    /// <summary>
    /// 产品信息DAL
    /// </summary>
    public class ProductInfoDAL<T> where T : ProductInfo
    {
        protected SQLHelper m_mysqlHelper = SQLHelper.GetInstance(SQLHelper.DataBaseTypes.MySQL);
        /// <summary>
        /// 查询所有生产数据
        /// </summary>
        /// <returns></returns>
        public virtual List<T> QueryProducts()
        {
            string sql = "select * from productinfodata order by CreateTime";
            return m_mysqlHelper.Query<T>(sql).ToList();
        }
        /// <summary>
        /// 根据条件查询生产数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="barcode"></param>
        /// <param name="typeCode"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public virtual List<T> QueryProducts(DateTime start, DateTime end, string barcode, string typeCode, string typeName, string filter)
        {
            string sql = "Select * from productinfodata where CreateTime between '" + start + "' and '" + end + "'";
            if (!string.IsNullOrEmpty(barcode))
            {
                sql += " and Barcode like '%" + barcode + "%'";
            }
            if (!string.IsNullOrEmpty(typeCode))
            {
                sql += " and TypeCode like '%" + typeCode + "%'";
            }
            if (!string.IsNullOrEmpty(typeName))
            {
                sql += " and TypeName like '%" + typeName + "%'";
            }
            if (!string.IsNullOrEmpty(filter))
            {
                if (filter == "OK")
                {
                    sql += "and IsOk=1";
                }
                else if (filter == "NG")
                {
                    sql += "and IsOk=0";
                }
            }
            return m_mysqlHelper.Query<T>(sql).ToList();
        }
        /// <summary>
        /// 根据条码查询产品信息
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public virtual T QueryProduct(string barcode)
        {
            DynamicParameters d = new DynamicParameters();
            d.Add("Barcode", barcode);
            string sql = "select * from productinfodata where Barcode=@Barcode";
            return m_mysqlHelper.QueryFirstOrDefault<T>(sql, d);
        }
        /// <summary>
        /// 更新生产数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual bool UpdateProduct(T data)
        {
            string sql = "Update productinfodata set TypeCode=@TypeCode,TypeName=@TypeName,CreateTime=@CreateTime,CameraImageInfos=@CameraImageInfos,IsOk=@IsOk,Note=@Note where Barcode=@Barcode";
            return m_mysqlHelper.Execute(sql, data) > 0;
        }
        /// <summary>
        //插入生产数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual bool InsertProduct(T data)
        {
            string sql = "INSERT into productinfodata(TypeCode,TypeName,CreateTime,Barcode,CameraImageInfos,IsOk,Note)values(@TypeCode,@TypeName,@CreateTime,@Barcode,@CameraImageInfos,@IsOk,@Note)";
            return m_mysqlHelper.Execute(sql, data) > 0;
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public virtual bool DeleteProduct(string barcode)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("Barcode", barcode);
            string sql = "DELETE FROM productinfodata where Barcode=@Barcode";
            return m_mysqlHelper.Execute(sql, p) > 0;
        }
    }
}
