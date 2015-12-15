using Schema = Noroshi.Server.Daos.Rdb.Schemas.ExchangeCashGiftSchema;
namespace Noroshi.Server.Daos.Rdb.Item
{
    public class ExchangeCashGiftDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
