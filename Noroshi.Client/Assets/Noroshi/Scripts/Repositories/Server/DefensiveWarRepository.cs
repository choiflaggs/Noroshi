using System;
using Noroshi.Core.WebApi.Response;
using UniRx;
using UniLinq;


namespace Noroshi.Repositories.Server
{
    public class DefensiveWarRepository : MasterDataRepository<DefensiveWar>
    {

        public IObservable<DefensiveWar> Get(uint battleId, DateTime startDay, DateTime endDay)
        {
            return LoadAll().Select(ts => ts.Where(t => t.BattleID == battleId && t.StartDay == startDay && t.EndDay == endDay).FirstOrDefault());
        }
        public override IObservable<DefensiveWar[]> GetMulti(uint[] ids)
        {
            return LoadAll().Select(ts =>
            {
                var map = ts.ToDictionary(t => t.BattleID);
                return ids.Select(id => map[id]).ToArray();
            });
        }



        protected override string _url()
        {
            return base._url() + "DefensiveWar/MasterData";
        }
    }
}
