using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace HYAutomation.Core.Web
{
    public class WebApiHelper
    {
        /// <summary>
        /// 发送Get请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="dic">请求参数定义</param>
        /// <returns></returns>
        public static string HttpGet(string url, Dictionary<string, string> dic, int timeout = 8000)
        {
            string result = "";
            StringBuilder builder = new StringBuilder();
            builder.Append(url);
            if (dic!=null&&dic.Count > 0)
            {
                builder.Append("?");
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(builder.ToString());
            req.Timeout = timeout;
            req.Method= "GET";
            //添加参数
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            try
            {
                //获取内容
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            finally
            {
                stream.Close();
            }
            return result;
        }
        /// <summary>
        /// json格式读取数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="body">参数</param>
        /// <param name="timeout">超时</param>
        /// <returns></returns>
        public static string HttpJsonPost(string url, string body, int timeout = 8000)
        {
            try
            {
                Encoding encoding = Encoding.UTF8;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = timeout;
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                byte[] buffer = encoding.GetBytes(body);
                request.ContentLength = buffer.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// json格式读取数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="body">参数</param>
        /// <param name="timeout">超时</param>
        /// <returns></returns>
        public static async Task<string> HttpJsonPostAsync(string url, string body, int timeout = 8000)
        {
            try
            {
                Encoding encoding = Encoding.UTF8;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = timeout;
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                byte[] buffer = encoding.GetBytes(body);
                request.ContentLength = buffer.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 表单格式读取数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="paramData">参数</param>
        /// <returns></returns>
        public static string HttpFormPost(string url, string paramData)
        {
            try
            {
                string ret = string.Empty;
                byte[] byteArray = Encoding.Default.GetBytes(paramData); //转化 /
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";

                webReq.ContentLength = byteArray.Length;
                using (Stream newStream = webReq.GetRequestStream())
                {
                    newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                }
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    ret = sr.ReadToEnd();
                }
                response.Close();
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 表单格式读取数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="paramData">参数</param>
        /// <returns></returns>
        public static async Task<string> HttpFormPostAsync(string url, string paramData)
        {
            try
            {
                string ret = string.Empty;
                byte[] byteArray = Encoding.Default.GetBytes(paramData); //转化 /
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";

                webReq.ContentLength = byteArray.Length;
                using (Stream newStream = webReq.GetRequestStream())
                {
                    newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                }
                HttpWebResponse response = (HttpWebResponse)await webReq.GetResponseAsync();
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    ret = sr.ReadToEnd();
                }
                response.Close();
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string PostMultipartFormData(string url, Dictionary<string, string> headers, NameValueCollection nameValueCollection, NameValueCollection fileCollection)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    foreach (var item in headers)
                    {
                        client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }

                    using (var content = new MultipartFormDataContent())
                    {
                        // text参数
                        string[] allKeys = nameValueCollection.AllKeys;
                        foreach (string key in allKeys)
                        {
                            var dataContent = new ByteArrayContent(Encoding.UTF8.GetBytes(nameValueCollection[key]));
                            dataContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                            {
                                Name = key
                            };
                            content.Add(dataContent);
                        }

                        //file参数
                        string[] fileKeys = fileCollection.AllKeys;
                        foreach (string key in fileKeys)
                        {
                            byte[] bmpBytes = File.ReadAllBytes(fileCollection[key]);
                            var fileContent = new ByteArrayContent(bmpBytes);//字节流
                            fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                            {
                                Name = key,
                                FileName = Path.GetFileName(fileCollection[key])
                            };
                            content.Add(fileContent);
                        }

                        var result = client.PostAsync(url, content).Result;
                        string data = result.Content.ReadAsStringAsync().Result;
                        return data;
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static string UploadImageFile(string url, string imgPath, string fileparameter = "file")
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.AllowAutoRedirect = true;
                request.Method = "POST";

                string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
                request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
                byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
                byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

                int pos = imgPath.LastIndexOf("/");
                string fileName = imgPath.Substring(pos + 1);

                //请求头部信息
                StringBuilder sbHeader = new StringBuilder(string.Format("Content-Disposition:form-data;name=\"" + fileparameter + "\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n", fileName));
                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());

                FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
                byte[] bArr = new byte[fs.Length];
                fs.Read(bArr, 0, bArr.Length);
                fs.Close();

                Stream postStream = request.GetRequestStream();
                postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                postStream.Write(bArr, 0, bArr.Length);
                postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                postStream.Close();

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream instream = response.GetResponseStream();
                StreamReader sr = new StreamReader(instream, Encoding.UTF8);
                string content = sr.ReadToEnd();
                return content;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static string UploadImageFile(string url, byte[] imgBuffer, string fileName, string fileparameter = "file")
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.AllowAutoRedirect = true;
                request.Method = "POST";

                string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
                request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
                byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
                byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

                //请求头部信息
                StringBuilder sbHeader = new StringBuilder(string.Format("Content-Disposition:form-data;name=\"" + fileparameter + "\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n", fileName));
                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());

                Stream postStream = request.GetRequestStream();
                postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                postStream.Write(imgBuffer, 0, imgBuffer.Length);
                postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                postStream.Close();

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream instream = response.GetResponseStream();
                StreamReader sr = new StreamReader(instream, Encoding.UTF8);
                string content = sr.ReadToEnd();
                return content;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static void HttpDownLoadFile(string url,int timeout = 600000)
        {
            try
            {
               string urlText= HttpUtility.UrlDecode(url);
                string str=urlText.Substring(0,urlText.IndexOf("?"));
               string fileName = str.Substring(str.LastIndexOf('/')+1);
                 Encoding encoding = Encoding.UTF8;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = timeout;
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                Stream instream = response.GetResponseStream();
                MemoryStream outstream = new MemoryStream();
                const int bufferLen = 4096;
                byte[] buffer = new byte[bufferLen];
                int count = 0;
                while ((count = instream.Read(buffer, 0, bufferLen)) > 0)
                {
                    outstream.Write(buffer, 0, count);
                }
                outstream.Seek(0, SeekOrigin.Begin);
                int buffsize = (int)outstream.Length;
                byte[] bytes = new byte[buffsize];
                outstream.Read(bytes, 0, buffsize);
                outstream.Flush(); outstream.Close();
                instream.Flush(); instream.Close();
                using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                {
                    fileStream.Write(bytes, 0, bytes.Length);
                    fileStream.Close();
                }
                //using (StreamReader reader = new StreamReader(stream, encoding))
                //{
                //    File.WriteAllText(fileName, reader.ReadToEnd());
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 从远程服务器上下载读取文件，写入到本地
        /// </summary>
        /// <param name="serviceUrl">远程服务地址</param>
        /// <returns></returns>
        public static void WebDownLoadFile(string serviceUrl)
        {
            string urlText = HttpUtility.UrlDecode(serviceUrl);
            string str = urlText.Substring(0, urlText.IndexOf("?"));
            string fileName = str.Substring(str.LastIndexOf('/') + 1);
            WebClient client = new WebClient();
            byte[] buffer = client.DownloadData(serviceUrl);
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                fileStream.Write(buffer, 0, buffer.Length);
                fileStream.Close();
            }
            client.Dispose();
        }

    }
}
