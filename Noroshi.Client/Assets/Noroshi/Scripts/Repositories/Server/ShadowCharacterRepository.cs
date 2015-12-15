using UniRx;
using UniLinq;
//using MsgPack.Serialization;
using Noroshi.Core.WebApi.Response.Character;


namespace Noroshi.Repositories.Server
{
    public class ShadowCharacterRepository : MasterDataRepository<ShadowCharacter>
    {

        public override IObservable<ShadowCharacter> Get(uint id)
        {
            return LoadAll().Select(ts => ts.FirstOrDefault(t => t.ID == id));
        }
        public override IObservable<ShadowCharacter[]> GetMulti(uint[] ids)
        {
            return LoadAll().Select(ts =>
            {
                var map = ts.ToDictionary(t => t.ID);
                return ids.Select(id => map[id]).ToArray();
            });
        }

//        protected override ShadowCharacter[] UnpackBytes(byte[] bytes)
//        {
//            var stream = new MemoryStream(Cryption.Dencrypt(bytes));
//            var serializer = MessagePackSerializer.Get<Noroshi.Datas.Server.ShadowCharacter[]>();
//            var serverData = serializer.Unpack(stream);
//            var tmpData = new List<ShadowCharacter>();
//            serverData.ToList().ForEach(data =>
//            {
//                var tmp = new ShadowCharacter();
//                tmp.ID = data.ID;
//                tmp.CharacterID = data.CharacterID;
//                tmp.Level = data.Level;
//                tmp.PromotionLevel = data.PromotionLevel;
//                tmp.EvolutionLevel = data.EvolutionLevel;
//                tmp.ActionLevel1 = data.ActionLevel1;
//                tmp.ActionLevel2 = data.ActionLevel2;
//                tmp.ActionLevel3 = data.ActionLevel3;
//                tmp.ActionLevel4 = data.ActionLevel4;
//                tmp.ActionLevel5 = data.ActionLevel5;
//                tmp.GearIDs = new uint[]{};
//                tmpData.Add(tmp);
//            });
//
//            return tmpData.ToArray();
//        }



        protected override string _url()
        {
            return base._url() + "ShadowCharacter/MasterData";
        }
    }
}
