namespace Noroshi.Server.Daos.Rdb
{
    public interface IRecord
    {
        ReadType GetReadType();
        void SetReadType(ReadType readType);
        IRecord Clone();
    }
}