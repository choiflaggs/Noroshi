using UniRx;
using Noroshi.Core.Game.Character;

namespace Noroshi.Game
{
    public class CharacterStatus : Core.Game.Character.CharacterStatus
    {
        public CharacterStatus(IPersonalCharacter personalData, Core.WebApi.Response.Character.Character masterData) : base(personalData, masterData)
        {
        }

        public virtual IObservable<CharacterStatus> LoadGears()
        {
            var initialGearIds = new uint[]
            {
                _personalData.GearID1,
                _personalData.GearID2,
                _personalData.GearID3,
                _personalData.GearID4,
                _personalData.GearID5,
                _personalData.GearID6,
            };
            return GlobalContainer.RepositoryManager.GearRepository.GetMulti(initialGearIds)
                .Select(gs => {
                    for (byte no = 1; no <= MAX_GEAR_SLOT_NUM; no++)
                    {
                        var gear = no <= gs.Length ? gs[(int)(no - 1)] : null;
                        if (gear != null)
                        {
                            _gearContainer.Add(no, new Core.Game.Character.Gear(gear));
                        }
                    }
                    return this;
                });
        }
    }
}
