using System;

namespace HYAutomation.Core.FileUtils
{
    public class FilePathUtil
    {
        public static string NormalizePath(string path)
        {
            try
            {
                return path?.Replace('\\', '/').Trim();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString());
                return path;
            }
        }
        public static string RemoveChineseCharacter(string src)
        {
            if (string.IsNullOrEmpty(src))
            {
                return string.Empty;
            }
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"[\u4e00-\u9fa5]"
            );
            return regex.Replace(src, string.Empty);
        }
        public static string GetDirectoryName(string filePath)
        {
            return NormalizePath(System.IO.Path.GetDirectoryName(filePath));
        }
    }
}
