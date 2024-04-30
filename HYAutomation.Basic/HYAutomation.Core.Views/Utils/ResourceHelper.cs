using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows;

namespace HYAutomation.Core.Views.Utils
{
    public static class ResourceHelper
    {
        private static string[] resourceNames;
        private static ResourceSet set;
        private static List<DictionaryEntry> Datas;
        /// <summary>
        /// 是否存在图片资源
        /// </summary>
        /// <param name="imgResourceName"></param>
        /// <returns></returns>
        internal static bool ResourceExists(string imgResourceName)
        {
            if (Datas == null)
            {
                if (resourceNames == null)
                {
                    resourceNames =
                        Assembly.GetExecutingAssembly().GetManifestResourceNames();
                }
                if (set == null)
                {
                    set = new ResourceSet(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceNames[0]));
                }
                Datas = set.OfType<DictionaryEntry>().ToList();
            }
            return Datas.Exists(a => string.Equals(a.Key.ToString(), imgResourceName, StringComparison.OrdinalIgnoreCase));
        }
        /// <summary>
        /// 获取资源值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static object GetResourceValue(string key)
        {
            if (Application.Current.Resources.Contains(key))
            {
                return Application.Current.Resources[key];
            }
            return null;
        }
    }
}
