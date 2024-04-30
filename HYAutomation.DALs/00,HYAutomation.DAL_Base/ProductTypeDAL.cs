using Dapper;
using HYAutomation.Entity_Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HYAutomation.DAL_Base
{
    /// <summary>
    /// 生产型号DAL
    /// </summary>
    public class ProductTypeDAL<T> where T : ProductTypeInfo
    {
        protected SQLHelper _mysqlHelper = SQLHelper.GetInstance(SQLHelper.DataBaseTypes.MySQL);
        /// <summary>
        /// 查询所有型号数据
        /// </summary>
        /// <returns></returns>
        public virtual List<T> QueryProductTypes()
        {
            string sql = "select * from producttypedata order by LastAccessTime asc";
            return _mysqlHelper.Query<T>(sql).ToList();
        }
        /// <summary>
        /// 根据搜索关键字型号数据
        /// </summary>
        /// <returns></returns>
        public virtual List<T> QueryProductTypes(string serachTxt)
        {
            string sql = "select * from producttypedata WHERE TypeCode LIKE '%" + serachTxt + "%'" + " OR TypeName LIKE '%" + serachTxt + "%'";
            return _mysqlHelper.Query<T>(sql).ToList();
        }
        /// <summary>
        /// 根据typecode查询型号信息
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public virtual T QueryProductTypeByTypeCode(string typeCode)
        {
            DynamicParameters d = new DynamicParameters();
            d.Add("TypeCode", typeCode);
            string sql = "select * from producttypedata where TypeCode=@TypeCode";
            return _mysqlHelper.QueryFirstOrDefault<T>(sql, d);
        }
        /// <summary>
        /// 更新型号
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual bool UpdateProductType(T data)
        {
            string sql = "Update producttypedata set TypeName=@TypeName,AlgorithmUtilsContent=@AlgorithmUtilsContent,LastAccessTime=@LastAccessTime where TypeCode=@TypeCode";
            return _mysqlHelper.Execute(sql, data) > 0;
        }
        /// <summary>
        ///添加型号
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual bool InsertProductType(T data)
        {
            string sql = "INSERT into producttypedata(TypeCode,TypeName,AlgorithmUtilsContent,LastAccessTime)values(@TypeCode,@TypeName,@AlgorithmUtilsContent,@LastAccessTime)";
            return _mysqlHelper.Execute(sql, data) > 0;
        }
        /// <summary>
        /// 删除型号同时删除该型号的条目
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public virtual bool DeleteProductType(string typeCode)
        {
            List<Tuple<string, object>> sqlLs = new List<Tuple<string, object>>();
            DynamicParameters p = new DynamicParameters();
            p.Add("TypeCode", typeCode);
            string sql = "DELETE FROM producttypedata where TypeCode=@TypeCode";
            string sql1 = "DELETE FROM detectitemdata where TypeCode=@TypeCode";
            sqlLs.Add(new Tuple<string, object>(sql, p));
            sqlLs.Add(new Tuple<string, object>(sql1, p));
            return _mysqlHelper.ExecuteTransaction(sqlLs) > 0;
        }
    }
}
