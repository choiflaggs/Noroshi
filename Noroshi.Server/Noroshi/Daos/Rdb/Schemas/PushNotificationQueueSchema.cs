namespace Noroshi.Server.Daos.Rdb.Schemas
{
    public class PushNotificationQueueSchema
    {
        public static string TableName => "push_notification_queue";

        public class Record : AbstractRecord
        {
            public System.UInt32 ID { get; set; }
            public System.Byte Status { get; set; }
            public System.Byte DeviceType { get; set; }
            public System.String DeviceToken { get; set; }
            public System.String Message { get; set; }
            public System.UInt32 CreatedAt { get; set; }
        }
        public class PrimaryKey : IPrimaryKey
        {
            public System.UInt32 ID { get; set; }
        }
    }
}