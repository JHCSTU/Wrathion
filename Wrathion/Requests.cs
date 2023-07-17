using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace Wrathion
{
    public class Requests
    {
        private static Dictionary<string, string> ReqHeader = new Dictionary<string, string>();
        private static Dictionary<string, string> DataList = new Dictionary<string, string>();

        public static void SetValue(string key, string value)
        {
            DataList[key] = value;
        }

        public static string GetValue(string key)
        {
            return DataList[key];
        }

        public static void SetHeader(string key, string value)
        {
            ReqHeader[key] = value;
        }

        public static string GetHeader(string key)
        {
            return ReqHeader[key];
        }

        public static string Post(string url, Dictionary<string, string> dataDic = null)
        {
            string result = null;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            //配置默认参数
            req.ServicePoint.Expect100Continue = false;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            req.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36 Edg/114.0.1823.82";
            StringBuilder sb = new StringBuilder();
            int i = 0;
            //导入Header参数

            foreach (var item in ReqHeader)
            {
                req.Headers.Add(item.Key, item.Value);
            }

            //导入提交数据
            if (dataDic != null)
                foreach (var item in dataDic)
                {
                    if (i != 0)
                    {
                        sb.Append("&");
                    }

                    sb.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }

            //获取数据
            byte[] data = Encoding.UTF8.GetBytes(sb.ToString());
            req.ContentLength = data.Length;
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(data, 0, data.Length);
            reqStream.Flush();
            reqStream.Close();
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            if (stream != null)
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }

            return result;
        }

        //根据所需Key 生成请求数据
        // public static Dictionary<string, string> GenerateData(params string[] keys)
        // {
        //     var data = new Dictionary<string, string>();
        //     foreach (var item in keys)
        //     {
        //         data.Add(item, GetValue(item));
        //     }
        //
        //     return data;
        // }

        public static int GetProgress()
        {
            var url = "https://weiban.mycourse.cn/pharos/project/showProgress.do";
            var data = new Dictionary<string, string>()
            {
                { "userProjectId", GetValue("userProjectId") },
                { "tenantCode", GetValue("tenantCode") },
                { "userId", GetValue("userId") },
            };
            var text = Post(url, data);
            var obj = JObject.Parse(text);
            text = obj["data"]?["progressPet"]?.ToString();
            return Convert.ToInt32(text);
        }

        public static string GetProjectId()
        {
            var url = "https://weiban.mycourse.cn/pharos/index/listMyProject.do";
            var data = new Dictionary<string, string>()
            {
                { "userId", GetValue("userId") },
                { "tenantCode", GetValue("tenantCode") }
            };
            //Todo 2为未完成 1为已完成
            data.Add("ended", "1");
            var text = Requests.Post(url, data);
            var obj = JObject.Parse(text);
            return obj["data"]?[0]?["userProjectId"]?.ToString();
        }

        public static void test()
        {
            MessageBox.Show(GetProgress().ToString());
        }
    }
}