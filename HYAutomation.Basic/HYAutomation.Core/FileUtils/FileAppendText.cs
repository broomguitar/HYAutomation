using System;
using System.IO;
using System.Threading;

namespace HYAutomation.Core.FileUtils
{
    public class FileAppendText
    {
        private readonly static ReaderWriterLockSlim _logWriteLock = new ReaderWriterLockSlim();
        public static void AppendText(string path, string content)
        {
            try
            {
                _logWriteLock.EnterWriteLock();
                File.AppendAllText(path, content);
            }
            catch (Exception ex)
            {

                LogHelper.Instance.AddLog(ex.ToString(), true);
            }
            finally
            {
                _logWriteLock.ExitWriteLock();
            }

        }
    }
}
