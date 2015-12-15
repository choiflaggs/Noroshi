namespace Noroshi.BattleScene.UI
{
    public interface IWaveGaugeUIView : MonoBehaviours.IUIView
    {
        void Initialize(byte lv, string waveGuageTextKey, float hpRatio, Noroshi.Core.Game.Battle.WaveGaugeType waveGaugeType);
        void ChangeHpRatio(float raito);
    }
}
