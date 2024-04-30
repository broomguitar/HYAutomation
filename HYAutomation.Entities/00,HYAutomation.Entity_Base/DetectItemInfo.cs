namespace HYAutomation.Entity_Base
{
    /// <summary>
    /// 检测条目信息
    /// </summary>
    public class DetectItemInfo
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 检测条目的Guid
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// 型号编号
        /// </summary>
        public string TypeCode { get; set; }
        /// <summary>
        /// 相机名称
        /// </summary>
        public string CameraName { get; set; }
        /// <summary>
        /// 检测条目名字
        /// </summary>
        public string DetectItemName { get; set; }
        /// <summary>
        /// 检测条目范围
        /// </summary>
        public string DetectitemRegion { get; set; }
        /// <summary>
        /// 检测条目框位置
        /// </summary>
        public string DetectItemLocation { get; set; }
    }
}
