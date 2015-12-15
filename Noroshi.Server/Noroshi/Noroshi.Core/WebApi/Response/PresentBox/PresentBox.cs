using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.PresentBox
{
    public class PresentBox
    {
        public uint ID { get; set; }
        public PossessionObject[] PossessionObjects { get; set; }
        public string TextID { get; set; }
        public string[] TextParams { get; set; }
        public uint CreatedAt { get; set; }
    }
}
