using System;
using System.IO;
using MsgPack.Serialization;
using LightNode.Formatter;
using Newtonsoft.Json;
using Noroshi.Server.Security;

namespace Noroshi.Server.Formatter
{
    public class DefaultJsonFormatter : ContentFormatterBase
    {
        readonly SerializationContext serializationContext;

        public DefaultJsonFormatter(string mediaType = "application/octet-stream", string ext = "")
            : this(SerializationContext.Default, mediaType, ext)
        {
        }

        public DefaultJsonFormatter(SerializationContext serializationContext, string mediaType = "application/octet-stream", string ext = "")
            : base(mediaType, ext, null)
        {
            this.serializationContext = serializationContext;
        }

        public override void Serialize(Stream stream, object obj)
        {
            if (obj == null)
            {
                return;
            }
            var jsonData = JsonConvert.SerializeObject(obj);
            var bytes = Cryption.Encrypt(System.Text.Encoding.UTF8.GetBytes(jsonData));

            stream.Write(bytes, 0, bytes.Length);
        }

        public override object Deserialize(Type type, Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            var text = System.Text.Encoding.UTF8.GetString(Cryption.Dencrypt(bytes));
            return JsonConvert.DeserializeObject(text);
        }
    }
}