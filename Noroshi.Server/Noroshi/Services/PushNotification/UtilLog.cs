using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Xml.Serialization;

namespace PushNotification
{
    class UtilLog
    {
        private static string settingsFilePath;

        public UtilLog()
        {
        }

        /// <summary>
        /// ログファイルへ出力
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="msg"></param>
        public static void ConsoleLog(StreamWriter sw, string msg)
        {
            Console.SetOut(sw);
            Console.WriteLine(msg);
            sw.Flush();
        }

        /// <summary>
        /// ログ取得
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="respMessage"></param>
        public static void getLog(StreamWriter sw, string respMessage)
        {
            try
            {
                ConsoleLog(sw, respMessage);
            }
            catch (Exception ex)
            {
                ConsoleLog(sw, "Error: " + ex.Message);
            }
        }

        /// <summary>
        /// 設定FileDirectory取得
        /// </summary>
        /// <returns>PushSettingsDirectory</returns>
        public static string GetPushSettingsFileDirectory()
        {
            settingsFilePath = HttpContext.Current.Server.MapPath("~");
            //settingsFilePath = System.Environment.CurrentDirectory;
            Debug.WriteLine("PushSettingsPath: " + settingsFilePath);

            string[] dirArraryData = settingsFilePath.Split('\\');
            string _alldirectory = "";

            foreach (string drData in dirArraryData)
            {
                _alldirectory += drData + "\\";
                if (drData == "Noroshi.Server")
                {
                    break;
                }
            }
            return _alldirectory + "PushSettings" + "\\";
        }

        /// <summary>
        /// 設定ファイル内容取得
        /// </summary>
        public static SettingsModel GetSettingsInfo()
        {
            string settingsFilePath = GetPushSettingsFileDirectory() + "settings.xml";

            FileStream fs = new FileStream(settingsFilePath, 
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            XmlSerializer serializer = new XmlSerializer(typeof(SettingsModel));
            SettingsModel settings = (SettingsModel)serializer.Deserialize(fs);

            return settings;
        }
        
    }
}
