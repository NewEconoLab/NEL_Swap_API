using NEL.NNS.lib;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace NEL_Swap_API.Service
{
    public class ExchangeService
    {
        public MongoHelper mh { get; set; }
        public string notify_mongodbConnStr { get; set; }
        public string notify_mongodbDatabase { get; set; }
        public string assetInfoCol { get; set; } = "swapAssetInfo";
        public string exchangeRateInfoCol { get; set; } = "swapExchangeRateInfo";
        public string exchangePriceInfoCol { get; set; } = "swapExchangePriceInfo";
        public string poolInfoCol { get; set; } = "swapPoolInfo";
        public string swapAdmCol { get; set; } = "";
        public string swapExcCol { get; set; } = "";

        public JArray getUinTotal(string hash, string address = "{}")
        {
            var findJo = new JObject { { "hash", hash } };
            if(address != "{}")
            {
                findJo.Add("address", address);
            }
            string findStr = findJo.ToString();
            var list = new List<string>();
            

            return null;
        }
        public JArray getLiquidityRate(string assetHash, string tokenHash)
        {
            if (!assetHash.StartsWith("0x")) assetHash = "0x" + assetHash;
            if (!tokenHash.StartsWith("0x")) tokenHash = "0x" + tokenHash;
            string findStr = new JObject {
                { "assetHash", assetHash},
                { "tokenHash", tokenHash},
            }.ToString();
            string sortStr = "{}";
            string fieldStr = new JObject { { "ratio", 1 }, { "_id", 0 } }.ToString();
            var queryRes = mh.GetData(notify_mongodbConnStr, notify_mongodbDatabase, swapAdmCol, findStr, sortStr, 0,1, fieldStr);
            if (queryRes.Count == 0) return queryRes;

            var res = new JArray { new JObject {
                {"ratio", queryRes[0]["ratio"].ToString()},
                {"exchangeFee", (decimal.Parse(queryRes[0]["ratio"].ToString()) * 10000).ToString() }
            } };
            return res;
        }
        public JArray getLiquidityHash(string assetHash, string tokenHash)
        {
            if (!assetHash.StartsWith("0x")) assetHash = "0x" + assetHash;
            if (!tokenHash.StartsWith("0x")) tokenHash = "0x" + tokenHash;
            string findStr = new JObject {
                { "assetHash", assetHash},
                { "tokenHash", tokenHash},
            }.ToString();
            string sortStr = "{}";
            string fieldStr = new JObject { { "hash",1},{ "_id",0} }.ToString();
            var queryRes = mh.GetData(notify_mongodbConnStr, notify_mongodbDatabase, poolInfoCol, findStr, sortStr, 0,1,fieldStr);
            return queryRes;
        }
        public JArray getLiquidityInfo(string assetHash, string tokenHash)
        {
            if (!assetHash.StartsWith("0x")) assetHash = "0x" + assetHash;
            if (!tokenHash.StartsWith("0x")) tokenHash = "0x" + tokenHash;
            string findStr = new JObject {
                { "assetHash", assetHash},
                { "tokenHash", tokenHash},
            }.ToString();
            string sortStr = new JObject { { "time", -1 } }.ToString();
            string fieldStr = new JObject { { "assetTotal", 1 }, { "tokenTotal", 1 },{ "_id",0} }.ToString();
            var queryRes = mh.GetData(notify_mongodbConnStr, notify_mongodbDatabase, poolInfoCol, findStr, sortStr, 0, 1, fieldStr);
            return queryRes;
        }

        public JArray getExchangePrice()
        {
            string findStr = "{}";
            string sortStr = "{'time': -1}";
            var queryRes = mh.GetData(notify_mongodbConnStr, notify_mongodbDatabase, exchangePriceInfoCol, findStr, sortStr, 0, 1);
            if (queryRes == null || queryRes.Count == 0) return new JArray { };

            var price = queryRes[0]["last"].ToString();

            return new JArray { new JObject { { "instrument_id","GAS-USDT"}, { "price", price } } };
        }
        public JArray getExchangeRate()
        {
            string findStr = "{}";
            string sortStr = "{'time': -1}";
            var queryRes = mh.GetData(notify_mongodbConnStr, notify_mongodbDatabase, exchangeRateInfoCol, findStr, sortStr, 0, 1);
            if (queryRes == null || queryRes.Count == 0) return new JArray { };

            var rate = queryRes[0]["exchangeT"].ToString();

            return new JArray { new JObject { { "currency_pair", "USDCNY"},{"rate",rate } } };
        }
        public JArray getAssetList(string assetName, int pageNum=1, int pageSize=10)
        {
            string findStr = "{}";
            if (assetName != "")
            {
                findStr = new JObject() { { "symbol", new JObject() { { "$regex", assetName }, { "$options", "i" } } } }.ToString();
            }
            string sortStr = new JObject { {"name",1} }.ToString();
            string fieldStr = new JObject { { "assetid", 1 }, { "name", 1 }, { "symbol", 1 },{ "decimals",1}, { "picUrl", 1 }, { "_id", 0 } }.ToString();
            var queryRes = mh.GetData(notify_mongodbConnStr, notify_mongodbDatabase, assetInfoCol, findStr, sortStr, pageSize*(pageNum-1),pageSize, fieldStr);
            if (queryRes == null || queryRes.Count == 0) return new JArray { };

            var count = mh.GetDataCount(notify_mongodbConnStr, notify_mongodbDatabase, assetInfoCol, findStr);
            
            return new JArray { new JObject {
                { "count", count},
                { "list", queryRes}
            } };
        }
    }
}
