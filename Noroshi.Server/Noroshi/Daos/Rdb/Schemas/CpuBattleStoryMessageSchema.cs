namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class CpuBattleStoryMessageSchema
	{
		public static string TableName => "cpu_battle_story_message";

		public class Record : AbstractRecord
		{
			public System.UInt32 StoryID { get; set; }
			public System.Byte No { get; set; }
			public System.String TextKey { get; set; }
			public System.UInt32 OwnCharacterID { get; set; }
			public System.Byte OwnCharacterActingType { get; set; }
			public System.UInt32 EnemyCharacterID { get; set; }
			public System.Byte EnemyCharacterActingType { get; set; }
			public System.Byte EffectType { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 StoryID { get; set; }
			public System.Byte No { get; set; }
		}
	}
}
