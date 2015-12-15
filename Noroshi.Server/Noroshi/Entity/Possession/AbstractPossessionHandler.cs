using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Possession;

namespace Noroshi.Server.Entity.Possession
{
    public abstract class AbstractPossessionHandler<T> : IPossessionHandler
        where T : IPossessionObject
    {
        public readonly uint PlayerID;
        readonly IEnumerable<uint> _candidateIds;
        bool _hasAlreadyLoadedBeforeUpdate = false;

        public AbstractPossessionHandler(uint playerId, IEnumerable<uint> candidateIds)
        {
            PlayerID = playerId;
            _candidateIds = candidateIds.Distinct();
        }

        public abstract PossessionCategory PossessionCategory { get; }

        public abstract IPossessionObject GetPossessionObject(uint id, uint num);
        public abstract uint GetMaxNum(uint id);
        public abstract uint GetCurrentNum(uint id);

        public void Load()
        {
            _load(_candidateIds, false);
        }
        protected void _loadBeforeUpdate()
        {
            if (_hasAlreadyLoadedBeforeUpdate) return;
            _load(_candidateIds, true);
            _hasAlreadyLoadedBeforeUpdate = true;
        }
        protected abstract void _load(IEnumerable<uint> candidateIds, bool beforeUpdate);

        public bool CanAdd(IEnumerable<IPossessionParam> possessionParams)
        {
            // 付与対象が何かしらあれば許可してしまう。
            return _sanitizeParams(possessionParams).Any(pp => _getAddNum(pp) > 0);
        }
        public IEnumerable<IPossessionObject> Add(IEnumerable<IPossessionParam> possessionParams)
        {
            _loadBeforeUpdate();

            var fixedPossessableParams = _sanitizeParams(possessionParams).Cast<IPossessionParam>();
            if (fixedPossessableParams.Count() > 0)
            {
                if (!_add(fixedPossessableParams))
                {
                    throw new Exception("Fail To Add PossessionObject");
                }
            }
            return fixedPossessableParams.Select(pp => GetPossessionObject(pp.ID, pp.Num));
        }
        uint _getAddNum(IPossessionParam possessionParams)
        {
            var currentNum = GetCurrentNum(possessionParams.ID);
            var maxNum = GetMaxNum(possessionParams.ID);
            return currentNum + possessionParams.Num > maxNum ? maxNum - currentNum : possessionParams.Num;
        }
        protected abstract bool _add(IEnumerable<IPossessionParam> possessionParams);

        public bool CanRemove(IEnumerable<IPossessionParam> possessionParams)
        {
            return _sanitizeParams(possessionParams).All(pp => _getRemoveNum(pp) > 0);
        }
        public IEnumerable<IPossessionObject> Remove(IEnumerable<IPossessionParam> possessionParams)
        {
            _loadBeforeUpdate();

            var fixedPossessableParams = _sanitizeParams(possessionParams).Cast<IPossessionParam>();
            if (fixedPossessableParams.Count() > 0)
            {
                if (!_remove(fixedPossessableParams))
                {
                    throw new Exception("Fail To Remove PossessableObject");
                }
            }
            return fixedPossessableParams.Select(pp => GetPossessionObject(pp.ID, pp.Num));
        }
        uint _getRemoveNum(IPossessionParam possessionParams)
        {
            var currentNum = GetCurrentNum(possessionParams.ID);
            return possessionParams.Num <= currentNum ? possessionParams.Num : 0;
        }
        protected abstract bool _remove(IEnumerable<IPossessionParam> possessionParams);

        IEnumerable<PossessionParam> _sanitizeParams(IEnumerable<IPossessionParam> possessionParams)
        {
            return possessionParams
                // カテゴリフィルタリング
                .Where(pp => pp.Category == PossessionCategory)
                // ID毎にまとめる
                .ToLookup(pp => pp.ID).Select(grouping => new PossessionParam
                {
                    Category = PossessionCategory,
                    ID = grouping.Key,
                    Num = (uint)grouping.Sum(pp => pp.Num),
                })
                // 数補正
                .Select(pp => new PossessionParam
                {
                    Category = pp.Category,
                    ID = pp.ID,
                    Num = _getAddNum(pp),
                })
                // 数フィルタリング
                .Where(pp => pp.Num > 0);
        }
    }
}
