using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using BitcoinApi.Database;
using BitcoinApi.Enums;
using BitcoinApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BitcoinApi.Methods
{
    public class Utils
    {
        
        private static  string _bitcoindAddress="http://localhost:18443";
        //Get information about transaction and convert it to new my object
        public static void GetTransactionInfo(JToken transactionId, RequestInformation data)
        {
            var webRequest = CreatePostWebRequest(data,_bitcoindAddress);

            var jObject = new JObject
            {
                new JProperty("jsonrpc", "1.0"),
                new JProperty("id", "1"),
                new JProperty("method", "gettransaction")
            };

            if (string.IsNullOrEmpty(transactionId.ToString()))
            {
                jObject.Add(new JProperty("params", new JArray()));
            }
            else
            {
                var props = new JArray {transactionId.Value<string>()};
                jObject.Add(new JProperty("params", props));
            }

            var byteArray = SerializeObject(jObject);
            webRequest.ContentLength = byteArray.Length;


            using (var dataStream = webRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            // Deserialize response  
            var deserializedResponse = DeserializeResponse(webRequest);
            SaveTransactionInformation(deserializedResponse);
           
        }

        public static JObject DeserializeResponse(HttpWebRequest webRequest)
        {
            WebResponse webResponse;
            using (webResponse = webRequest.GetResponse())
            {
                using (var stream = webResponse.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var deserializedResponse = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
                        return deserializedResponse;
                    }
                }
            }
        }

        public static HttpWebRequest CreatePostWebRequest(RequestInformation data, string bitcoindAddress)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(bitcoindAddress);
            webRequest.Credentials = new NetworkCredential(data.Username, data.Password);
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";
            webRequest.PreAuthenticate = true;
            return webRequest;
        }

        //Parsing response and converting it and saving to database
        private static void SaveTransactionInformation(JToken deserializedResponse)
        {
            var childList = deserializedResponse.First.First.Children().ToList();
            var confirmations = childList[2].First.Value<int>();
            var details = childList[9];
            var operations = details.First.Children().ToList();
            foreach (var operation in operations)
            {
                var operationChildren = operation.Children().ToList();
                var address = operationChildren[0].First.Value<string>();
                var operationType = operationChildren[1].First.Value<string>() == OperationType.Send.ToString()
                    ? OperationType.Send
                    : OperationType.Receive;
                var amount = operationChildren[2].First.Value<decimal>();
                var addressId = GetAddressId(address);

                var information = new Transaction
                {
                    OperationType = (int) operationType,
                    IdWallet = addressId,
                    Amount = amount,
                    Confirmations = confirmations,
                    Date = DateTime.Now
                };
                var response = DatabaseMethods.SaveTransaction(information);

            }
        }

        private static int GetAddressId(string address)
        {
            using (var context = new BitcoinApiContext())
            {
                var isAddressExist = context.Wallets.Any(w => w.Address == address);
                if (isAddressExist)
                    return context.Wallets.Where(w => w.Address == address).Select(w => w.Id).FirstOrDefault();

                var response = DatabaseMethods.SaveWallet(address);
                if (response.IsSuccessful)
                    return context.Wallets.Where(w => w.Address == address).Select(w => w.Id).FirstOrDefault();
                var exception = new Exception(response.Message);
                throw exception;
            }
        }


        public static bool CheckUsername(string username)
        {
            using (var context = new BitcoinApiContext())
            {
                var isUsernameExist = context.Users.Any(u => u.Username == username);
                return isUsernameExist;
            }
        }

        public static bool CheckPassword(string username, string password)
        {
            using (var context = new BitcoinApiContext())
            {
                var userId = context.Users.Where(u => u.Username == username).Select(u => u.Id).FirstOrDefault();
                var salt = context.Users.Where(u => u.Id == userId).Select(u => u.Salt).FirstOrDefault();
                var hashPasswordFormBase =
                    context.Users.Where(u => u.Id == userId).Select(u => u.Password).FirstOrDefault();
                var hashedPassword = HashPassword(salt, password);

                return hashedPassword == hashPasswordFormBase;
            }
        }

        private static string HashPassword(byte[] salt, string password)
        {
            var passwordRfc2898DeriveBytes = new Rfc2898DeriveBytes(password,salt, 10000);
            var hash = passwordRfc2898DeriveBytes.GetBytes(20);
            var hashBytes = new byte[36];

            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            var hashedPassword = Convert.ToBase64String(hashBytes);
            return hashedPassword;
        }

        public static byte[] SerializeObject(JObject joe)
        {
            var serializeObject = JsonConvert.SerializeObject(joe);
            var byteArray = Encoding.UTF8.GetBytes(serializeObject);
            return byteArray;
        }
    }
}