using SpeechLib;
using System;
using System.Threading.Tasks;

namespace HYAutomation.Core
{
    public class SpeakerHelper
    {
        static SpVoice voice = new SpVoice();
        private static bool isSpeaking = false;
        /// <summary>
        /// 朗读文本
        /// </summary>
        /// <param name="obj_Message"></param>
        public static void Speak_Voice(object obj_Message, int times = 0)
        {
            try
            {
                isSpeaking = true;
                Task.Run(() =>
                {
                    if (times == 0)
                    {
                        do
                        {
                            string str_Message = obj_Message.ToString();
                            voice.Speak(str_Message);
                            System.Threading.Thread.Sleep(500);
                        }
                        while (isSpeaking);
                    }
                    else
                    {
                        do
                        {
                            string str_Message = obj_Message.ToString();
                            voice.Speak(str_Message);
                            System.Threading.Thread.Sleep(500);
                        }
                        while (isSpeaking && --times > 0);
                    }
                });
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog("=============>朗读失败" + ex, true);
            }
        }
        public static void StopSpeak_Voice()
        {
            isSpeaking = false;
        }
    }
}
