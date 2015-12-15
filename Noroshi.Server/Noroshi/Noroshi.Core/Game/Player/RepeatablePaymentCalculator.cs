namespace Noroshi.Core.Game.Player
{
    public class RepeatablePaymentCalculator
    {
        uint _basePaymentNum;

        public RepeatablePaymentCalculator(uint basePaymentNum)
        {
            _basePaymentNum = basePaymentNum;
        }

        public uint GetPaymentNum(ushort repeatNum)
        {
            var ratio = 1f;
            if (repeatNum < 3)
            {
                ratio = 1f;
            }
            else if (repeatNum < 7)
            {
                ratio = 1.5f;
            }
            else if (repeatNum < 11)
            {
                ratio = 2f;
            }
            else
            {
                ratio = 3f;
            }
            return (uint)(_basePaymentNum * ratio);
        }
    }
}
