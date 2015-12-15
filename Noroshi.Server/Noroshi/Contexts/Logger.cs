using System.Collections.Generic;
using System.Linq;
using NLog;

namespace Noroshi.Server.Contexts
{
    public class Logger
    {
        readonly NLog.Logger _logger;

        public Logger()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void Info(string name, params object[] objects)
        {
            var objectList = new List<object> { $"[{name}]" };
            objectList.AddRange(objects);
            _logger.Info(string.Join("\t", objectList.ToArray()));
        }
    }
}
