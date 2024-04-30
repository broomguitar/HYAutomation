using HYAutomation.Core.Views.Models;
using HYAutomation.Device;
using HYAutomation.Devices.HiEdgeMind;
using HYAutomation.Entity_Base;
using HYCommonUtils.SerializationUtils;
using System;
using System.Collections.Generic;

namespace HYAutomation.Projects.HiEdgeMind
{
    #region 硬件
        public class HardwareItem
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string deviceId { get; set; }
        /// <summary>
        /// 索引号
        /// </summary>
        public int sort_index { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 设备描述
        /// </summary>
        public string memo { get; set; }
        /// <summary>
        /// 1:TCP，2:其他（串口或采集卡）
        /// </summary>
        public ConnectEnum connetType { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int port { get; set; }
        /// <summary>
        /// 1:PLC;2:相机;3:扫码枪;4:IO板卡;5:机械臂
        /// </summary>
        public HardwareTypeEnum type { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string brand { get; set; }
        public int needConfigFile { get; set; }
        /// <summary>
        /// 1:面阵相机;2:线扫相机;3:3D相机
        /// </summary>
        public int cameraType { get; set; }
        public string sn { get; set; }
        /// <summary>
        /// 1:无;2;软触发;3:硬触发
        /// </summary>
        public int trigger_type { get; set; }
        /// <summary>
        /// 到位信号地址
        /// </summary>
        public string Addr_Arrive { get; set; }
        /// <summary>
        /// 合格信号地址值
        /// </summary>
        public string Addr_OK { get; set; }
        /// <summary>
        /// NG信号地址值
        /// </summary>
        public string Addr_NG { get; set; }
        /// <summary>
        /// 抓拍完成信号地址值
        /// </summary>
        public string Addr_GrabFinish { get; set; }
        /// <summary>
        /// 线扫相机使能信号
        /// </summary>
        public string Addr_LinearStart { get; set; }
        /// <summary>
        /// 通讯成功信号地址值
        /// </summary>
        public string Addr_CommOK { get; set; }
    }
    public enum HardwareTypeEnum
    {
        None,
        PLC,
        Camera,
        Scanner,
        IOBoard,
        Robot
    }
    public enum ConnectEnum
    {
        None,
        Tcp,
        SerialPort
    }
    public class HardwareRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public List<HardwareItem> hardwareList { get; set; }
    }
    #endregion
    #region 工作流
    public class Properties
    {
        /// <summary>
        /// 
        /// </summary>
        public string targetNodeType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string sourceNodeType { get; set; }
    }

    public class LineListItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int targetNodeType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lineId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lineType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string sourceNodeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int sourceNodeType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string targetNodeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Properties properties { get; set; }
    }
    public class SourceRefsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string sourceRef { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string condition { get; set; }
    }
    public class TargetRefsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string targetRef { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string condition { get; set; }
    }
    public class InputOutputItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string format { get; set; }
        public int originIndex { get; set; }
        public string type { get; set; }
        public string value { get; set; }

    }

    public class NodeListItem
    {
        /// <summary>
        /// 开始
        /// </summary>
        public string nodeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<SourceRefsItem> sourceRefs { get; set; }
        /// <summary>
        /// 开始节点: 1;结束节点: 2;等待节点: 3;任务节点: 4;排他网关: 5;并行网关: 6.
        /// </summary>
        public int nodeType { get; set; }
        /// <summary>
        /// 1:创建对象;2:等待产品到位;3:查询型号信息;4:相机参数设置;5:执行算法检测项;6:合并检测结果;7:设置结果.
        /// </summary>
        public int? taskType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string @delegate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<TargetRefsItem> targetRefs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string hardwareName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<InputOutputItem> inputOutput { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string nodeId { get; set; }
        public bool isFinish { get; set; }
    }

    public class WorkFlowRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public List<LineListItem> lineList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<NodeListItem> nodeList { get; set; }
    }
    #endregion
    #region 检测项

    public class CheckItemData
    {
        public string color { get; set; }
        public int algorithmVersionId { get; set; }
        public int checkItemId { get; set; }
        public string checkItemName { get; set; }
    }
    #endregion
    #region 产品型号信息
    public class assistParamdata
    {
        public string productCheckItemId { get; set; }
        public string auxKey { get; set; }
        public string auxValue { get; set; }
        public string standard { get; set; }
    }
    public class CheckItem
    {
        public int? xmin { get; set; }
        public int? ymin { get; set; }
        public int? xmax { get; set; }
        public int? ymax { get; set; }
        public List<assistParamdata> assistParam { get; set; }
        public int? businessModelId { get; set; }
        public int checkItemId { get; set; }
        public int uses { get; set; }
    }
    public class ProductTypeDetectItem
    {
        public string hardwareId { get; set; }
        public string hardwareName { get; set; }
        public string imgPath { get; set; }
        public List<CheckItem> checkItemList { get; set; }
        public string extraCfg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, object> extraCfgData
        {
            get
            {
                if (!string.IsNullOrEmpty(extraCfg))
                {
                    try
                    {
                        return JsonUtils.JsonDeserialize<Dictionary<string, object>>(extraCfg);
                    }
                    catch { return null; }
                }
                return null;
            }
        }
    }
    public class ProductTypeData
    {
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public List<ProductTypeDetectItem> hardwareList { get; set; }
    }
    #endregion
    public class DarknetModelFileUrl
    {
        public string names { get; set; }
        public string pt { get; set; }
        public string data { get; set; }
        public string cfg { get; set; }
    }
}
