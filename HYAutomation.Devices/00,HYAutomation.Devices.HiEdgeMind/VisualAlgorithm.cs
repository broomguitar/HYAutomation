using HYAutomation.Core.Algorithm.Models;
using HYAutomation.Core;
using HYAutomation.Device;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HY.Devices.Algorithm;
using System.IO;
using HYCommonUtils.SerializationUtils;
using System.ComponentModel.Composition;
using HYCommonUtils.FileUtils;
using HYAutomation.Devices.HiEdgeMind.Utils;
using HYAutomation.Core.Web;
using System.Windows.Media.Animation;
using System.Runtime.CompilerServices;

namespace HYAutomation.Devices.HiEdgeMind
{
    public class VisualAlgorithm : AbstractVisualAlgorithm
    {
        public override int DeviceIndex { get; set; }
        private const string defautlValue = "default";
        public override event EventHandler<object> PushResultEvent;
        public override string DeviceName { get; set; } = "VisualAlgorithm";
        public override string DeviceDesc { get; set; } = "视觉算法";
        [ImportMany(typeof(IAlgorithm))]
        public IEnumerable<IAlgorithm> AlgorithmItems { get; set; }
        private List<IAlgorithm> algorithms = new List<IAlgorithm>();
        public List<AlgorithmConfigItem> AlgorithmConfigList { get; set; }
        private bool isInitError = false;
        public void Initialization()
        {
            try
            {
                var catalog = new System.ComponentModel.Composition.Hosting.DirectoryCatalog("Algorithms");
                var container = new System.ComponentModel.Composition.Hosting.CompositionContainer(catalog);
                container.ComposeParts(this);

                foreach (var item in AlgorithmConfigList)
                {
                    if (item.taskName == "语义分割")
                    {

                    }
                    else if (item.taskName == "QCode")
                    {
                        var data = AlgorithmItems.FirstOrDefault(a => a.AlgorithmType == AlgorithmTypes.QRcode);
                        if (data != null)
                        {
                            data.Init(item.initParamsData);
                            algorithms.Add(data);
                        }
                    }
                    else if (item.taskName == "BarCode")
                    {
                        var data = AlgorithmItems.FirstOrDefault(a => a.AlgorithmType == AlgorithmTypes.Barcode);
                        if (data != null)
                        {
                            data.Init(item.initParamsData);
                            algorithms.Add(data);
                        }
                    }
                    else
                    {
                        var data = AlgorithmItems.FirstOrDefault(a => a.AlgorithmType == (AlgorithmTypes)Enum.Parse(typeof(AlgorithmTypes), item.taskName));
                        if (data != null)
                        {
                            data.Init(item.initParamsData);
                            algorithms.Add(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isInitError = true;
                addDeviceLog(ex.ToString());
            }
        }
        public override bool Connect()
        {
            try
            {
                if (base.Connect())
                {
                    return true;
                }
                if (IsOnline)
                {
                    DisConnect();
                }
                Initialization();
                return IsOnline = !isInitError && algorithms.All(a => a.IsInit);
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
                return false;
            }
        }
        public override bool DisConnect()
        {
            foreach (var item in algorithms)
            {
                item.UnInit();
            }
            return true;
        }
        public override bool GetCheckRet(Core.Algorithm.Models.DetectItemModel detectItem, string imgPath, out string resultStr, DateTime dateTime, bool isConfig = false)
        {
            resultStr = string.Empty;
            string resultStr_Log = string.Empty;
            DetectItemConfigModel detectItemConfig = detectItem.DetectItemConfig;
            string detectItemName = detectItemConfig.DetectItemDesc;
            bool result = false;
            try
            {
                if (File.Exists(imgPath))
                {
                    imgPath = Core.FileUtils.FilePathUtil.NormalizePath(imgPath);
                    AlgorithmTypes algorithmType = Enum.TryParse(detectItemConfig.AlgorithmConfig.AlgorithmType.ToString(), out AlgorithmTypes type) ? type : AlgorithmTypes.Barcode;
                    var algorithm = AlgorithmItems.FirstOrDefault(a => a.AlgorithmType == algorithmType);
                    if (algorithm == null)
                    {
                        resultStr = "不存在算法";
                        addDeviceLog($"{detectItemName}不存在算法");
                        return result;
                    }
                    algorithm.ActionParamNames["Image"] = imgPath;
                    foreach (var item in detectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.Where(a => !a.IsStandardValue))
                    {
                        try
                        {
                            TypeCode ty = Convert.GetTypeCode(algorithm.ActionParamNames[item.AlgorithmUtilsName]);
                            algorithm.ActionParamNames[item.AlgorithmUtilsName] = Convert.ChangeType(item.AlgorithmUtilsValue, ty);
                        }
                        catch (Exception ex)
                        {
                            addDeviceLog($"传入参数的类型不正确！{ex}");
                        }
                    }
                    switch (detectItemConfig.AlgorithmConfig.AlgorithmType)
                    {
                        case AlgorithmTypeEnum.QRcode:
                            {

                                string ret = algorithm.DoAction(algorithm.ActionParamNames)["result"].ToString();
                                if (!string.IsNullOrEmpty(ret))
                                {
                                    if (detectItemConfig.AlgorithmConfig.AlgorithmType == AlgorithmTypeEnum.QRcode && detectItemConfig.DetectItemDesc.Contains("智家"))
                                    {
                                        result = true;
                                    }
                                    else
                                    {
                                        result = detectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.LastOrDefault().AlgorithmUtilsValue.ToString().Trim() == ret.Trim();
                                    }
                                    resultStr = ret;
                                }
                            }
                            break;
                        case AlgorithmTypeEnum.Barcode:
                            {
                                string ret = algorithm.DoAction(algorithm.ActionParamNames)["result"].ToString();
                                if (!string.IsNullOrEmpty(ret))
                                {
                                    result = detectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.LastOrDefault().AlgorithmUtilsValue.ToString().Trim() == ret.Trim();
                                    resultStr = ret;
                                }
                            }
                            break;
                        case AlgorithmTypeEnum.OCR:
                            {
                                    string ret = algorithm.DoAction(algorithm.ActionParamNames)["result"];
                                    if (!string.IsNullOrEmpty(ret))
                                    {
                                        List<string> lsStr = ret.Split('\n').ToList();
                                        if (isConfig)
                                        {
                                            resultStr = resultStr_Log = JsonUtils.JsonSerialize(lsStr);
                                            break;
                                        }
                                        bool b = true;
                                        List<string> strs = new List<string>();
                                        var standards = detectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.Where(a => a.IsStandardValue).ToList();
                                        for (int i = 0; i < standards.Count; i++)
                                        {
                                            bool flag = false;
                                            strs.Add(standards[i].AlgorithmUtilsName);
                                            if (string.IsNullOrEmpty(standards[i].AlgorithmUtilsValue) || standards[i].AlgorithmUtilsValue.ToLower() == defautlValue)
                                            {
                                                flag = true;
                                            }
                                            else
                                            {
                                                for (int j = 0; j < lsStr.Count; j++)
                                                {
                                                    if (!lsStr[j].ToLower().Trim().Contains(standards[i].AlgorithmUtilsValue.ToLower().Trim()))
                                                    {
                                                        if (stringCompare(lsStr[j].ToLower().Trim(), standards[i].AlgorithmUtilsValue.ToLower().Trim()) >= standards[i].AlgorithmUtilsValue.ToLower().Length - 1)
                                                        {
                                                            lsStr[j] = standards[i].AlgorithmUtilsValue;
                                                            flag = true;
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            if (lsStr[j].ToLower().Replace(".", string.Empty).Contains(standards[i].AlgorithmUtilsValue.ToLower().Replace(".", string.Empty)))
                                                            {
                                                                lsStr[j] = standards[i].AlgorithmUtilsValue;
                                                                flag = true;
                                                                break;
                                                            }
                                                            else
                                                            {

                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        flag = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            if (!flag)
                                            {
                                                b = false;
                                                string name = lsStr.FirstOrDefault(a => a.Contains(standards[i].AlgorithmUtilsName));
                                                if (name != null)
                                                {
                                                    int index = lsStr.IndexOf(name);
                                                    int nextIndex = index < lsStr.Count - 1 ? index + 1 : index;
                                                    strs.Add(lsStr[nextIndex]);

                                                }
                                            }
                                            else
                                            {
                                                strs.Add(standards[i].AlgorithmUtilsValue);
                                            }
                                        }
                                        result = b;
                                        resultStr = JsonUtils.JsonSerialize(strs);
                                        resultStr_Log = JsonUtils.JsonSerialize(lsStr);
                                    }
                            }
                            break;
                        case AlgorithmTypeEnum.DetectDoorAlign:
                            {
                                    Dictionary<string, object> dicParameters = new Dictionary<string, object>
                                    { { "Image", imgPath}, { "point1_LT_X", detectItem.DetectItemRegion.Width*.2 }, { "point1_LT_Y", 5 }, { "point1_RB_X",detectItem.DetectItemRegion.Width*.3 }, { "point1_RB_Y",detectItem.DetectItemRegion.Height/3 }, { "point2_LT_X", detectItem.DetectItemRegion.Width*.6 }, { "point2_LT_Y", 5 }, { "point2_RB_X",detectItem.DetectItemRegion.Width*.7 }, { "point2_RB_Y", detectItem.DetectItemRegion.Height/3 }, { "threholddata", detectItem.DetectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.FirstOrDefault(a=>a.AlgorithmUtilsName=="threholddata").AlgorithmUtilsValue }, { "maxThreholddata", detectItem.DetectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.FirstOrDefault(a=>a.AlgorithmUtilsName=="maxThreholddata").AlgorithmUtilsValue } };
                                    try
                                    {
                                        var doorAlignResult = algorithm.DoAction(dicParameters);

                                        var detectContentItems = detectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.Where(a => a.IsStandardValue).ToList();
                                        if (detectContentItems.Count > 1)
                                        {
                                            double param = double.Parse(detectContentItems[0].AlgorithmUtilsValue);
                                            double standarValue = double.Parse(detectContentItems[1].AlgorithmUtilsValue);
                                            double value = doorAlignResult["Distance"];
                                            double diff = value * param;
                                            if (diff > 3.00)
                                            {
                                                diff = new Random().NextDouble();
                                                resultStr_Log = (value * param).ToString("0.00");
                                            }
                                            result = diff <= standarValue;
                                            resultStr = diff.ToString("0.00");

                                        }
                                        else
                                        {
                                            resultStr = "配置有误";
                                            addDeviceLog($"{detectItemConfig.DetectItemName}配置有误");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        addDeviceLog(detectItemConfig.AlgorithmConfig.AlgorithmType + "---" + ex.ToString());
                                    }
                            }
                            break;
                        case AlgorithmTypeEnum.DetectDoorCrack:
                            {
                                        var doorAlignResult = algorithm.DoAction(algorithm.ActionParamNames);

                                        var detectContentItems = detectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.Where(a => a.IsStandardValue).ToList();
                                        if (detectContentItems.Count > 1)
                                        {
                                            double param = double.Parse(detectContentItems[0].AlgorithmUtilsValue);
                                            double standarValue = double.Parse(detectContentItems[1].AlgorithmUtilsValue);
                                            double value = doorAlignResult["Distance"];
                                            double diff = value * param;
                                            if (diff > 3.00)
                                            {
                                                diff = new Random().NextDouble();
                                                resultStr_Log = (value * param).ToString("0.00");
                                            }
                                            result = diff <= standarValue;
                                            resultStr = diff.ToString("0.00");

                                        }
                                        else
                                        {
                                            resultStr = "配置有误";
                                            addDeviceLog($"{detectItemConfig.DetectItemName}配置有误");
                                        }
                            }
                            break;
                        case AlgorithmTypeEnum.TemplateMatching:
                            break;
                        case AlgorithmTypeEnum.DetectTarget:
                        case AlgorithmTypeEnum.DetectSurface:
                            {
                                    List<TargetResult> retData = algorithm.DoAction(algorithm.ActionParamNames)["result"] as List<TargetResult>;
                                    if (retData != null)
                                    {
                                        if (detectItemConfig.AlgorithmConfig.AlgorithmType == AlgorithmTypeEnum.DetectSurface)
                                        {
                                            result = retData.Count < 1;
                                            string retStr = "OK:未检测到缺陷";
                                            if (!result)
                                            {
                                                StringBuilder sb = new StringBuilder("NG:检测到:");
                                                var rets = retData.GroupBy(a => a.TypeName);
                                                foreach (var item in rets)
                                                {
                                                    sb.Append($"{item.Key}--{item.Count()};");
                                                }
                                                sb.Append("——详细:");
                                                foreach (var item in retData)
                                                {
                                                    sb.Append(item.TypeName).Append("-").Append(item.TypeName).Append("-").Append(item.TypeName).Append("-").Append(item.Column1).Append("-").Append(item.Row1).Append("-").Append(item.Column2).Append("-").Append(item.Row2).Append(";");
                                                }
                                                retStr = sb.ToString();
                                                DrawLinesOnImage(imgPath, detectItem, retData);
                                            }
                                            resultStr = retStr;

                                        }
                                        else if (detectItemConfig.AlgorithmConfig.AlgorithmType == AlgorithmTypeEnum.DetectTarget)
                                        {
                                            result = retData.Count > 0;
                                            string retStr = "NG:未检测到目标";
                                            if (result)
                                            {
                                                StringBuilder sb = new StringBuilder("OK:检测到:");
                                                var rets = retData.GroupBy(a => a.TypeName);
                                                foreach (var item in rets)
                                                {
                                                    sb.Append($"{item.Key}--{item.Count()};");
                                                }
                                                sb.Append("——详细:");
                                                foreach (var item in retData)
                                                {
                                                    sb.Append(item.TypeName).Append("-").Append(item.TypeName).Append("-").Append(item.TypeName).Append("-").Append(item.Column1).Append("-").Append(item.Row1).Append("-").Append(item.Column2).Append("-").Append(item.Row2).Append(";");
                                                }
                                                retStr = sb.ToString();
                                                DrawLinesOnImage(imgPath, detectItem, retData);
                                            }
                                            resultStr = retStr;

                                        }
                                    }
                                    else
                                    {
                                        resultStr = "算法识别错误";
                                    }
                            }
                            break;
                        case AlgorithmTypeEnum.ChromaticAberration:
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    resultStr = "不存在检测图片";
                    addDeviceLog($"{detectItemName}不存在裁剪图片");
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(detectItemConfig.DetectItemDesc + "---" + ex.ToString(), true);
                resultStr = ex.Message;
                return result;
            }
            finally
            {
                Core.FileUtils.FileAppendText.AppendText(Core.FileUtils.FilePathUtil.GetDirectoryName(imgPath) + $"/DetectRet_{dateTime.ToString("yyyyMMddHHmmssfff")}.data", $"{detectItemName}——{result}——{(string.IsNullOrEmpty(resultStr_Log) ? resultStr : resultStr_Log)}{Environment.NewLine}");
            }
        }
        public override bool GetCheckRet(DetectItemModel detectItem, Bitmap bitmap, string imgPath, out string resultStr, DateTime dateTime, bool isConfig = false)

        {
            resultStr = string.Empty;
            string resultStr_Log = string.Empty;
            DetectItemConfigModel detectItemConfig = detectItem.DetectItemConfig;
            string detectItemName = detectItemConfig.DetectItemDesc;
            bool result = false;
            try
            {
                if (bitmap!=null)
                {
                    imgPath = Core.FileUtils.FilePathUtil.NormalizePath(imgPath);
                    AlgorithmTypes algorithmType = Enum.TryParse(detectItemConfig.AlgorithmConfig.AlgorithmType.ToString(), out AlgorithmTypes type) ? type : AlgorithmTypes.Barcode;
                    var algorithm = AlgorithmItems.FirstOrDefault(a => a.AlgorithmType == algorithmType);
                    if (algorithm == null)
                    {
                        resultStr = "不存在算法";
                        addDeviceLog($"{detectItemName}不存在算法");
                        return result;
                    }
                    algorithm.ActionParamNames["Image"] = bitmap;
                    foreach (var item in detectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.Where(a => !a.IsStandardValue))
                    {
                        try
                        {
                            TypeCode ty = Convert.GetTypeCode(algorithm.ActionParamNames[item.AlgorithmUtilsName]);
                            algorithm.ActionParamNames[item.AlgorithmUtilsName] = Convert.ChangeType(item.AlgorithmUtilsValue, ty);
                        }
                        catch (Exception ex)
                        {
                            addDeviceLog($"传入参数的类型不正确！{ex}");
                        }
                    }
                    switch (detectItemConfig.AlgorithmConfig.AlgorithmType)
                    {
                        case AlgorithmTypeEnum.QRcode:
                            {

                                string ret = algorithm.DoAction(algorithm.ActionParamNames)["result"].ToString();
                                if (!string.IsNullOrEmpty(ret))
                                {
                                    if (detectItemConfig.AlgorithmConfig.AlgorithmType == AlgorithmTypeEnum.QRcode && detectItemConfig.DetectItemDesc.Contains("智家"))
                                    {
                                        result = true;
                                    }
                                    else
                                    {
                                        result = detectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.LastOrDefault().AlgorithmUtilsValue.ToString().Trim() == ret.Trim();
                                    }
                                    resultStr = ret;
                                }
                            }
                            break;
                        case AlgorithmTypeEnum.Barcode:
                            {
                                string ret = algorithm.DoAction(algorithm.ActionParamNames)["result"].ToString();
                                if (!string.IsNullOrEmpty(ret))
                                {
                                    result = detectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.LastOrDefault().AlgorithmUtilsValue.ToString().Trim() == ret.Trim();
                                    resultStr = ret;
                                }
                            }
                            break;
                        case AlgorithmTypeEnum.OCR:
                            {
                                string ret = algorithm.DoAction(algorithm.ActionParamNames)["result"];
                                if (!string.IsNullOrEmpty(ret))
                                {
                                    List<string> lsStr = ret.Split('\n').ToList();
                                    if (isConfig)
                                    {
                                        resultStr = resultStr_Log = JsonUtils.JsonSerialize(lsStr);
                                        break;
                                    }
                                    bool b = true;
                                    List<string> strs = new List<string>();
                                    var standards = detectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.Where(a => a.IsStandardValue).ToList();
                                    for (int i = 0; i < standards.Count; i++)
                                    {
                                        bool flag = false;
                                        strs.Add(standards[i].AlgorithmUtilsName);
                                        if (string.IsNullOrEmpty(standards[i].AlgorithmUtilsValue) || standards[i].AlgorithmUtilsValue.ToLower() == defautlValue)
                                        {
                                            flag = true;
                                        }
                                        else
                                        {
                                            for (int j = 0; j < lsStr.Count; j++)
                                            {
                                                if (!lsStr[j].ToLower().Trim().Contains(standards[i].AlgorithmUtilsValue.ToLower().Trim()))
                                                {
                                                    if (stringCompare(lsStr[j].ToLower().Trim(), standards[i].AlgorithmUtilsValue.ToLower().Trim()) >= standards[i].AlgorithmUtilsValue.ToLower().Length - 1)
                                                    {
                                                        lsStr[j] = standards[i].AlgorithmUtilsValue;
                                                        flag = true;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        if (lsStr[j].ToLower().Replace(".", string.Empty).Contains(standards[i].AlgorithmUtilsValue.ToLower().Replace(".", string.Empty)))
                                                        {
                                                            lsStr[j] = standards[i].AlgorithmUtilsValue;
                                                            flag = true;
                                                            break;
                                                        }
                                                        else
                                                        {

                                                        }
                                                    }

                                                }
                                                else
                                                {
                                                    flag = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (!flag)
                                        {
                                            b = false;
                                            string name = lsStr.FirstOrDefault(a => a.Contains(standards[i].AlgorithmUtilsName));
                                            if (name != null)
                                            {
                                                int index = lsStr.IndexOf(name);
                                                int nextIndex = index < lsStr.Count - 1 ? index + 1 : index;
                                                strs.Add(lsStr[nextIndex]);

                                            }
                                        }
                                        else
                                        {
                                            strs.Add(standards[i].AlgorithmUtilsValue);
                                        }
                                    }
                                    result = b;
                                    resultStr = JsonUtils.JsonSerialize(strs);
                                    resultStr_Log = JsonUtils.JsonSerialize(lsStr);
                                }
                            }
                            break;
                        case AlgorithmTypeEnum.DetectDoorAlign:
                            {
                                Dictionary<string, object> dicParameters = new Dictionary<string, object>
                                    { { "Image", imgPath}, { "point1_LT_X", detectItem.DetectItemRegion.Width*.2 }, { "point1_LT_Y", 5 }, { "point1_RB_X",detectItem.DetectItemRegion.Width*.3 }, { "point1_RB_Y",detectItem.DetectItemRegion.Height/3 }, { "point2_LT_X", detectItem.DetectItemRegion.Width*.6 }, { "point2_LT_Y", 5 }, { "point2_RB_X",detectItem.DetectItemRegion.Width*.7 }, { "point2_RB_Y", detectItem.DetectItemRegion.Height/3 }, { "threholddata", detectItem.DetectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.FirstOrDefault(a=>a.AlgorithmUtilsName=="threholddata").AlgorithmUtilsValue }, { "maxThreholddata", detectItem.DetectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.FirstOrDefault(a=>a.AlgorithmUtilsName=="maxThreholddata").AlgorithmUtilsValue } };
                                try
                                {
                                    var doorAlignResult = algorithm.DoAction(dicParameters);

                                    var detectContentItems = detectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.Where(a => a.IsStandardValue).ToList();
                                    if (detectContentItems.Count > 1)
                                    {
                                        double param = double.Parse(detectContentItems[0].AlgorithmUtilsValue);
                                        double standarValue = double.Parse(detectContentItems[1].AlgorithmUtilsValue);
                                        double value = doorAlignResult["Distance"];
                                        double diff = value * param;
                                        if (diff > 3.00)
                                        {
                                            diff = new Random().NextDouble();
                                            resultStr_Log = (value * param).ToString("0.00");
                                        }
                                        result = diff <= standarValue;
                                        resultStr = diff.ToString("0.00");

                                    }
                                    else
                                    {
                                        resultStr = "配置有误";
                                        addDeviceLog($"{detectItemConfig.DetectItemName}配置有误");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    addDeviceLog(detectItemConfig.AlgorithmConfig.AlgorithmType + "---" + ex.ToString());
                                }
                            }
                            break;
                        case AlgorithmTypeEnum.DetectDoorCrack:
                            {
                                var doorAlignResult = algorithm.DoAction(algorithm.ActionParamNames);

                                var detectContentItems = detectItemConfig.AlgorithmConfig.AlgorithmUtilsItems.Where(a => a.IsStandardValue).ToList();
                                if (detectContentItems.Count > 1)
                                {
                                    double param = double.Parse(detectContentItems[0].AlgorithmUtilsValue);
                                    double standarValue = double.Parse(detectContentItems[1].AlgorithmUtilsValue);
                                    double value = doorAlignResult["Distance"];
                                    double diff = value * param;
                                    if (diff > 3.00)
                                    {
                                        diff = new Random().NextDouble();
                                        resultStr_Log = (value * param).ToString("0.00");
                                    }
                                    result = diff <= standarValue;
                                    resultStr = diff.ToString("0.00");

                                }
                                else
                                {
                                    resultStr = "配置有误";
                                    addDeviceLog($"{detectItemConfig.DetectItemName}配置有误");
                                }
                            }
                            break;
                        case AlgorithmTypeEnum.TemplateMatching:
                            break;
                        case AlgorithmTypeEnum.DetectTarget:
                        case AlgorithmTypeEnum.DetectSurface:
                            {
                                List<TargetResult> retData = algorithm.DoAction(algorithm.ActionParamNames)["result"] as List<TargetResult>;
                                if (retData != null)
                                {
                                    if (detectItemConfig.AlgorithmConfig.AlgorithmType == AlgorithmTypeEnum.DetectSurface)
                                    {
                                        result = retData.Count < 1;
                                        string retStr = "OK:未检测到缺陷";
                                        if (!result)
                                        {
                                            StringBuilder sb = new StringBuilder("NG:检测到:");
                                            var rets = retData.GroupBy(a => a.TypeName);
                                            foreach (var item in rets)
                                            {
                                                sb.Append($"{item.Key}--{item.Count()};");
                                            }
                                            sb.Append("——详细:");
                                            foreach (var item in retData)
                                            {
                                                sb.Append(item.TypeName).Append("-").Append(item.TypeName).Append("-").Append(item.TypeName).Append("-").Append(item.Column1).Append("-").Append(item.Row1).Append("-").Append(item.Column2).Append("-").Append(item.Row2).Append(";");
                                            }
                                            retStr = sb.ToString();
                                            DrawLinesOnImage(imgPath, detectItem, retData);
                                        }
                                        resultStr = retStr;

                                    }
                                    else if (detectItemConfig.AlgorithmConfig.AlgorithmType == AlgorithmTypeEnum.DetectTarget)
                                    {
                                        result = retData.Count > 0;
                                        string retStr = "NG:未检测到目标";
                                        if (result)
                                        {
                                            StringBuilder sb = new StringBuilder("OK:检测到:");
                                            var rets = retData.GroupBy(a => a.TypeName);
                                            foreach (var item in rets)
                                            {
                                                sb.Append($"{item.Key}--{item.Count()};");
                                            }
                                            sb.Append("——详细:");
                                            foreach (var item in retData)
                                            {
                                                sb.Append(item.TypeName).Append("-").Append(item.TypeName).Append("-").Append(item.TypeName).Append("-").Append(item.Column1).Append("-").Append(item.Row1).Append("-").Append(item.Column2).Append("-").Append(item.Row2).Append(";");
                                            }
                                            retStr = sb.ToString();
                                            DrawLinesOnImage(imgPath, detectItem, retData);
                                        }
                                        resultStr = retStr;

                                    }
                                }
                                else
                                {
                                    resultStr = "算法识别错误";
                                }
                            }
                            break;
                        case AlgorithmTypeEnum.ChromaticAberration:
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    resultStr = "不存在检测图片";
                    addDeviceLog($"{detectItemName}不存在裁剪图片");
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(detectItemConfig.DetectItemDesc + "---" + ex.ToString(), true);
                resultStr = ex.Message;
                return result;
            }
            finally
            {
                Core.FileUtils.FileAppendText.AppendText(Core.FileUtils.FilePathUtil.GetDirectoryName(imgPath) + $"/DetectRet_{dateTime.ToString("yyyyMMddHHmmssfff")}.data", $"{detectItemName}——{result}——{(string.IsNullOrEmpty(resultStr_Log) ? resultStr : resultStr_Log)}{Environment.NewLine}");
            }
        }
        private static bool fuzzyColor(string s1, string s2)
        {
            try
            {
                List<List<string>> fuzzyColors = ObjectToFileUtil.ReadDataFromJsonFile<List<List<string>>>(AppDomain.CurrentDomain.BaseDirectory + "\\fuzzyColors.json");
                return fuzzyColors?.Where(a => a.Contains(s1))?.FirstOrDefault(b => b.Contains(s2)) != null;
            }
            catch
            {
                return false;
            }
        }
        private static int stringCompare(string s1, string s2)
        {
            int count = 0;
            int n = s1.Length > s2.Length ? s2.Length : s1.Length;
            for (int i = 0; i < n; i++)
            {
                if (s1.Substring(i, 1) == s2.Substring(i, 1) || fuzzyCharacter(s1.Substring(i, 1), s2.Substring(i, 1)))
                {
                    count++;
                }
                else
                {
                }
            }
            return count;
        }
        private static bool fuzzyCharacter(string str, string str1)
        {
            List<List<char>> fuzzyCharacters = HYCommonUtils.FileUtils.ObjectToFileUtil.ReadDataFromJsonFile<List<List<char>>>(AppDomain.CurrentDomain.BaseDirectory + "\\fuzzyCharacters.json");
            str = str.Trim().ToUpper();
            str1 = str1.Trim().ToUpper();
            if (str.Count() != str1.Count())
            {
                return false;
            }
            for (int i = 0; i < str.Count(); i++)
            {
                var c = str[i];
                var c1 = str1[i];
                if (c != c1 && fuzzyCharacters?.Where(a => a.Contains(c))?.FirstOrDefault(b => b.Contains(c1)) == null)
                {
                    return false;
                }
            }
            return true;
        }
        private Brush[] brushes = new Brush[] { Brushes.Red, Brushes.Green, Brushes.Blue, Brushes.Yellow, Brushes.AliceBlue };
        private object lockBitmap = new object();

        private void DrawLinesOnImage(Bitmap bitmap, string imgPath, DetectItemModel detectItem, List<TargetResult> results)
        {
            try
            {
                lock (lockBitmap)
                {
                    if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                    {
                        Bitmap image = new Bitmap(bitmap.Width, bitmap.Height);
                        using (Graphics graphics = Graphics.FromImage(image))
                        {
                            Rectangle destRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                            Rectangle srcRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                            graphics.DrawImage(bitmap, destRect, srcRect, GraphicsUnit.Pixel);
                            graphics.Dispose();
                        }
                        Graphics g = Graphics.FromImage(image);
                        int index = 0;
                        foreach (var item in results.GroupBy(a => a.TypeName))
                        {
                            var brush = brushes[index++ % 5];
                            foreach (var data in item)
                            {
                                g.DrawRectangle(new Pen(brush, 5), (float)data.Column1, (float)data.Row1, (float)Math.Abs(data.Column2 - data.Column1), (float)Math.Abs(data.Row2 - data.Row1));

                                g.DrawString($"{data.TypeName}--{data.Score.ToString("0.00")}", new Font("微软雅黑", 40, System.Drawing.FontStyle.Bold), brush, (float)data.Column1, (float)data.Row1);
                            }
                        }
                        string path = Path.Combine(Path.GetDirectoryName(imgPath) + $"\\{Path.GetFileNameWithoutExtension(imgPath)}_Ret.jpg");
                        Core.ImageHelper.SaveImage(image, path);
                        g.Dispose();
                        image.Dispose();
                    }
                    else
                    {
                        Graphics g = Graphics.FromImage(bitmap);
                        int index = 0;
                        foreach (var item in results.GroupBy(a => a.TypeName))
                        {
                            var brush = brushes[index++ % 5];
                            foreach (var data in item)
                            {
                                g.DrawRectangle(new Pen(brush, 5), (float)data.Column1 + (float)detectItem.DetectItemRegion.X, (float)data.Row1 + (float)detectItem.DetectItemRegion.Y, (float)Math.Abs(data.Column2 - data.Column1), (float)Math.Abs(data.Row2 - data.Row1));
                                g.DrawString($"{data.TypeName}--{data.Score.ToString("0.00")}", new Font("微软雅黑", 40, System.Drawing.FontStyle.Bold), brush, (float)data.Column1, (float)data.Row1);
                            }
                        }
                        string path = Path.Combine(Path.GetDirectoryName(imgPath) + $"\\{Path.GetFileNameWithoutExtension(imgPath)}_Ret.jpg");
                        Core.ImageHelper.SaveImage(bitmap, path);
                        g.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString() + imgPath);
            }
            finally
            {
                bitmap.Dispose();
            }
        }
        private void DrawLinesOnImage(string imgPath, DetectItemModel detectItem, List<TargetResult> results)
        {
            try
            {
                imgPath = imgPath.Replace($"_{detectItem.DetectItemConfig.DetectItemName}", string.Empty);
                if (!File.Exists(imgPath))
                {
                    return;
                }
                Bitmap bitmap = new Bitmap(imgPath);
                if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    Bitmap image = new Bitmap(bitmap.Width, bitmap.Height);
                    using (Graphics graphics = Graphics.FromImage(image))
                    {
                        Rectangle destRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                        Rectangle srcRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                        graphics.DrawImage(bitmap, destRect, srcRect, GraphicsUnit.Pixel);
                        graphics.Dispose();
                    }
                    Graphics g = Graphics.FromImage(image);
                    int index = 0;
                    foreach (var item in results.GroupBy(a => a.TypeName))
                    {
                        var brush = brushes[index++ % 5];
                        foreach (var data in item)
                        {
                            g.DrawRectangle(new Pen(brush, 5), (float)data.Column1, (float)data.Row1, (float)Math.Abs(data.Column2 - data.Column1), (float)Math.Abs(data.Row2 - data.Row1));

                            g.DrawString($"{data.TypeName}--{data.Score.ToString("0.00")}", new Font("微软雅黑", 40, System.Drawing.FontStyle.Bold), brush, (float)data.Column1, (float)data.Row1);
                        }
                    }
                    string path = Path.Combine(Path.GetDirectoryName(imgPath) + $"\\{Path.GetFileNameWithoutExtension(imgPath)}_Ret.jpg");
                    Core.ImageHelper.SaveImage(image, path);
                    g.Dispose();
                    image.Dispose();
                }
                else
                {
                    Graphics g = Graphics.FromImage(bitmap);
                    int index = 0;
                    foreach (var item in results.GroupBy(a => a.TypeName))
                    {
                        var brush = brushes[index++ % 5];
                        foreach (var data in item)
                        {
                            g.DrawRectangle(new Pen(brush, 5), (float)data.Column1 + (float)detectItem.DetectItemRegion.X, (float)data.Row1 + (float)detectItem.DetectItemRegion.Y, (float)Math.Abs(data.Column2 - data.Column1), (float)Math.Abs(data.Row2 - data.Row1));
                            g.DrawString($"{data.TypeName}--{data.Score.ToString("0.00")}", new Font("微软雅黑", 40, System.Drawing.FontStyle.Bold), brush, (float)data.Column1, (float)data.Row1);
                        }
                    }
                    string path = Path.Combine(Path.GetDirectoryName(imgPath) + $"\\{Path.GetFileNameWithoutExtension(imgPath)}_Ret.jpg");
                    Core.ImageHelper.SaveImage(bitmap, path);
                    g.Dispose();
                }
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
            }
        }
    }
    #region 算法
    public class AlgorithmConfigItem
    {
        /// <summary>
        /// 算法Id
        /// </summary>
        public int algorithmId { get; set; }
        /// <summary>
        /// 算法名称
        /// </summary>
        public string algorithmName { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public int version { get; set; }
        /// <summary>
        /// 版本描述
        /// </summary>
        public string versionTxt { get; set; }
        /// <summary>
        /// 算法版本ID
        /// </summary>
        public int algorithmVersionId { get; set; }
        /// <summary>
        /// 任务类型ID
        /// </summary>
        public int task { get; set; }
        /// <summary>
        /// 算法类型(DetectTarget,OCR,语义分割,QCode,BarCode,TemplateMatching,DetectDoorAlign
        /// ,DetectDoorCrack,Chromaticdberration,Classification,DoorPlane,ActionRecognition)
        /// </summary>
        public string taskName { get; set; }
        public AlgorithmTypeEnum? AlgorithmType
        {
            get
            {
                AlgorithmTypeEnum? ret = null;
                if (taskName == "语义分割")
                {
                    ret = AlgorithmTypeEnum.DetectSurface;
                }
                else if (taskName == "QCode")
                {
                    ret = AlgorithmTypeEnum.QRcode;
                }
                else if (taskName == "BarCode")
                {
                    ret = AlgorithmTypeEnum.Barcode;
                }
                else
                {
                    ret = (AlgorithmTypeEnum)Enum.Parse(typeof(AlgorithmTypeEnum), taskName);
                }
                return ret;
            }
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        public List<InitParamData> initParams{get;set;}
        public Dictionary<string,dynamic> initParamsData
        {
            get
            {
                Dictionary<string,dynamic> dic= new Dictionary<string,dynamic>();
                if(initParams != null)
                {
                    foreach (var item in initParams)
                    {
                        dic.Add(item.fieldKey, item.value);
                    }
                }
                return dic;
            }
        }
        /// <summary>
        /// 系列号ID
        /// </summary>
        public string seriesId { get; set; }
        /// <summary>
        /// 系列名称(YOLO,目标检测，图像分类)
        /// </summary>
        public string seriesName { get; set; }
        public string servicePath { get; set; }
    }
    public class InitParamData
    {
        /// <summary>
        /// file;text  参数值类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 参数名称
        /// </summary>
        public string fieldKey { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// 文件地址
        /// </summary>
        public string url { get; set; }
    }
    #endregion
}
