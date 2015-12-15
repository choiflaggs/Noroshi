using System;

namespace Noroshi.Core.WebApi.Response.Debug
{
    public class TimeDebug
    {
        public DateTime ServerTime
        { get; set; }
        public DateTime DebugServerTime
        { get; set; }
        public int ModifyTime
        { get; set; }
    }
}