using System;

namespace Noroshi.Core.Utility
{
    /// <summary>
    /// 自然増加値を扱うクラス。
    /// </summary>
    public class NaturalIncreaseValue
    {
        /// <summary>
        /// 値が増加するのに必要な期間。
        /// </summary>
        readonly TimeSpan _timeSpan;

        /// <summary>
        /// 値が増加するのに必要な期間を指定してインスタンス化。
        /// </summary>
        /// <param name="timeSpan">値が増加するのに必要な期間</param>
        public NaturalIncreaseValue(TimeSpan timeSpan)
        {
            _timeSpan = timeSpan;
        }

        /// <summary>
        /// 現在値。
        /// </summary>
        /// <param name="lastValue">前回値（uint）</param>
        /// <param name="lastUpdatedAt">前回値更新日時</param>
        /// <param name="maxValue">最大値（uint）</param>
        /// <param name="currentUnixTime">現在日時</param>
        /// <returns></returns>
        protected uint _currentValue(uint lastValue, uint lastUpdatedAt, uint maxValue, uint currentUnixTime)
        {
            if (maxValue < lastValue) return lastValue;

            var currentValue = lastValue + (currentUnixTime - lastUpdatedAt) / _timeSpan.TotalSeconds;
            return Math.Min(maxValue, (uint)currentValue);
        }
        /// <summary>
        /// 現在値。
        /// </summary>
        /// <param name="lastValue">前回値（ushort）</param>
        /// <param name="lastUpdatedAt">前回値更新日時</param>
        /// <param name="maxValue">最大値（ushort）</param>
        /// <param name="currentUnixTime">現在日時</param>
        /// <returns></returns>
        protected ushort _currentValue(ushort lastValue, uint lastUpdatedAt, ushort maxValue, uint currentUnixTime)
        {
            return (ushort)_currentValue((uint)lastValue, lastUpdatedAt, (uint)maxValue, currentUnixTime);
        }
        /// <summary>
        /// 現在値。
        /// </summary>
        /// <param name="lastValue">前回値（byte）</param>
        /// <param name="lastUpdatedAt">前回値更新日時</param>
        /// <param name="maxValue">最大値（byte）</param>
        /// <param name="currentUnixTime">現在日時</param>
        /// <returns></returns>
        protected byte _currentValue(byte lastValue, uint lastUpdatedAt, byte maxValue, uint currentUnixTime)
        {
            return (byte)_currentValue((uint)lastValue, lastUpdatedAt, (uint)maxValue, currentUnixTime);
        }

        /// <summary>
        /// 次の値増加までの時間。
        /// </summary>
        /// <param name="lastValue">前回値（uint）</param>
        /// <param name="lastUpdatedAt">前回値更新日時</param>
        /// <param name="maxValue">最大値（uint）</param>
        /// <param name="currentUnixTime">現在日時</param>
        /// <returns></returns>
        protected TimeSpan? _getTimeToNextIncrease(uint lastValue, uint lastUpdatedAt, uint maxValue, uint currentUnixTime)
        {
            if (currentUnixTime < lastUpdatedAt) return null;
            if (maxValue <= _currentValue(lastValue, lastUpdatedAt, maxValue, currentUnixTime)) return null;
            var second = (currentUnixTime - lastUpdatedAt) % _timeSpan.TotalSeconds;
            return TimeSpan.FromSeconds(_timeSpan.TotalSeconds - second);
        }
        /// <summary>
        /// 次の値増加までの時間。
        /// </summary>
        /// <param name="lastValue">前回値（ushort）</param>
        /// <param name="lastUpdatedAt">前回値更新日時</param>
        /// <param name="maxValue">最大値（ushort）</param>
        /// <param name="currentUnixTime">現在日時</param>
        /// <returns></returns>
        protected TimeSpan? _getTimeToNextIncrease(ushort lastValue, uint lastUpdatedAt, ushort maxValue, uint currentUnixTime)
        {
            return _getTimeToNextIncrease((uint)lastValue, lastUpdatedAt, (uint)maxValue, currentUnixTime);
        }
        /// <summary>
        /// 次の値増加までの時間。
        /// </summary>
        /// <param name="lastValue">前回値（byte）</param>
        /// <param name="lastUpdatedAt">前回値更新日時</param>
        /// <param name="maxValue">最大値（byte）</param>
        /// <param name="currentUnixTime">現在日時</param>
        /// <returns></returns>
        protected TimeSpan? _getTimeToNextIncrease(byte lastValue, uint lastUpdatedAt, byte maxValue, uint currentUnixTime)
        {
            return _getTimeToNextIncrease((uint)lastValue, lastUpdatedAt, (uint)maxValue, currentUnixTime);
        }
    }
}
