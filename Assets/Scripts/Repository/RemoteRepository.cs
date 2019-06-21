using System;
using System.Collections.Generic;
using System.Text;
using Assets.Scripts.Infrastructure;
using UnityEngine;
using Assets.Scripts.Network;
using UnityEngine;
using Newtonsoft.Json;
namespace Assets.Scripts.Repository
{
    public class RemoteRepository<T> where T : class, new()
    {
        public ISerializer Serializer { get; set; }

        public RemoteRepository(ISerializer serializer = null)
        {
            Serializer = serializer ?? SerializerJson.Instance;
        }
        public void Get<R>(string url, T instance, Action<R> onSuccess) where R : class, new()
        {
            var parameters = HttpUtility.BuildParameters(instance, new StringBuilder("?"));
            var httpRequest = new HttpRequest
            {
                Url = url,
                Method = HttpMethod.Get,
                Parameters = parameters
            };
            Debug.Log(httpRequest.Url + httpRequest.Parameters);
            HttpClient.Instance.SendAsync(httpRequest, httpResponse =>
            {
                if (httpResponse.IsSuccess)
                {
                    R r = JsonConvert.DeserializeObject<R>(httpResponse.Data);
                    onSuccess(r);
                }
                //TODO:异常处理
            });
        }

        public void Get<R>(string url, Action<R> onSuccess) where R : class, new()
        {
            // var parameters = HttpUtility.BuildParameters(instance, new StringBuilder("?"));
            var httpRequest = new HttpRequest
            {
                Url = url,
                Method = HttpMethod.Get,
                // Parameters = parameters
            };
            HttpClient.Instance.SendAsync(httpRequest, httpResponse =>
            {
                if (httpResponse.IsSuccess)
                {
                    R r = JsonConvert.DeserializeObject<R>(httpResponse.Data);
                    Debug.Log("json str :" + r);
                    onSuccess(r);
                }
                //TODO:异常处理
            });
        }

        public void Post<R>(string url, T instance, Action<R> onSuccess) where R : class, new()
        {
            var parameters = HttpUtility.BuildParameters(instance, new StringBuilder());
            var httpRequest = new HttpRequest
            {
                Url = url,
                Method = HttpMethod.Post,
                Parameters = parameters
            };
            HttpClient.Instance.SendAsync(httpRequest, httpResponse =>
            {
                if (httpResponse.IsSuccess)
                {
                    //TODO:判断是否有Data
                    onSuccess(Serializer.Deserialize<R>(httpResponse.Data));
                }
            });
        }

        public void Test()
        {
            Debug.Log("Hello...");
        }
    }
}
