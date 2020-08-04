using System;
using System.Reflection;
using log4net;
using Newtonsoft.Json;
using ReviewApi.Shared.Interfaces;

namespace ReviewApi.Shared.Utils
{
    public class LogUtils : ILogUtils
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void LogError(Exception exception)
        {
            log.Error(JsonConvert.SerializeObject(exception));
        }
    }
}
