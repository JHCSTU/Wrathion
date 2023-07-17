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

        public static void SetHeader(string key, string value)
        {
            ReqHeader.Add(key, value);
        }

        public static string Post(string url, Dictionary<string, string> dataDic = null)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            //配置默认参数
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            req.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.41";
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
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        public static int getProgress()
        {
            var url = "https://weiban.mycourse.cn/pharos/project/showProgress.do";
            var data = new Dictionary<string, string>()
            {
                { "userProjectId", "89fcaf08-d3a0-4277-85a8-95521beffa36" },
                { "tenantCode", "32101701" },
                { "userId", "fcf3c3c2-2569-4bc1-b36c-93b9e9c0466f" },
            };
            var text = Requests.Post(url, data);
            var obj = JObject.Parse(text);
            text = obj["data"]["progressPet"].ToString();
            return Convert.ToInt32(text);
        }
    }
}