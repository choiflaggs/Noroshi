using System;

namespace PushNotification
{
    [System.Xml.Serialization.XmlRoot("PushSettings")]
    public class SettingsModel
    {
        [System.Xml.Serialization.XmlElement("AppleCertFileName")]
        public string AppleCertFileName { get; set; }

        [System.Xml.Serialization.XmlElement("AppleCertPassword")]
        public string AppleCertPassword { get; set; }

        [System.Xml.Serialization.XmlElement("AndroidSenderID")]
        public string AndroidSenderID { get; set; }

        [System.Xml.Serialization.XmlElement("AndroidAPIKey")]
        public string AndroidAPIKey { get; set; }

        [System.Xml.Serialization.XmlElement("AndroidAppPackageName")]
        public string AndroidAppPackageName { get; set; }
    }
}