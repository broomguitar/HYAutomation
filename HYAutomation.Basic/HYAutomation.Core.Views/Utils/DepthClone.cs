using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace HYAutomation.Core.Views.Utils
{
    public class Clone
    {
        /// <summary>
        /// 深克隆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T DepthClone<T>(T t)
        {
            T clone = default(T);
            using (Stream stream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                try
                {
                    formatter.Serialize(stream, t);
                    stream.Seek(0, SeekOrigin.Begin);
                    clone = (T)formatter.Deserialize(stream);
                }
                catch (SerializationException e)
                {
                    LogHelper.Instance.AddLog("Failed to serialize. Reason: " + e.Message);
                }
            }
            return clone;
        }
    }
}
