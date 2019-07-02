using Newtonsoft.Json;
using System;

namespace NEL.NNS.lib
{
    public static class LogHelper
    {
        public static string logInfoFormat(object inputJson, object outputJson, DateTime start)
        {
            return "\r\n input:\r\n"
                + JsonConvert.SerializeObject(inputJson)
                + "\r\n output \r\n"
                + JsonConvert.SerializeObject(outputJson)
                + "\r\n exetime \r\n"
                + DateTime.Now.Subtract(start).TotalMilliseconds
                + "ms \r\n";
        }
    }
}
