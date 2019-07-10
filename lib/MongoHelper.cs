using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using MongoDB.Bson.IO;

namespace NEL.NNS.lib
{
    public class MongoHelper
    {
        public string notify_mongodbConnStr_testnet = string.Empty;
        public string notify_mongodbDatabase_testnet = string.Empty;
        public string notify_mongodbConnStr_mainnet = string.Empty;
        public string notify_mongodbDatabase_mainnet = string.Empty;
        public string swapAdmHash = string.Empty;
        public string swapExcHash = string.Empty;
        //
        public string isStartRechargeFlag = string.Empty;
        public string isStartApplyGasFlag = string.Empty;
        public string startMonitorFlag = string.Empty;

        public MongoHelper()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection()    //将配置文件的数据加载到内存中
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())   //指定配置文件所在的目录
                .AddJsonFile("mongodbsettings.json", optional: true, reloadOnChange: true)  //指定加载的配置文件
                .Build();    //编译成对象  
            //
            notify_mongodbConnStr_testnet = config["notify_mongodbConnStr_testnet"];
            notify_mongodbDatabase_testnet = config["notify_mongodbDatabase_testnet"];
            notify_mongodbConnStr_mainnet = config["notify_mongodbConnStr_mainnet"];
            notify_mongodbDatabase_mainnet = config["notify_mongodbDatabase_mainnet"];

            swapAdmHash = config["swapAdmHash"];
            swapExcHash = config["swapExcHash"];
            
            //
            startMonitorFlag = config["startMonitorFlag"];

        }

        //
        public long GetDataCount(string mongodbConnStr, string mongodbDatabase, string coll, string findStr = "{}")
        {
            var client = new MongoClient(mongodbConnStr);
            var database = client.GetDatabase(mongodbDatabase);
            var collection = database.GetCollection<BsonDocument>(coll);

            var txCount = collection.Find(BsonDocument.Parse(findStr)).CountDocuments();

            client = null;

            return txCount;
        }
        public JArray GetData(string mongodbConnStr, string mongodbDatabase, string coll, string findStr = "{}", string sortStr = "{}", int skip = 0, int limit = 0, string fieldStr = "{'_id':0}")
        {
            var client = new MongoClient(mongodbConnStr);
            var database = client.GetDatabase(mongodbDatabase);
            var collection = database.GetCollection<BsonDocument>(coll);

            List<BsonDocument> query = null;
            if (limit == 0)
            {
                query = collection.Find(BsonDocument.Parse(findStr)).Project(BsonDocument.Parse(fieldStr)).ToList();
            }
            else
            {
                query = collection.Find(BsonDocument.Parse(findStr)).Project(BsonDocument.Parse(fieldStr)).Sort(sortStr).Skip(skip).Limit(limit).ToList();
            }
            client = null;

            if (query != null && query.Count > 0)
            {
                return JArray.Parse(query.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.Strict }));
            }
            else { return new JArray(); }
        }
        public List<T> GetData<T>(string mongodbConnStr, string mongodbDatabase, string coll, string findStr = "{}", string sortStr = "{}", int skip = 0, int limit = 0)
        {
            var client = new MongoClient(mongodbConnStr);
            var database = client.GetDatabase(mongodbDatabase);
            var collection = database.GetCollection<T>(coll);

            List<T> query = null;
            if (limit == 0)
            {
                query = collection.Find(BsonDocument.Parse(findStr)).ToList();
            }
            else
            {
                query = collection.Find(BsonDocument.Parse(findStr)).Sort(sortStr).Skip(skip).Limit(limit).ToList();
            }
            client = null;

            return query;
        }

        //
        public void PutData(string mongodbConnStr, string mongodbDatabase, string coll, string data, bool isAync = false)
        {
            var client = new MongoClient(mongodbConnStr);
            var database = client.GetDatabase(mongodbDatabase);
            var collection = database.GetCollection<BsonDocument>(coll);
            if (isAync)
            {
                collection.InsertOneAsync(BsonDocument.Parse(data));
            }
            else
            {
                collection.InsertOne(BsonDocument.Parse(data));
            }

            client = null;
        }
        public void PutData<T>(string mongodbConnStr, string mongodbDatabase, string coll, T data, bool isAync = false)
        {
            var client = new MongoClient(mongodbConnStr);
            var database = client.GetDatabase(mongodbDatabase);
            var collection = database.GetCollection<T>(coll);
            if (isAync)
            {
                collection.InsertOneAsync(data);
            }
            else
            {
                collection.InsertOne(data);
            }

            client = null;
        }
        public void PutData(string mongodbConnStr, string mongodbDatabase, string coll, JObject JO)
        {
            var client = new MongoClient(mongodbConnStr);
            var database = client.GetDatabase(mongodbDatabase);
            var collection = database.GetCollection<BsonDocument>(coll);

            collection.InsertOne(BsonDocument.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(JO)));

            client = null;
        }
        public void PutData(string mongodbConnStr, string mongodbDatabase, string coll, JArray JA)
        {
            var client = new MongoClient(mongodbConnStr);
            var database = client.GetDatabase(mongodbDatabase);
            var collection = database.GetCollection<BsonDocument>(coll);

            List<BsonDocument> bsons = JA.Select(p => BsonDocument.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(p))).ToList();
            collection.InsertMany(bsons);

            client = null;
        }

        //
        public void UpdateData(string mongodbConnStr, string mongodbDatabase, string coll, string updateStr, string whereStr)
        {
            var client = new MongoClient(mongodbConnStr);
            var database = client.GetDatabase(mongodbDatabase);
            var collection = database.GetCollection<BsonDocument>(coll);
            collection.UpdateOne(BsonDocument.Parse(whereStr), BsonDocument.Parse(updateStr));

            client = null;
        }

        //
        public void ReplaceData(string mongodbConnStr, string mongodbDatabase, string coll, string replaceStr, string whereStr)
        {
            var client = new MongoClient(mongodbConnStr);
            var database = client.GetDatabase(mongodbDatabase);
            var collection = database.GetCollection<BsonDocument>(coll);

            collection.ReplaceOne(BsonDocument.Parse(whereStr), BsonDocument.Parse(replaceStr));

            client = null;
        }
        public void ReplaceData<T>(string mongodbConnStr, string mongodbDatabase, string coll, T data, string whereStr)
        {
            var client = new MongoClient(mongodbConnStr);
            var database = client.GetDatabase(mongodbDatabase);
            var collection = database.GetCollection<T>(coll);

            collection.ReplaceOne(BsonDocument.Parse(whereStr), data);

            client = null;
        }

        //
        public void DeleteData(string mongodbConnStr, string mongodbDatabase, string coll, string whereStr)
        {
            var client = new MongoClient(mongodbConnStr);
            var database = client.GetDatabase(mongodbDatabase);
            var collection = database.GetCollection<BsonDocument>(coll);

            collection.DeleteOne(BsonDocument.Parse(whereStr));

            client = null;
        }

        //
        public void setIndex(string mongodbConnStr, string mongodbDatabase, string coll, string indexDefinition, string indexName, bool isUnique = false)
        {
            var client = new MongoClient(mongodbConnStr);
            var database = client.GetDatabase(mongodbDatabase);
            var collection = database.GetCollection<BsonDocument>(coll);

            //检查是否已有设置idnex
            bool isSet = false;
            using (var cursor = collection.Indexes.List())
            {
                JArray JAindexs = JArray.Parse(cursor.ToList().ToJson());
                var query = JAindexs.Children().Where(index => (string)index["name"] == indexName);
                if (query.Count() > 0) isSet = true;
                // do something with the list...
            }

            if (!isSet)
            {
                try
                {
                    var options = new CreateIndexOptions { Name = indexName, Unique = isUnique };
                    collection.Indexes.CreateOne(indexDefinition, options);
                }
                catch { }
            }

            client = null;
        }
        //
        public List<string> listCollection(string mongodbConnStr, string mongodbDatabase)
        {
            var client = new MongoClient(mongodbConnStr);
            var database = client.GetDatabase(mongodbDatabase);
            List<string> list = database.ListCollectionNames().ToList();
            client = null;

            return list;
        }

        //
        private string AggregateCountStr = new JObject { { "$group", new JObject { { "_id", 1 }, { "sum", new JObject { { "$sum", 1 } } } } } }.ToString();
        public long AggregateCount(string mongodbConnStr, string mongodbDatabase, string coll, IEnumerable<string> collection)
        {
            var res = Aggregate(mongodbConnStr, mongodbDatabase, coll, collection, true);
            if (res != null && res.Count > 0)
            {
                return long.Parse(res[0]["sum"].ToString());
            }
            return 0;
        }
        public JArray Aggregate(string mongodbConnStr, string mongodbDatabase, string coll, IEnumerable<string> collection, bool isGetCount = false)
        {
            IList<IPipelineStageDefinition> stages = new List<IPipelineStageDefinition>();
            foreach (var item in collection)
            {
                stages.Add(new JsonPipelineStageDefinition<BsonDocument, BsonDocument>(item));
            }
            if (isGetCount)
            {
                stages.Add(new JsonPipelineStageDefinition<BsonDocument, BsonDocument>(AggregateCountStr));
            }
            PipelineDefinition<BsonDocument, BsonDocument> pipeline = new PipelineStagePipelineDefinition<BsonDocument, BsonDocument>(stages);
            var queryRes = Aggregate(mongodbConnStr, mongodbDatabase, coll, pipeline);
            if (queryRes != null && queryRes.Count > 0)
            {
                return JArray.Parse(queryRes.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.Strict }));
            }
            return new JArray { };
        }
        public List<BsonDocument> Aggregate(string mongodbConnStr, string mongodbDatabase, string coll, PipelineDefinition<BsonDocument, BsonDocument> pipeline)
        {
            var client = new MongoClient(mongodbConnStr);
            var database = client.GetDatabase(mongodbDatabase);
            var collection = database.GetCollection<BsonDocument>(coll);
            var query = collection.Aggregate(pipeline).ToList();

            client = null;
            return query;
        }

    }
}
