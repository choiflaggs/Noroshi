using System;
using System.IO;
using MsgPack.Serialization;
using LightNode.Core;
using LightNode.Formatter;
using Noroshi.Server.Security;

namespace Noroshi.Server.Formatter
{
    public class DefaultFormatter : ContentFormatterBase
    {
        readonly SerializationContext _serializationContext;

        public DefaultFormatter(string mediaType = "application/octet-stream", string ext = "")
            : this(SerializationContext.Default, mediaType, ext)
        {
        }

        public DefaultFormatter(SerializationContext serializationContext, string mediaType = "application/octet-stream", string ext = "")
            : base(mediaType, ext, null)
        {
            _serializationContext = serializationContext;
        }

        public override void Serialize(Stream stream, object obj)
        {
            /*
            using (var packer = MsgPack.Packer.Create(stream))
            {
                var serializer = serializationContext.GetSerializer(obj.GetType());
                serializer.PackTo(packer, obj);
            }
            */
            
            var memoryStream = new MemoryStream();
            var serializer = _serializationContext.GetSerializer(obj.GetType());
            serializer.Pack(memoryStream, obj);
            var bytes = Cryption.Encrypt(memoryStream.GetBuffer());
            if (bytes != null)
            {
                stream.Write(bytes, 0, bytes.Length);
                return;
            }
            throw new InvalidOperationException();
        }

        public override object Deserialize(Type type, Stream stream)
        {
            using (var packer = MsgPack.Unpacker.Create(stream))
            {
                if (!packer.Read()) throw SerializationExceptions.NewUnexpectedEndOfStream();

                var serializer = _serializationContext.GetSerializer(type);
                return serializer.UnpackFrom(packer);
            }
        }
    }

    public class DefaultContentFormatterFactory : IContentFormatterFactory
    {
        public IContentFormatter CreateFormatter()
        {
            return new DefaultFormatter();
        }
    }
}