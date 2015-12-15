using Schema = Noroshi.Server.Daos.Rdb.Schemas.GearPieceSchema;
namespace Noroshi.Server.Daos.Rdb.Item
{
    public class GearPieceDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
