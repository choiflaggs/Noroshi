using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Possession;
using Noroshi.Core.Game.Player;
using CharacterConstant = Noroshi.Core.Game.Character.Constant;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Item;

namespace Noroshi.Server.Entity.Player
{
    public class TutorialEntity
    {
        static readonly Dictionary<TutorialStep, Func<IEnumerable<PossessionParam>>> TUTORIAL_STEP_TO_POSSESSION_PARAMS_MAP = new Dictionary<TutorialStep, Func<IEnumerable<PossessionParam>>>
        {
            { TutorialStep.ClearStoryStage1, _getSecondCharacter },
            { TutorialStep.ClearStoryStage2, _getFirstCharacterEquipmentsAndDrug },
            { TutorialStep.ClearStoryStage5, _getThirdCharacter },
            { TutorialStep.ClearStoryStage7, _getFourthCharacter },
            { TutorialStep.ClearStoryStage10, _getFourthCharacterSouls },
        };
        static IEnumerable<PossessionParam> _getFirstCharacterEquipmentsAndDrug()
        {
            var firstCharacter = CharacterEntity.ReadAndBuild(CharacterConstant.FIRST_CHARACTER_ID);
            var possessionParams = firstCharacter.GetGearIDs(1).Select(gearId => new PossessionParam
            {
                Category = PossessionCategory.Gear,
                ID = gearId,
                Num = 1,
            })
            .ToList();
            possessionParams.Add(new PossessionParam
            {
                Category = PossessionCategory.Drug,
                ID = 50101005,
                Num = 1,
            });
            return possessionParams;
        }
        static IEnumerable<PossessionParam> _getSecondCharacter()
        {
            return _getCharacter(CharacterConstant.SECOND_CHARACTER_ID);
        }
        static IEnumerable<PossessionParam> _getThirdCharacter()
        {
            return _getCharacter(CharacterConstant.THIRD_CHARACTER_ID);
        }
        static IEnumerable<PossessionParam> _getFourthCharacter()
        {
            return _getCharacter(CharacterConstant.FOURTH_CHARACTER_ID);
        }
        static IEnumerable<PossessionParam> _getFourthCharacterSouls()
        {
            var thirdCharacter = CharacterEntity.ReadAndBuild(CharacterConstant.FOURTH_CHARACTER_ID);
            var soul = SoulEntity.ReadAndBuildByCharacterID(thirdCharacter.ID);
            return new PossessionParam[]
            {
                new PossessionParam
                {
                    Category = PossessionCategory.Soul,
                    ID = soul.SoulID,
                    Num = 50,
                }
            };
        }
        static IEnumerable<PossessionParam> _getCharacter(uint characterId)
        {
            return new PossessionParam[]
            {
                new PossessionParam
                {
                    Category = PossessionCategory.Character,
                    ID = characterId,
                    Num = 1,
                }
            };
        }

        TutorialStep _tutorialStep;

        public TutorialEntity(TutorialStep tutorialStep)
        {
            _tutorialStep = tutorialStep;
        }

        public IEnumerable<PossessionParam> GetPossessionParams()
        {
            return TUTORIAL_STEP_TO_POSSESSION_PARAMS_MAP.ContainsKey(_tutorialStep) ? TUTORIAL_STEP_TO_POSSESSION_PARAMS_MAP[_tutorialStep].Invoke() : new PossessionParam[0];
        }
    }
}
