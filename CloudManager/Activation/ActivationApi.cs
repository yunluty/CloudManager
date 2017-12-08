using RestSharp;
using System;

namespace CloudManager.Activation
{
    class ActivationApi
    {
        public class ResponseLife
        {
            public int Time { get; set; }
        }

        private static readonly RestClient mRestClient = new RestClient("http://39.108.229.176/");

        public const long TICKSBYSECOND = 10000000L;
        public const int RunCondition = 300;
        public static int KeyLife;

        public static int ActivateKey(string aki, string key)
        {
            var request = new RestRequest($"cloudbak/activate?akiUID={aki}&cpy={aki}&cdkey={key}");
            var response = mRestClient.Execute<ResponseLife>(request);
            if (response.ErrorException != null) throw response.ErrorException;
            else if (response.StatusCode != System.Net.HttpStatusCode.Created) throw new Exception("激活服务异常");
            else if (response.Data == null) throw new Exception("激活服务数据异常");
            return KeyLife = response.Data.Time;
        }

        public static int GetKeyLife(string aki)
        {
            var request = new RestRequest($"cloudbak/loginvalidate?akiUID={aki}");
            var response = mRestClient.Execute<ResponseLife>(request);
            if (response.ErrorException != null) throw response.ErrorException;
            //if (response.StatusCode != HttpStatusCode.OK) throw new Exception("Key服务异常");
            if (response.Data == null) throw new Exception("激活服务数据异常");
            return KeyLife = response.Data.Time;
        }
    }
}
