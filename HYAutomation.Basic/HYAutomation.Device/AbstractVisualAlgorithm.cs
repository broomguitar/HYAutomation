using HYAutomation.Core.Algorithm.Models;
using System;
using System.Drawing;

namespace HYAutomation.Device
{
    public abstract class AbstractVisualAlgorithm : AbstractDevice
    {
        public override string DeviceName { get; set; } = "VisualAlgorithm";
        public override string DeviceDesc { get; set; } = "视觉算法";
        public override DeviceEnum DeviceEnum { get; } = DeviceEnum.Algorithm;
        /// <summary>
        /// 算法检测
        /// </summary>
        /// <param name="detectItem"></param>
        /// <param name="imgPath"></param>
        /// <param name="resultStr"></param>
        /// <param name="dateTime"></param>
        /// <param name="isConfig"></param>
        /// <returns></returns>
        public abstract bool GetCheckRet(DetectItemModel detectItem, string imgPath, out string resultStr, DateTime dateTime, bool isConfig = false);
        /// <summary>
        /// 算法检测
        /// </summary>
        /// <param name="detectItem"></param>
        /// <param name="bitmap"></param>
        /// <param name="resultStr"></param>
        /// <param name="dateTime"></param>
        /// <param name="isConfig"></param>
        /// <returns></returns>
        public virtual bool GetCheckRet(DetectItemModel detectItem, Bitmap bitmap, string imgPath, out string resultStr, DateTime dateTime, bool isConfig = false)
        {
            resultStr = string.Empty;
            return false;
        }
    }
}
