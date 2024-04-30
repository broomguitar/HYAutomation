using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace HYAutomation.Core
{
    public class ConfigManagerUtil
    {
        private static readonly object _lockObj = new object();
        public static ConfigManagerUtil GetInstance(System.Reflection.Assembly assembly)
        {

            return new ConfigManagerUtil(assembly.Location.ToString() + ".config");
        }
        private ConfigManagerUtil(string configPath)
        {
            ConfigPath = configPath;
            if (string.IsNullOrEmpty(ConfigPath))
            {
                ConfigPath = System.Reflection.Assembly.GetExecutingAssembly().Location.ToString() + ".config";
            }
        }

        public string ConfigPath { get; }
        public Configuration Config => ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap()
        {
            ExeConfigFilename = ConfigPath
        }, ConfigurationUserLevel.None);

        /// <summary>
        /// 读取配置文件值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            try
            {

                if (Config.AppSettings.Settings.AllKeys.Contains(key))
                {
                    return Config.AppSettings.Settings[key].Value;
                }
                return string.Empty;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 向配置文件某节点下写入数据（默认 appSettings）
        /// </summary>
        /// <param name="pairs"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool SaveValue(Dictionary<string, string> pairs, string section = "appSettings")
        {
            try
            {
                foreach (var item in pairs)
                {
                    if (!Config.AppSettings.Settings.AllKeys.Contains(item.Key))
                    {
                        Config.AppSettings.Settings.Add(item.Key, item.Value);
                    }
                    else
                    {
                        Config.AppSettings.Settings[item.Key].Value = item.Value;
                    }
                }
                Config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(section);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
