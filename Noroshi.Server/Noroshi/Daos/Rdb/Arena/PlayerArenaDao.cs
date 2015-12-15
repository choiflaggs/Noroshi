using System.Collections.Generic;
using Noroshi.Server.Contexts;
using Noroshi.Core.Game.Arena;
using Noroshi.Server.Entity.PlayerCounter;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerArenaSchema;

namespace Noroshi.Server.Daos.Rdb.Arena
{
    public class PlayerArenaDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByRankOverAndRowCountOtherPlayerId(uint rank, uint rowCount, uint playerId )
        {
            return _select(
            "@Rank < Rank AND PlayerID != @PlayerID ORDER By Rank ASC LIMIT @RowCount",
            new
            {
                Rank                = rank,
                RowCount            = rowCount,
                PlayerID            = playerId
            });
        }

        public IEnumerable<Schema.Record> ReadByRankUnderAndRowCountOtherPlayerId( uint rank, uint rowCount, uint playerId )
        {
            return _select(
            "Rank < @Rank And PlayerID != @PlayerID ORDER By Rank DESC LIMIT @RowCount",
            new
            {
                Rank                = rank,
                RowCount            = rowCount,
                PlayerID            = playerId
            });
        }

        public Schema.Record CreateOrRead(uint playerId)
        {
            return Create(DefaultRecord(playerId)) ?? ReadByPK(new Schema.PrimaryKey { PlayerID = playerId }, ReadType.Lock);
        }

        public Schema.Record BuildOrRead(uint playerId, ReadType readType = ReadType.Slave)
        {
            return ReadByPK(new Schema.PrimaryKey { PlayerID = playerId }, ReadType.Lock) ?? DefaultRecord(playerId);
        }

        private Schema.Record DefaultRecord(uint playerId)
        {
            return new Schema.Record
            {
                PlayerID                = playerId,
                Rank                    = 0,
                BestRank                = 0,
                DeckPlayerCharacterID1  = 0,
                DeckPlayerCharacterID2  = 0,
                DeckPlayerCharacterID3  = 0,
                DeckPlayerCharacterID4  = 0,
                DeckPlayerCharacterID5  = 0,
                Win                     = 0,
                Lose                    = 0,
                DefenseWin              = 0,
                DefenseLose             = 0,
                AllHP                   = 0,
                AllStrength             = 0,
                PlayNum                 = 0,
                PlayResetNum            = 0,
                LastBattledAt           = 0,
                LastPlayResetAt         = 0,
                CoolTimeAt              = 0,
                CoolTimeResetNum        = 0,
                LastCoolTimeResetAt     = 0,
                BattleStartedAt         = 0,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
            };
        }

    }
}