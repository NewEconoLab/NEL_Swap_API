using NEL.API.RPC;
using NEL.NNS.lib;
using NEL_Swap_API.Service;
using Newtonsoft.Json.Linq;
using System;

namespace NEL.Comm
{
    public class Api
    {
        private string netnode;
        private static Api testApi = new Api("testnet");
        private static Api mainApi = new Api("mainnet");
        public static Api getTestApi() { return testApi; }
        public static Api getMainApi() { return mainApi; }
        //
        private HttpHelper hh = new HttpHelper();
        private MongoHelper mh = new MongoHelper();
        private ExchangeService exchangeService;

        public Api(string node)
        {
            netnode = node;
            switch (netnode)
            {
                case "testnet":
                    exchangeService = new ExchangeService {
                        mh = mh,
                        notify_mongodbConnStr = mh.notify_mongodbConnStr_testnet,
                        notify_mongodbDatabase = mh.notify_mongodbDatabase_testnet,
                    };
                    break;
                case "mainnet":
                    break;
            }
        }
        public object getRes(JsonRPCrequest req, string reqAddr)
        {
            JArray result = null;
            try
            {
                switch (req.method)
                {
                    // 获取GAS/USD价格
                    case "getExchangePrice":
                        result = exchangeService.getExchangePrice();
                        break;
                    // 获取USD/CNY汇率
                    case "getExchangeRate":
                        result = exchangeService.getExchangeRate();
                        break;
                    // 获取资产列表
                    case "getAssetList":
                        result = exchangeService.getAssetList(req.@params[0].ToString(), int.Parse(req.@params[1].ToString()), int.Parse(req.@params[2].ToString()));
                        break;
                    case "getnodetype":
                        result = new JArray { new JObject { { "nodeType", netnode } } };
                        break;
                }
                //
                if (result == null || result.Count == 0)
                {
                    JsonPRCresponse_Error resE = new JsonPRCresponse_Error(req.id, -1, "No Data", "Data does not exist");
                    return resE;
                }
            }
            catch (Exception e)
            {
                JsonPRCresponse_Error resE = new JsonPRCresponse_Error(req.id, -100, "Parameter Error", e.Message);
                return resE;
            }

            JsonPRCresponse res = new JsonPRCresponse();
            res.jsonrpc = req.jsonrpc;
            res.id = req.id;
            res.result = result;
            return res;
        }
    }
}
