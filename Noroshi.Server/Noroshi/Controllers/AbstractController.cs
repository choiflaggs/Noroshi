using System.Linq;
using System.IO;
using System.Threading.Tasks;
using MsgPack.Serialization;
using LightNode.Server;
using Newtonsoft.Json;
using Noroshi.Server.Security;


namespace Noroshi.Server.Controllers
{
    public abstract class AbstractController : LightNodeContract
    {
        // ‹¤’Ê‹@”\‚Æ‚µ‚Ä‚Í”pŽ~—\’è
        protected T _getRequest<T>() where T : class
        {
            // Take raw stream
            var body = this.Environment["owin.RequestBody"] as Stream;
            byte[] bodyBytes;
            using (var ms = new MemoryStream())
            {
                body.CopyTo(ms);
                bodyBytes = ms.ToArray();
            }
            //var stream = new MemoryStream(Cryption.Dencrypt(bodyBytes));
            //var serializer = MessagePackSerializer.Get<T>();
            //return serializer.Unpack(stream);
            var text = System.Text.Encoding.UTF8.GetString(bodyBytes);
            return JsonConvert.DeserializeObject<T>(text); //serializer.Unpack(stream);

        }
    }
}