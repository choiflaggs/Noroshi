using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Battle;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Possession;

namespace Noroshi.Server.Entity.Battle
{
    public class PlayerBattleEntity : AbstractBattleEntity
    {
        public static PlayerBattleEntity ReadAndBuild(uint id)
        {
            var player = PlayerEntity.ReadAndBuild(id);
            var battle = new PlayerBattleEntity(player);
            battle.LoadAssociatedEntities();
            return battle;
        }

        PlayerEntity _player;

        IEnumerable<PlayerBattleWave> _waves;

        public PlayerBattleEntity(PlayerEntity player)
        {
            _player = player; ;
        }

        public uint   ID => _player.ID;
        public override uint Gold => 0;
        public uint   FieldID => 1;

        public void LoadAssociatedEntities()
        {
            var playerCharacters = PlayerCharacterEntity.ReadAndBuildMulti(PlayerArenaEntity.CreateOrReadAndBuild(_player.ID).DeckPlayerCharacterIDs);
            _waves = new [] { new PlayerBattleWave(playerCharacters) };
        }

        public override uint GetCharacterExp()
        {
            return 0;
        }

        public override void ApplyInitialCondition(InitialCondition.CharacterCondition[][] enemyCharacterConditions)
        {
        }


        public override IEnumerable<PossessionParam> GetDroppableRewards()
        {
            return new PossessionParam[0];
        }

        public override List<List<List<PossessionParam>>> LotDropRewards()
        {
            return new List<List<List<PossessionParam>>>();
        }

        public Core.WebApi.Response.Battle.PlayerBattle ToResponseData()
        {
            return new Core.WebApi.Response.Battle.PlayerBattle()
            {
                CharacterExp = GetCharacterExp(),
                Gold = Gold,
                FieldID = FieldID,
                Waves = _waves.Select(w => w.ToResponseData()).ToArray(),
            };
        }
    }
}
