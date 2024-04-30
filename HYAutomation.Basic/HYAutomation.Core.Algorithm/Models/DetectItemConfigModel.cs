using HYWindowUtils.WPF.VMBaseUtil;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace HYAutomation.Core.Algorithm.Models
{
    public class DetectItemConfigModel : NotifyPropertyObject
    {
        private string _detectItemName;
        /// <summary>
        /// 检测条目名称
        /// </summary>
        public string DetectItemName
        {
            get { return _detectItemName; }
            set { _detectItemName = value; RaisePropertyChanged(); }
        }
        private string _detectItemDesc;
        /// <summary>
        /// 检测条目名称
        /// </summary>
        public string DetectItemDesc
        {
            get { return _detectItemDesc; }
            set { _detectItemDesc = value; RaisePropertyChanged(); }
        }
        private string _markerBorderBrushStr = Brushes.Green.ToString();
        /// <summary>
        /// 标记框颜色
        /// </summary>
        public string MarkerBorderBrushStr
        {
            get { return _markerBorderBrushStr; }
            set { _markerBorderBrushStr = value; RaisePropertyChanged(); }
        }
        private AlgorithmConfigModel _algorithmConfig;
        /// <summary>
        /// 算法
        /// </summary>
        public AlgorithmConfigModel AlgorithmConfig
        {
            get { return _algorithmConfig; }
            set { _algorithmConfig = value; RaisePropertyChanged(); }
        }

    }
    public class AlgorithmConfigModel : NotifyPropertyObject
    {
        private string _algorithmName;

        public string AlgorithmName
        {
            get { return _algorithmName; }
            set { _algorithmName = value; RaisePropertyChanged(); }
        }

        private string _algorithmUrl;
        /// <summary>
        /// 算法路径
        /// </summary>
        public string AlgorithmUrl
        {
            get { return _algorithmUrl; }
            set { _algorithmUrl = value; RaisePropertyChanged(); }
        }
        private AlgorithmTypeEnum _algorithmType;
        /// <summary>
        /// 算法类型
        /// </summary>
        public AlgorithmTypeEnum AlgorithmType
        {
            get { return _algorithmType; }
            set { _algorithmType = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 算法配置
        /// </summary>
        public ObservableCollection<AlgorithmUtilsModel> AlgorithmUtilsItems { get; set; } = new ObservableCollection<AlgorithmUtilsModel>();
    }
    public class AlgorithmUtilsModel
    {
        public string AlgorithmUtilsName { get; set; }
        public string AlgorithmUtilsValue { get; set; } = "Default";
        public bool IsStandardValue { get; set; }
    }
    public enum AlgorithmTypeEnum
    {
        /// <summary>
        /// 二维码
        /// </summary>
        QRcode,
        /// <summary>
        /// 条形码
        /// </summary>
        Barcode,
        /// <summary>
        /// OCR
        /// </summary>
        OCR,
        /// <summary>
        /// 模板匹配
        /// </summary>
        TemplateMatching,
        /// <summary>
        /// 检测门对齐
        /// </summary>
        DetectDoorAlign,
        /// <summary>
        /// 检测门缝
        /// </summary>
        DetectDoorCrack,
        /// <summary>
        /// 检测表面
        /// </summary>
        DetectSurface,
        /// <summary>
        /// 色差
        /// </summary>
        ChromaticAberration,
        /// <summary>
        /// 检测目标
        /// </summary>
        DetectTarget,
        /// <summary>
        /// 推理分类
        /// </summary>
        Classification,
        /// <summary>
        /// 门平
        /// </summary>
        DoorPlane
    }
}
