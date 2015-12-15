using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Entity
{
    public abstract class AbstractLevelExpEntity<TEntity, TDao, TPrimaryKey, TRecord> : AbstractDaoWrapperEntity<TEntity, TDao, TPrimaryKey, TRecord>
        where TEntity : AbstractLevelExpEntity<TEntity, TDao, TPrimaryKey, TRecord>, new()
        where TDao : IDao<TPrimaryKey, TRecord>, new()
        where TPrimaryKey : IPrimaryKey
        where TRecord : class, IRecord
    {
        static AbstractLevelExpEntity()
        {
            _tryToSetCache();
        }

        static Dictionary<ushort, TEntity> _levelToEntityMap;
        static Dictionary<ushort, uint> _levelToNecessaryExpMap;
        static ushort _maxLevel;
        static uint _maxExp;


        public static IEnumerable<TEntity> ReadAndBuildAll()
        {
            return _levelToEntityMap.OrderBy(kv => kv.Key).Select(kv => kv.Value);
        }
        /// <summary>
        /// レベルを取得。
        /// </summary>
        /// <param name="exp">経験値</param>
        /// <returns></returns>
        public static ushort GetLevel(uint exp)
        {
            return _levelToNecessaryExpMap.OrderByDescending(kv => kv.Key).First(kv => kv.Value <= exp).Key;
        }
        /// <summary>
        /// 該当レベルに必要な経験値を取得。
        /// </summary>
        /// <param name="level">対象レベル</param>
        /// <returns></returns>
        public static uint GetNecessaryExp(ushort level)
        {
            return _levelToNecessaryExpMap[level];
        }
        public static uint GetMaxLevel()
        {
            return _maxLevel;
        }
        public static uint GetMaxExp()
        {
            return _maxExp;
        }
        /// <summary>
        /// 該当レベルにおける最大経験値。
        /// </summary>
        /// <param name="level">対象レベル</param>
        /// <returns></returns>
        public static uint GetMaxExpByLevel(ushort level)
        {
            return level == _maxLevel ? _maxExp : GetNecessaryExp((ushort)(level + 1)) - 1;
        }
        public static uint GetExpInLevel(ushort currentLevel, uint currentExp)
        {
            return currentLevel < _maxExp ? GetNecessaryExp((ushort)(currentLevel + 1)) - currentExp : 0;
        }

        static void _tryToSetCache()
        {
            if (_levelToNecessaryExpMap == null)
            {
                var allEntities = _instantiate((new TDao()).ReadAll());
                _levelToEntityMap = allEntities.ToDictionary(entity => entity.Level);
                _maxLevel = (ushort)(allEntities.Max(entity => entity.Level) + 1);
                var minLevel = allEntities.Min(entity => entity.Level);

                _levelToNecessaryExpMap = new Dictionary<ushort, uint>();
                _maxExp = 0;
                for (ushort level = minLevel; level < _maxLevel; level++)
                {
                    _levelToNecessaryExpMap.Add(level, _maxExp);
                    _maxExp += _levelToEntityMap[level].Exp;
                }
                _levelToNecessaryExpMap.Add(_maxLevel, _maxExp);
            }
        }


        public abstract ushort Level { get; }
        public abstract uint Exp { get; }
    }
}
