using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Http;
using BitcoinApi.Methods;
using BitcoinApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BitcoinApi.Controllers
{
    public class MainController : ApiController
    {
        private readonly string _bitcoindAddress="http://localhost:18443";

        
        public IEnumerable<string> GetLast()
        {
            var lastTransactions = DatabaseMethods.GetLastTransactions();
            var lastTransactionsSerialized = new List<string>();
            foreach (var lastTransaction in lastTransactions)
            {
                var serialzedTransaction = JsonConvert.SerializeObject(lastTransaction);
                lastTransactionsSerialized.Add(serialzedTransaction);
            }

            return lastTransactionsSerialized;
        }
        
        public JObject SendBtc([FromBody] RequestInformation data)
        {
            var webRequest = Utils.CreatePostWebRequest(data,_bitcoindAddress);

            if (!Utils.CheckUsername(data.Username) && !Utils.CheckPassword(data.Username,data.Password))
            {
                return JsonConvert.DeserializeObject<JObject>("No such user");
            }

            var jObject = new JObject
            {
                new JProperty("jsonrpc", "1.0"),
                new JProperty("id", "1"),
                new JProperty("method", "sendtoaddress")
            };

            if (string.IsNullOrEmpty(data.Address))
            {
                jObject.Add(new JProperty("params", new JArray()));
            }
            else
            {
                var props = new JArray {data.Address, data.Amount};
                jObject.Add(new JProperty("params", props));
            }
            
            var byteArray = Utils.SerializeObject(jObject);
            webRequest.ContentLength = byteArray.Length;


            using (var dataStream = webRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            try
            {
                var deserializedResponse = Utils.DeserializeResponse(webRequest);
                var transactionId = deserializedResponse.GetValue("result");
                Utils.GetTransactionInfo(transactionId, data);

                return deserializedResponse;
            }
            catch (WebException exception)
            {
                using (var stream = exception.Response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
                    }
                }
            }
        }
    }
}