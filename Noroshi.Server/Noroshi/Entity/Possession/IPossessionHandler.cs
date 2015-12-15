using System.Collections.Generic;

namespace Noroshi.Server.Entity.Possession
{
    public interface IPossessionHandler
    {
        void Load();

        IPossessionObject GetPossessionObject(uint id, uint num);

        uint GetMaxNum(uint id);
        uint GetCurrentNum(uint id);

        bool CanAdd(IEnumerable<IPossessionParam> possessionParams);
        IEnumerable<IPossessionObject> Add(IEnumerable<IPossessionParam> possessionParams);

        bool CanRemove(IEnumerable<IPossessionParam> possessableParamst);
        IEnumerable<IPossessionObject> Remove(IEnumerable<IPossessionParam> possessionParams);
    }
}
