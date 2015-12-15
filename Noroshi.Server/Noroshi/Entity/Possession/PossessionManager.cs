using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Entity.Player;

namespace Noroshi.Server.Entity.Possession
{
    public class PossessionManager
    {
        public static PossessionParam GetPlayerExpParam(ushort playerExp)
        {
            return new PossessionParam
            {
                Category = PossessionCategory.Status,
                ID = (uint)PossessionStatusID.PlayerExp,
                Num = playerExp
            };
        }
        public static PossessionParam GetGoldParam(uint gold)
        {
            return new PossessionParam
            {
                Category = PossessionCategory.Status,
                ID = (uint)PossessionStatusID.Gold,
                Num = gold
            };
        }
        public static bool IsGoldParam(IPossessionParam param)
        {
            var dummyGoldParam = GetGoldParam(0);
            return param.Category == dummyGoldParam.Category && param.ID == dummyGoldParam.ID;
        }
        public static PossessionParam GetCommonGemParam(ushort gem)
        {
            return new PossessionParam
            {
                Category = PossessionCategory.Status,
                ID = (uint)PossessionStatusID.CommonGem,
                Num = gem,
            };
        }
        public static PossessionParam GetFreeGemParam(byte gem)
        {
            return new PossessionParam
            {
                Category = PossessionCategory.Status,
                ID = (uint)PossessionStatusID.FreeGem,
                Num = gem,
            };
        }
        public static PossessionParam GetBPParam(byte bp)
        {
            return new PossessionParam
            {
                Category = PossessionCategory.Status,
                ID = (uint)PossessionStatusID.Gold,
//                ID = (uint)PossessionStatusID.BP,
                Num = bp,
            };
        }
        public static PossessionParam GetGuildPointParam(ushort guildPoint)
        {
            return new PossessionParam
            {
                Category = PossessionCategory.Status,
                ID = (uint)PossessionStatusID.Gold,
//                ID = (uint)PossessionStatusID.BP,
                Num = guildPoint,
            };
        }
        public static bool IsGuildPointParam(IPossessionParam param)
        {
            var dummyParam = GetGuildPointParam(0);
            return param.Category == dummyParam.Category && param.ID == dummyParam.ID;
        }

        public static PossessionParam GetExpeditionPointParam(uint expeditionPoint)
        {
            return new PossessionParam
            {
                Category = PossessionCategory.Status,
                ID = (uint)PossessionStatusID.Gold,
                Num = expeditionPoint,
            };
        }


        private readonly Dictionary<PossessionCategory, IPossessionHandler> _handlerMap;

        public PossessionManager(uint playerId, PossessionParam possessionParam)
        {
            _handlerMap = _buildHandlers(playerId, new[] { possessionParam });
        }
        public PossessionManager(uint playerId, IEnumerable<PossessionParam> possessableParams)
        {
            _handlerMap = _buildHandlers(playerId, possessableParams);
        }

        public static Dictionary<PossessionCategory, IPossessionHandler> _buildHandlers(uint playerId, IEnumerable<PossessionParam> possessableParams)
        {
            var handlerMap = new Dictionary<PossessionCategory, IPossessionHandler>();
            foreach (var grouping in possessableParams.ToLookup(po => po.Category))
            {
                switch (grouping.Key)
                {
                    case PossessionCategory.Gear:
                        handlerMap.Add(grouping.Key, new GearHandler(playerId, grouping.Select(v => v.ID)));
                        break;
                    case PossessionCategory.GearPiece:
                        handlerMap.Add(grouping.Key, new GearPieceHandler(playerId, grouping.Select(v => v.ID)));
                        break;
                    case PossessionCategory.GearEnchantMaterial:
                        handlerMap.Add(grouping.Key, new GearEnchantMaterialHandler(playerId, grouping.Select(v => v.ID)));
                        break;
                    case PossessionCategory.Soul:
                        handlerMap.Add(grouping.Key, new SoulHandler(playerId, grouping.Select(v => v.ID)));
                        break;
                    case PossessionCategory.Drug:
                        handlerMap.Add(grouping.Key, new DrugHandler(playerId, grouping.Select(v => v.ID)));
                        break;
                    case PossessionCategory.ExchangeCashGift:
                        handlerMap.Add(grouping.Key, new ExchangeCashGiftHandler(playerId, grouping.Select(v => v.ID)));
                        break;
                    case PossessionCategory.RaidTicket:
                        handlerMap.Add(grouping.Key, new RaidTicketHandler(playerId, grouping.Select(v => v.ID)));
                        break;
                    case PossessionCategory.Character:
                        handlerMap.Add(grouping.Key, new CharacterHandler(playerId, grouping.Select(v => v.ID)));
                        break;
                    case PossessionCategory.Status:
                        handlerMap.Add(grouping.Key, new StatusHandler(playerId, grouping.Select(v => v.ID)));
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            return handlerMap;
        }

        public void Load()
        {
            foreach (var handler in _handlerMap.Values)
            {
                handler.Load();
            }
        }

        public IPossessionObject GetPossessionObject(PossessionParam possessionParam)
        {
            return _handlerMap[possessionParam.Category].GetPossessionObject(possessionParam.ID, possessionParam.Num);
        }
        public IEnumerable<IPossessionObject> GetPossessionObjects(IEnumerable<PossessionParam> possessableParams)
        {
            return possessableParams.Select(param => _handlerMap[param.Category].GetPossessionObject(param.ID, param.Num));
        }

        public void Add(PossessionParam possessionParam)
        {
            Add(new[] { possessionParam });
        }
        public void Add(IEnumerable<PossessionParam> possessionParam)
        {
            Add(possessionParam.Cast<IPossessionParam>());
        }
        public void Add(IEnumerable<IPossessionParam> possessionParam)
        {
            foreach (var grouping in possessionParam.ToLookup(po => po.Category))
            {
                _handlerMap[grouping.Key].Add(grouping.ToArray());
            }
        }

        public bool CanRemove(PossessionParam possessionParam)
        {
            return CanRemove(new[] { possessionParam });
        }
        public bool CanRemove(IEnumerable<PossessionParam> possessionParam)
        {
            return CanRemove(possessionParam.Cast<IPossessionParam>());
        }
        public bool CanRemove(IEnumerable<IPossessionParam> possessionParam)
        {
            return possessionParam.ToLookup(po => po.Category).All(grouping => _handlerMap[grouping.Key].CanRemove(grouping.ToArray()));
        }

        public void Remove(PossessionParam possessionParam)
        {
            Remove(new[] { possessionParam });
        }
        public void Remove(IEnumerable<PossessionParam> possessionParam)
        {
            Remove(possessionParam.Cast<IPossessionParam>());
        }
        public void Remove(IEnumerable<IPossessionParam> possessionParam)
        {
            foreach (var grouping in possessionParam.ToLookup(po => po.Category))
            {
                _handlerMap[grouping.Key].Remove(grouping.ToArray());
            }
        }

        public PlayerStatusEntity LoadPlayerStatusWithLock()
        {
            var statusHandler = _handlerMap.ContainsKey(PossessionCategory.Status) ? (StatusHandler)_handlerMap[PossessionCategory.Status] : null;
            return statusHandler?.LoadPlayerStatusWithLock();
        }
        public AddPlayerExpResult GetAddPlayerExpResult()
        {
            var statusHandler = _handlerMap.ContainsKey(PossessionCategory.Status) ? (StatusHandler)_handlerMap[PossessionCategory.Status] : null;
            return statusHandler?.GetAddPlayerExpResult();
        }
    }
}
