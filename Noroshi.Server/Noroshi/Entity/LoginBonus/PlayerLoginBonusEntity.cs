using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.LoginBonus;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerLoginBonusSchema;

namespace Noroshi.Server.Entity.LoginBonus
{
    public class PlayerLoginBonusEntity : AbstractDaoWrapperEntity<PlayerLoginBonusEntity, PlayerLoginBonusDao, Schema.PrimaryKey, Schema.Record>
    {
        public static PlayerLoginBonusEntity ReadAndBuild(uint playerId, uint loginBonusId, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuild(new Schema.PrimaryKey { PlayerID = playerId, LoginBonusID = loginBonusId }, readType);
        }
        public static IEnumerable<PlayerLoginBonusEntity >ReadAndBuildByPlayerIDAndLoginBonusIDs(uint playerId, IEnumerable<uint> loginBonusIds, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(loginBonusIds.Select(loginBonusId => new Schema.PrimaryKey { PlayerID = playerId, LoginBonusID = loginBonusId }));
        }

        public static PlayerLoginBonusEntity CountUp(uint playerId, uint loginBonusId)
        {
            var record = (new PlayerLoginBonusDao()).Create(playerId, loginBonusId, 1);
            PlayerLoginBonusEntity entity = null;
            if (record != null)
            {
                entity = _instantiate(record);
            }
            else
            {
                entity = ReadAndBuild(playerId, loginBonusId, ReadType.Lock);
                if (entity.CanCountUp())
                {
                    var currentNum = entity.CurrentNum;
                    if (entity.ReceiveRewardThreshold ==entity.CurrentNum)
                    {
                        currentNum += 1;
                    }
                    if (!entity.SaveCurrent((byte)(currentNum)))
                    {
                        throw new SystemException(string.Join("\t", "Fail to Update", playerId));
                    }

                }
            }
            return entity;
        }

        public uint LoginBonusID => _record.LoginBonusID;
        public byte CurrentNum => _record.CurrentNum;
        public byte ReceiveRewardThreshold => _record.ReceiveRewardThreshold;

        public bool CanCountUp()
        {
            return ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.LastLoggedInAt);
        }

        public bool SaveCurrent(byte current)
        {
            var newRecord = _cloneRecord();
            newRecord.CurrentNum = current;
            newRecord.LastLoggedInAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            newRecord.UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(newRecord);
            return Save();
        }
        public bool HasAlreadyReceivedReward(uint threshold)
        {
            return ReceiveRewardThreshold >= threshold;
        }

        public bool CanReceiveReward(uint threshold)
        {
            return !HasAlreadyReceivedReward(threshold) && threshold <= CurrentNum;
        }
        public bool ReceiveReward(byte threshold)
        {
            var newRecord = _cloneRecord();
            newRecord.ReceiveRewardThreshold = threshold;
            newRecord.UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(newRecord);
            return Save();
        }
    }
}
