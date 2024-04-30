using Dapper;
using HYAutomation.Entity_Base;
using System.Collections.Generic;
using System.Linq;

namespace HYAutomation.DAL_Base
{
    /// <summary>
    /// 检测条目信息DAL
    /// </summary>
    public class DetectItemDAL
    {
        private SQLHelper _mysqlHelper = SQLHelper.GetInstance(SQLHelper.DataBaseTypes.MySQL);
        /// <summary>
        /// 查询某型号所有检测条目库数据
        /// </summary>
        /// <returns></returns>
        public List<DetectItemInfo> QueryDetectionItems(string typecode)
        {
            DynamicParameters d = new DynamicParameters();
            d.Add("TypeCode", typecode);
            string sql = "select * from detectitemdata where TypeCode=@TypeCode";
            return _mysqlHelper.Query<DetectItemInfo>(sql, d).ToList();
        }
        /// <summary>
        /// 根据Guid查询某型号的条目
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public DetectItemInfo QueryDetectionItem(string typecode, string Guid)
        {
            DynamicParameters d = new DynamicParameters();
            d.Add("TypeCode", typecode);
            d.Add("Guid", Guid);
            string sql = "select * from detectitemdata where TypeCode=@TypeCode AND Guid=@Guid";
            return _mysqlHelper.QueryFirstOrDefault<DetectItemInfo>(sql, d);
        }
        /// <summary>
        /// 更新检测条目数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool UpdateDetectionItem(DetectItemInfo data)
        {
            string sql = "Update detectitemdata set TypeCode=@TypeCode,CameraName=@CameraName,DetectItemName=@DetectItemName,DetectitemRegion=@DetectitemRegion,DetectItemLocation=@DetectItemLocation where Guid=@Guid";
            return _mysqlHelper.Execute(sql, data) > 0;
        }
        /// <summary>
        //插入检测条目数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool InsertDetectionItem(DetectItemInfo data)
        {
            string sql = "INSERT into detectitemdata(Guid,TypeCode,CameraName,DetectItemName,DetectItemRegion,DetectItemLocation)values(@Guid,@TypeCode,@CameraName,@DetectItemName,@DetectItemRegion,@DetectItemLocation)";
            return _mysqlHelper.Execute(sql, data) > 0;
        }
        /// <summary>
        /// 删除检测条目数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public bool DeleteDetectionItem(string typeCode, string Guid)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("TypeCode", typeCode);
            p.Add("Guid", Guid);
            string sql = "DELETE FROM detectitemdata WHERE TypeCode=@TypeCode AND Guid=@Guid";
            return _mysqlHelper.Execute(sql, p) > 0;
        }
        /// <summary>
        /// 删除检测条目数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public bool DeleteTypeDetectionItems(string typeCode)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("TypeCode", typeCode);
            string sql = "DELETE FROM detectitemdata WHERE TypeCode=@TypeCode";
            return SQLHelper.GetInstance(SQLHelper.DataBaseTypes.MySQL).Execute(sql, p) > 0;
        }
    }
}
