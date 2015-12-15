using System.Collections.Generic;
using UniLinq;
using UniRx;

namespace Noroshi.BattleScene
{
    public class Story
    {
        public enum DramaType
        {
            /// ボスが死なずに逃亡。
            BossEscape = 1,
        }

        int _currentMessageNo = 0;
        Core.WebApi.Response.Battle.CpuBattleStory _data;
        Dictionary<uint, ICharacterView> _characterViewMap;
        Dictionary<uint, string> _characterNameTextKeyMap;

        public Story(Core.WebApi.Response.Battle.CpuBattleStory data)
        {
            _data = data;
        }

        public IEnumerable<uint> GetCharacterIDs()
        {
            return _data.Messages
                .SelectMany(message => new uint?[]{message.OwnCharacterID, message.EnemyCharacterID})
                .Where(id => id.HasValue).Select(id => id.Value).Distinct();
        }

        public void SetCharacterViewMap(Dictionary<uint, ICharacterView> characterViewMap)
        {
            _characterViewMap = characterViewMap;
        }

        public void SetCharacterNameTextKeyMap(Dictionary<uint, string> characterNameTextKeyMap)
        {
            _characterNameTextKeyMap = characterNameTextKeyMap;
        }

        public ICharacterView GetCurrentMessageOwnCharacterView()
        {
            var characterId = _data.Messages[_currentMessageNo - 1].OwnCharacterID;
            return characterId.HasValue ? _characterViewMap[characterId.Value] : null;
        }
        public ICharacterView GetCurrentMessageEnemyCharacterView()
        {
            var characterId = _data.Messages[_currentMessageNo - 1].EnemyCharacterID;
            return characterId.HasValue ? _characterViewMap[characterId.Value] : null;
        }
        public string GetCurrentMessageTextKey()
        {
            return _data.Messages[_currentMessageNo - 1].TextKey;
        }
        public string GetCurrentCharacterNameTextKey()
        {
            var currentMessage = _data.Messages[_currentMessageNo - 1];
            var characterId = currentMessage.OwnCharacterID.HasValue ? currentMessage.OwnCharacterID.Value
                : currentMessage.EnemyCharacterID.HasValue ? currentMessage.EnemyCharacterID.Value
                : 0;
            return characterId == 0 ? string.Empty : _characterNameTextKeyMap[characterId];
        }

        public int GetMessageNum()
        {
            return _data.Messages.Length;
        }

        public DramaType? GetDramaType()
        {
            return _data.DramaType > 0 ? (DramaType?)_data.DramaType : null;
        }

        public bool GoNextMessage()
        {
            if (_currentMessageNo < GetMessageNum())
            {
                _currentMessageNo++;
                return true;
            }
            return false;
        }
    }
}
