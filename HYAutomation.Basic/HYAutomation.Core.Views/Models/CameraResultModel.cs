using HYAutomation.Core.Algorithm.Models;
using HYWindowUtils.WPF.VMBaseUtil;
using System.Collections.Generic;
using System.Windows.Media;

namespace HYAutomation.Core.Views.Models
{
    public class CameraResultModel : NotifyPropertyObject
    {
        private string _cameraName;

        public string CameraName
        {
            get { return _cameraName; }
            set { _cameraName = value; RaisePropertyChanged(); }
        }
        private string _cameraDesc;

        public string CameraDesc
        {
            get { return _cameraDesc; }
            set { _cameraDesc = value; RaisePropertyChanged(); }
        }
        public string CameraImagePath { get; set; }
        private ImageSource _cameraImage;
        [Newtonsoft.Json.JsonIgnore]
        /// <summary>
        /// 相机图片
        /// </summary>
        public ImageSource CameraImage
        {
            get
            {
                return _cameraImage;
            }
            set
            {
                if (_cameraImage != null)
                {
                    _cameraImage = null;
                }
                _cameraImage = value;
                RaisePropertyChanged();
            }
        }
        private bool? _cameraRetIsOK;

        public bool? CameraRetIsOK
        {
            get { return _cameraRetIsOK; }
            set { _cameraRetIsOK = value; RaisePropertyChanged(); }
        }
        private IEnumerable<DetectItemModel> _detectItems = new List<DetectItemModel>();

        public IEnumerable<DetectItemModel> DetectItems
        {
            get { return _detectItems; }
            set { _detectItems = value; RaisePropertyChanged(); }
        }
    }
    public class CameraResultModel<T> : CameraResultModel
    {
        public T Data { get; set; }
    }
}
