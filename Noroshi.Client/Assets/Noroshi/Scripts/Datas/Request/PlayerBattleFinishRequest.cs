namespace Noroshi.Datas.Request
{
    // バトル終了時通信リクエスト
    public class PlayerBattleFinishRequest
    {
        public byte Category { get; set; }
        public uint ID { get; set; }
        public byte VictoryOrDefeat { get; set; }
        public byte Rank { get; set; }
        public string Result { get; set; }
    }
}
