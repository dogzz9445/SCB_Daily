using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SCB.Cores
{
    public static class RequestHelper
    {
        public static async Task DownloadAsync(string url, string filename)
        {
            using var client = new HttpClient();
            using var s = await client.GetStreamAsync(url);
            using var fs = new FileStream(filename, FileMode.OpenOrCreate);
            await s.CopyToAsync(fs);
        }

        public static async Task<string> RequestAsync(
            string url, 
            string content = null,
            string username = null, 
            string password = null, 
            string method = "Get"
            )
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.KeepAlive = false;
                request.ContentType = "application/json; charset=UTF-8";
                if (!string.IsNullOrEmpty(username))
                {
                    var svcCredential = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{username}:{password}"));
                    request.Headers.Add("Authorization", "Basic " + svcCredential);
                }
                //request.Headers.Add("Accept", "*/*");
                request.Method = method;
                if (!string.IsNullOrEmpty(content))
                {
                    var reqBody = Encoding.UTF8.GetBytes(content);
                    request.ContentLength = reqBody.Length;
                    var reqStream = request.GetRequestStream();
                    await reqStream.WriteAsync(reqBody, 0, reqBody.Length);
                    reqStream.Close();
                }

                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return null;
                return new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (WebException exception) 
            {
#if UNITY_ENGINE
                Debug.LogError($"Error request error {exception.Message}");
#else
                Console.WriteLine($"Error request error {exception.Message}");
#endif
            }

            return null;
        }
    }
}