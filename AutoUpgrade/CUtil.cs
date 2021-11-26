using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdater
{
    public class CUtil
    {
        public static string GetMD5HashFromFile(string filePath)
        {
            try
            {
                FileStream file = new FileStream(filePath, System.IO.FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                //throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
                return "";
            }
        }



        public static string GetDirName(string filePath)
        {
            int subIdx = int.MaxValue;
            int charIdx = -1;
            bool flag = false;
            for (int i = filePath.Length - 1; i >= 0; i--)
            {
                if (filePath[i] == '/' || filePath[i] == '\\')
                {
                    if (flag)
                    {
                        subIdx = i;
                        break;
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        subIdx = i - 1;
                    }
                    if (!flag)
                    {
                        charIdx = i;
                    }
                    flag = true;
                }
            }
            if (subIdx != int.MaxValue)
            {
                return filePath.Substring(subIdx + 1, charIdx + 1 - (subIdx + 1));
            }
            return "";
        }

        public static string GetFileName(string filePath)
        {
            int subIdx = filePath.LastIndexOf('/');
            if (subIdx == -1)
            {
                subIdx = filePath.LastIndexOf('\\');
            }
            if (subIdx != -1)
            {
                return filePath.Substring(subIdx + 1);
            }
            return "";
        }
        public static string GetFileParentDirPath(string filePath)
        {
            int subIdx = filePath.LastIndexOf('/');
            if (subIdx == -1)
            {
                subIdx = filePath.LastIndexOf('\\');
            }
            if (subIdx != -1)
            {
                return filePath.Substring(0, subIdx);
            }
            return "";
        }


        public static bool HttpGetFile(string url, string filePath, bool cover)
        {
            try
            {
                string name = CUtil.GetFileName(filePath);
                string dir = CUtil.GetFileParentDirPath(filePath);
                if (!cover && File.Exists(filePath))
                {//如果已经存在，那么就不需要拷贝了，如果没有，那么就进行拷贝
                    return true;
                }
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                request.ProtocolVersion = new Version(1, 1);
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;//找不到则直接返回null
                }
                // 转换为byte类型
                System.IO.Stream stream = response.GetResponseStream();
                //创建本地文件写入流
                Stream fs = new FileStream(filePath, FileMode.Create);
                byte[] bArr = new byte[51200];
                int size = stream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    fs.Write(bArr, 0, size);
                    size = stream.Read(bArr, 0, (int)bArr.Length);
                }
                fs.Close();
                stream.Close();
            }
            catch (Exception exp)
            {
                return false;
            }
            return true;
        }

        public static string HttpGetJson(string url, string para)
        {
            string str = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);//创建请求对象
                request.Method = "Post";//请求方式
                request.KeepAlive = true;
                request.ContentType = "application/json";//请求头参数
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(para);//设置请求参数
                request.ContentLength = bytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(bytes, 0, bytes.Length);//写入参数
                stream.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())//响应对象
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    str = reader.ReadToEnd();//获取返回的页面信息
                }
            }
            catch (Exception exp)
            {

            }
            return str;
        }

    }
}
