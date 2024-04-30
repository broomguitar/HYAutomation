using System;

namespace HYAutomation.Entity_Base
{
    /// <summary>
    /// 型号信息
    /// </summary>
    public class ProductTypeInfo
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 型号编码
        /// </summary>
        public string TypeCode { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 算法辅助参数
        /// </summary>
        public string AlgorithmUtilsContent { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastAccessTime { get; set; } = DateTime.Now;
    }
    /// <summary>
    /// 型号信息
    /// </summary>
    public class ProductTypeInfo<T> : ProductTypeInfo
    {
        public T Data { get; set; }
    }
}
