using System;

namespace Noroshi.Server.Daos.Rdb
{
	public class AbstractRecord : IRecord
	{
        ReadType? _readType;

        public ReadType GetReadType()
        {
            return _readType.Value;
        }
        public void SetReadType(ReadType readType)
        {
            _readType = readType;
        }

        public IRecord Clone()
        {
            return (IRecord)MemberwiseClone();
        }
	}
}
