using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Daos.Kvs;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Item;
using Noroshi.Server.Entity.Possession;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GearSchema;
using ItemSchema = Noroshi.Server.Daos.Rdb.Schemas.ItemSchema;

namespace Noroshi.Server.Entity.Item
{
    public class GearEntity : AbstractDaoWrapperEntity<GearEntity, GearDao, Schema.PrimaryKey, Schema.Record>
    {
        static GearEnthantCalculatedDao.Value _tryToSetCache()
        {
            // Redisが見れないためコメントアウト対応
            //var dao = new GearEnthantCalculatedDao();
            var keyValue = new GearEnthantCalculatedDao.Key();
            // Redisが見れないためコメントアウト対応
            //var value = dao.Get(keyValue);
            //if (value != null)
            //{
            //    return value;
            //}
            var value = new GearEnthantCalculatedDao.Value
            // Redisが見れないためコメントアウト対応
            //value = new GearEnthantCalculatedDao.Value
            {
                MaxLevel = new Dictionary<uint, byte>(),
                MaxExp = new Dictionary<uint, uint>(),
                LevelToNecessaryExpMap = new Dictionary<uint, Dictionary<byte, uint>>()
            };
            var LevelToDaoMap = new Dictionary<uint, Dictionary<byte, GearEnchantLevelEntity>>();
            var allEntity = GearEnchantLevelEntity.ReadAndBuildAll();
            var keys = allEntity.Select(entity => entity.GearID).Distinct();
            keys.ToList().ForEach(key =>
            {
                var keyEntities = allEntity.Where(d => d.GearID == key);
                var keyDaos = allEntity.Where(d => d.GearID == key);
                LevelToDaoMap.Add(key, keyDaos.ToDictionary(entity => entity.EnchantLevel));
                value.MaxLevel.Add(key, (byte)(keyEntities.Max(entity => entity.EnchantLevel) + 1));

                value.MaxExp[key] = 0;
                for (byte level = 1; level < value.MaxLevel[key]; level++)
                {
                    if (!value.LevelToNecessaryExpMap.ContainsKey(key))
                        value.LevelToNecessaryExpMap[key] = new Dictionary<byte, uint>();
                    value.LevelToNecessaryExpMap[key].Add(level, value.MaxExp[key]);
                    value.MaxExp[key] += LevelToDaoMap[key][level].Exp;
                }
                value.LevelToNecessaryExpMap[key].Add(value.MaxLevel[key], value.MaxExp[key]);
            });
            // Redisが見れないためコメントアウト対応
            //dao.Set(keyValue, value);
            return value;
        }

        public static IEnumerable<GearEntity> ReadAndBuildMulti(IEnumerable<uint> ids)
        {
            var entities = ReadAndBuildMulti(ids.Select(id => new Schema.PrimaryKey { ID = id }));
            return _addItemAndEnchant(entities);
        }

        public static GearEntity ReadAndBuild(uint id)
        {
            var entities = ReadAndBuildMulti(new List<uint> {id});
            return _addItemAndEnchant(entities).FirstOrDefault();
        }


        public static IEnumerable<GearEntity> ReadAndBuildAll()
        {
            var entities = _instantiate((new GearDao()).ReadAll());
            return _addItemAndEnchant(entities);
        }

        public PossessionParam GetPosssesionParam(uint num)
        {
            return new PossessionParam
            {
                Category = PossessionCategory.Gear,
                ID = GearID,
                Num = num
            };
        }

        public byte GetLevel(uint exp)
        {
            return NecessaryEnchantExpMaps.Length == 0 ? (byte) 0
                : (byte)Enumerable.Range(0, NecessaryEnchantExpMaps.Length).First(index => NecessaryEnchantExpMaps[index] <= exp);
        }


        public uint GearID => _record.ID;
        public ushort Level => _record.Level;

        public uint HP
            => _record.HP;

        public float StrengthGrowth
            => _record.StrengthGrowth;

        public float IntellectGrowth
            => _record.IntellectGrowth;

        public float AgilityGrowth
            => _record.AgilityGrowth;

        public float Strength
            => _record.Strength;

        public float Intellect
            => _record.Intellect;

        public float Agility
            => _record.Agility;

        public uint MagicCrit
            => _record.MagicCrit;

        public float HPRegen
            => _record.HPRegen;

        public float EnergyRegen
            => _record.EnergyRegen;

        public float Dodge
            => _record.Dodge;

        public uint ArmorPenetration
            => _record.ArmorPenetration;

        public float LifeStealRating
            => _record.LifeStealRating;

        public float ImproveHealings
            => _record.ImproveHealings;

        public uint IgnoreMagicResistance
            => _record.IgnoreMagicResistance;

        public uint PhysicalAttack
            => _record.PhysicalAttack;

        public uint MagicPower
            => _record.MagicPower;

        public uint Armor
            => _record.Armor;

        public uint MagicResistance
            => _record.MagicResistance;

        public uint PhysicalCrit
            => _record.PhysicalCrit;

        public byte Accuracy
            => _record.Accuracy;

        public string TextKey => "Master.Item." + _item.TextKey;
        public uint Rarity => _item.Rarity;
        public byte MaxEnchantLevel { get; private set; }
        public uint MaxEnchantExp { get; private set; }
        public uint[] NecessaryEnchantExpMaps { get; private set; }

        public Core.WebApi.Response.Gear ToResponseData()
        {
            return new Core.WebApi.Response.Gear
            {
                ID = GearID,
                Level = Level,
                HP = HP,
                StrengthGrowth = StrengthGrowth,
                IntellectGrowth = IntellectGrowth,
                AgilityGrowth = AgilityGrowth,
                Strength = Strength,
                Intellect = Intellect,
                Agility = Agility,
                MagicCrit = MagicCrit,
                HPRegen = HPRegen,
                EnergyRegen = EnergyRegen,
                Dodge = Dodge,
                ArmorPenetration = ArmorPenetration,
                LifeStealRating = LifeStealRating,
                ImproveHealings = ImproveHealings,
                IgnoreMagicResistance = IgnoreMagicResistance,
                PhysicalAttack = PhysicalAttack,
                MagicPower = MagicPower,
                Armor = Armor,
                MagicResistance = MagicResistance,
                PhysicalCrit = PhysicalCrit,
                Accuracy = Accuracy,
                TextKey = TextKey,
                Rarity = Rarity,
                NecessaryEnchantExpMaps = NecessaryEnchantExpMaps
            };
        }

        static IEnumerable<GearEntity> _addItemAndEnchant(IEnumerable<GearEntity> entities)
        {
            var gearEntities = entities as GearEntity[] ?? entities.ToArray();
            var itemMap = (new ItemDao()).ReadMultiByPKs(gearEntities.Select(s => new ItemSchema.PrimaryKey { ID = s.GearID })).ToDictionary(i => i.ID);
            var enchants = _tryToSetCache();
            foreach (var entity in gearEntities) {
                entity._item = itemMap[entity.GearID];
                entity.MaxEnchantLevel = enchants.MaxLevel.ContainsKey(entity.GearID) ? enchants.MaxLevel[entity.GearID] : (byte)0;
                entity.MaxEnchantExp = enchants.MaxExp.ContainsKey(entity.GearID) ? enchants.MaxExp[entity.GearID] : 0;
                entity.NecessaryEnchantExpMaps = enchants.LevelToNecessaryExpMap.ContainsKey(entity.GearID)
                    ? enchants.LevelToNecessaryExpMap[entity.GearID].OrderBy(map => map.Key).Select(map => map.Value).ToArray()
                    : Enumerable.Empty<uint>().ToArray();
            }
            return gearEntities;
        }

        private ItemSchema.Record _item;
    }
}