namespace Noroshi.Server.Daos.Kvs
{
    public class ItemDropLocationDao : AbstractDao<ItemDropLocationDao.Key, ItemDropLocationDao.Value>
    {
        public class Value
        {
            public uint ID { get; set; }
            public uint ContentID { get; set; }
        }
        public class Key
        {
            public uint ID { get; set; }
            public override string ToString()  => "item_drop_location_" + ID;
        }
    }
}