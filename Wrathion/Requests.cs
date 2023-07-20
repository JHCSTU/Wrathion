using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
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
            data.Add("ended", "1");
            var text = Requests.Post(url, data);
            var obj = JObject.Parse(text);
            return obj["data"]?[0]?["userProjectId"]?.ToString();
        }

        public static List<string> GetCourse()
        {
            var url = "https://weiban.mycourse.cn/pharos/usercourse/listCourse.do";
            var result = new List<string>();
            foreach (var code in GetCategoryList())
            {
                var data = new Dictionary<string, string>()
                {
                    { "userId", GetValue("userId") },
                    { "tenantCode", GetValue("tenantCode") },
                    { "chooseType", "3" },
                    { "userProjectId", GetValue("userProjectId") },
                    { "name", "" },
                    { "categoryCode", code }
                };
                var text = Post(url, data);
                var obj = JObject.Parse(text)["data"];
                foreach (var item in obj)
                {
                    //Todo Publish == 2
                    if (Convert.ToInt32(item["finished"]) == 2)
                    {
                        result.Add(item["resourceId"]?.ToString());
                    }
                }
            }

            return result;
        }

        public static Dictionary<string, string> GetFinishIdList()
        {
            var url = "https://weiban.mycourse.cn/pharos/usercourse/listCourse.do";
            var result = new Dictionary<string, string>();
            foreach (var code in GetCategoryList())
            {
                var data = new Dictionary<string, string>()
                {
                    { "userId", GetValue("userId") },
                    { "tenantCode", GetValue("tenantCode") },
                    { "chooseType", "3" },
                    { "userProjectId", GetValue("userProjectId") },
                    { "name", "" },
                    { "categoryCode", code }
                };
                var text = Post(url, data);
                var obj = JObject.Parse(text)["data"];
                foreach (var item in obj)
                {
                    //Todo Publish == 2
                    if (Convert.ToInt32(item["finished"]) == 2)
                    {
                        result.Add(item["resourceId"]?.ToString() ?? string.Empty, item["userCourseId"]?.ToString());
                    }
                }
            }

            return result;
        }

        static JToken GetCategory()
        {
            var r = new List<string>();
            var url = "https://weiban.mycourse.cn/pharos/usercourse/listCategory.do";
            var data = new Dictionary<string, string>()
            {
                { "userId", GetValue("userId") },
                { "tenantCode", GetValue("tenantCode") },
                { "chooseType", "3" },
                { "userProjectId", GetValue("userProjectId") }
            };
            var text = Post(url, data);
            var obj = JObject.Parse(text)["data"];
            return obj;
        }

        public static List<string> GetCategoryList()
        {
            var r = new List<string>();
            foreach (var item in GetCategory())
            {
                var totalNum = Convert.ToInt32(item["totalNum"]);
                var finishedNum = Convert.ToInt32(item["finishedNum"]);
                if (totalNum - finishedNum == 0)
                {
                    r.Add(item["categoryCode"]?.ToString());
                }
            }

            return r;
        }

        public static bool Start(string courseId)
        {
            var url = "https://weiban.mycourse.cn/pharos/usercourse/study.do";
            var data = new Dictionary<string, string>()
            {
                { "courseId", courseId },
                { "userId", GetValue("userId") },
                { "tenantCode", GetValue("tenantCode") },
                { "userProjectId", GetValue("userProjectId") }
            };
            var text = Post(url, data);
            var obj = JObject.Parse(text);
            return Convert.ToInt32(obj["code"]) != -1;
        }

        public static void Finish(string finishId)
        {
            var url = "https://weiban.mycourse.cn/pharos/usercourse/finish.do";
            var data = new Dictionary<string, string>()
            {
                { "callback", "" },
                { "tenantCode", GetValue("tenantCode") },
                { "userCourseId", finishId }
            };
            Post(url, data);
        }

        public static void Run()
        {
            //Main 
            var finishIdList = Requests.GetFinishIdList();
            var courseList = Requests.GetCourse();
            for (int i = 0; i < courseList.Count; i++)
            {
                // Judge the state
                while (Requests.Start(courseList[i]) == false)
                {
                    //Sleep
                    Thread.Sleep(5000);
                }

                //Wait to Vert
                Thread.Sleep(20000);
                //Send Finish Data
                Requests.Finish(finishIdList[courseList[i]]);
            }
        }
    }
}