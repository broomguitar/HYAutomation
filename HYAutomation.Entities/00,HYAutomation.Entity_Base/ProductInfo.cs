using System;

namespace HYAutomation.Entity_Base
{
    /// 产品信息
    /// </summary>
    public class ProductInfo
    {
        /// <summary>
        /// 唯一ID值
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 条码数据
        /// </summary>
        public string Barcode { get; set; }
        /// <summary>
        /// 型号编号
        /// </summary>
        public string TypeCode { get; set; }
        /// <summary>
        /// 型号名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 相机图片信息
        /// </summary>
        public string CameraImageInfos { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 检测结果是否合格
        /// </summary>
        public bool IsOK { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
    }
    public class ProductInfo<T> : ProductInfo
    {
        public T Data { get; set; }
    }
}
