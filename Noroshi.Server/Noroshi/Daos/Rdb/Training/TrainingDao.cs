using Schema = Noroshi.Server.Daos.Rdb.Schemas.TrainingSchema;

namespace Noroshi.Server.Daos.Rdb.Training
{
    public class TrainingDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
