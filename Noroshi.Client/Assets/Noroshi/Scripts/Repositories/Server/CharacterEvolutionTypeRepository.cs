using Noroshi.Core.WebApi.Response.Character;
using UniLinq;
using UniRx;


namespace Noroshi.Repositories.Server
{
    public class CharacterEvolutionTypeRepository : MasterDataRepository<CharacterEvolutionType>
    {
        public IObservable<CharacterEvolutionType> GetByTypeAndLevel(ushort type, ushort level)
        {
            return LoadAll().Select(ts => ts.FirstOrDefault(t => t.Type == type && t.EvolutionLevel == level));
        }


        public IObservable<CharacterEvolutionType[]> GetByType(ushort type)
        {
            return LoadAll().Select(ts => ts.Where(t => t.Type == type).ToArray());
        }
        protected override string _url()
        {
            return base._url() + "CharacterEvolutionType/MasterData";
        }
    }
}