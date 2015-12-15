using System;

namespace Noroshi.Core.WebApi.Response
{
    public class DefensiveWar
    {
        public uint BattleID
        { get; set; }
        public byte OrderPriority
        { get; set; }
        public byte HeldWeek
        { get; set; }
        public DateTime StartDay
        { get; set; }
        public DateTime EndDay
        { get; set; }
    }
}