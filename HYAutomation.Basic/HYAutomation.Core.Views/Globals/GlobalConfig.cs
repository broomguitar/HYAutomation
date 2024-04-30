namespace HYAutomation.Core.Views
{
    public class GlobalConfig
    {
        private static readonly object _lockObj = new object();
        private static GlobalConfig _instance;
        public static GlobalConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new GlobalConfig();
                        }
                    }
                }
                return _instance;
            }
        }
        private GlobalConfig() { }
        public static ConfigManagerUtil ConfigManager = ConfigManagerUtil.GetInstance(System.Reflection.Assembly.GetExecutingAssembly());
        public bool IsAutoStart
        {
            get
            {
                try
                {
                    return ConfigManager.GetValue("IsAutoStart") == "1";
                }
                catch
                {

                    return false;
                }
            }
        }
        #region 存储路径
        public string CacheFolder => ConfigManager.GetValue("CacheFolder");
        #endregion
        #region JpegQuality
        public long JpegQuality
        {
            get
            {
                string qualityStr = ConfigManager.GetValue("JpegQuality");
                if (!long.TryParse(qualityStr, out long quality))
                {
                    quality = 75L;
                }
                return quality;
            }
        }
        public int CutImgJpegQuality
        {
            get
            {
                string qualityStr = ConfigManager.GetValue("CutImgJpegQuality");
                if (!int.TryParse(qualityStr, out int quality))
                {
                    quality = 75;
                }
                return quality;
            }
        }
        #endregion

    }
}
