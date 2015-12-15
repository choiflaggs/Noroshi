using System;
using System.Collections.Generic;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Daos.Rdb;
using AddPlayerVipExpResult = Noroshi.Server.Entity.Player.AddPlayerVipExpResult;

namespace Noroshi.Server.Entity.Possession
{
    public class StatusHandler : AbstractPossessionHandler<Status>
    {
        static readonly Dictionary<PossessionStatusID, uint> STATUS_ID_TO_MAX_NUM_MAP = new Dictionary<PossessionStatusID, uint>
        {
            {PossessionStatusID.PlayerExp, ushort.MaxValue},
            {PossessionStatusID.Gold, uint.MaxValue},
            {PossessionStatusID.CommonGem, uint.MaxValue},
            {PossessionStatusID.FreeGem, uint.MaxValue},
            {PossessionStatusID.BP, uint.MaxValue},
            {PossessionStatusID.Stamina, uint.MaxValue},
            {PossessionStatusID.PlayerVipExp, uint.MaxValue},
        };

        PlayerStatusEntity _playerStatus;
        AddPlayerExpResult _addPlayerExpResult;
        AddPlayerVipExpResult _addPlayerVipExpResult;

        public StatusHandler(uint playerId, IEnumerable<uint> candidateItemIds) : base(playerId, candidateItemIds)
        {
        }

        public override PossessionCategory PossessionCategory => PossessionCategory.Status;

        protected override void _load(IEnumerable<uint> candidateIds, bool beforeUpdate)
        {
            var readType = beforeUpdate ? ReadType.Lock : ReadType.Slave;
            _playerStatus = PlayerStatusEntity.ReadAndBuild(PlayerID, readType);
        }

        public override IPossessionObject GetPossessionObject(uint id, uint num)
        {
            return new Status(id, num, GetCurrentNum(id));
        }

        public override uint GetCurrentNum(uint statusId)
        {
            switch ((PossessionStatusID)statusId)
            {
                case PossessionStatusID.PlayerExp:
                    return _playerStatus.Exp;
                case PossessionStatusID.Gold:
                    return _playerStatus.Gold;
                case PossessionStatusID.CommonGem:
                    return _playerStatus.TotalGem;
                case PossessionStatusID.FreeGem:
                    return _playerStatus.FreeGem;
                case PossessionStatusID.BP:
                    return _playerStatus.BP;
                case PossessionStatusID.Stamina:
                    return _playerStatus.Stamina;
                case PossessionStatusID.PlayerVipExp:
                    return _playerStatus.VipExp;
                default:
                    throw new ArgumentException();
            }
        }
        public override uint GetMaxNum(uint statusId)
        {
            return STATUS_ID_TO_MAX_NUM_MAP[(PossessionStatusID)statusId];
        }

        protected override bool _add(IEnumerable<IPossessionParam> possessionParams)
        {
            foreach (var possessionParam in possessionParams)
            {
                switch ((PossessionStatusID)possessionParam.ID)
                {
                    case PossessionStatusID.PlayerExp:
                        _addPlayerExpResult = _playerStatus.AddExp((ushort)possessionParam.Num);
                        break;
                    case PossessionStatusID.Gold:
                        _playerStatus.ChangeGold(_playerStatus.Gold + possessionParam.Num);
                        break;
                    case PossessionStatusID.FreeGem:
                        _playerStatus.ChangeFreeGem(_playerStatus.FreeGem + possessionParam.Num);
                        break;
                    case PossessionStatusID.BP:
                        _playerStatus.SetBP((byte)(_playerStatus.BP + possessionParam.Num));
                        break;
                    case PossessionStatusID.Stamina:
                        _playerStatus.SetStamina((byte)(_playerStatus.Stamina + possessionParam.Num));
                        break;
                    case PossessionStatusID.PlayerVipExp:
                        _addPlayerVipExpResult = _playerStatus.AddVipExp(possessionParam.Num);
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            return _playerStatus.Save();
        }

        protected override bool _remove(IEnumerable<IPossessionParam> possessionParams)
        {
            foreach (var possessionParam in possessionParams)
            {
                switch ((PossessionStatusID)possessionParam.ID)
                {
                    case PossessionStatusID.Gold:
                        _playerStatus.ChangeGold(_playerStatus.Gold - possessionParam.Num);
                        break;
                    case PossessionStatusID.CommonGem:
                        _playerStatus.UseGem(possessionParam.Num);
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            return _playerStatus.Save();
        }

        /// <summary>
        /// 外でも PlayerStatus を使いたい場合に利用する独自メソッド。
        /// </summary>
        /// <returns></returns>
        public PlayerStatusEntity LoadPlayerStatusWithLock()
        {
            _loadBeforeUpdate();
            return _playerStatus;
        }

        /// <summary>
        /// プレイヤー経験値を付与した結果を取得する。独自メソッド。
        /// </summary>
        /// <returns></returns>
        public AddPlayerExpResult GetAddPlayerExpResult()
        {
            return _addPlayerExpResult;
        }

        /// <summary>
        /// プレイヤーVIP経験値を付与した結果を取得する。独自メソッド。
        /// </summary>
        /// <returns></returns>
        public AddPlayerVipExpResult GetAddPlayerVipExpResult()
        {
            return _addPlayerVipExpResult;
        }
    }
}
