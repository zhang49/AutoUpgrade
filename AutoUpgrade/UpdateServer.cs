using HttpServerLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static AutoUpdater.Models;

namespace AutoUpdater
{
    public class UpdateServer
    {
        public static UpgradeCfg mUpgradeCfg = new UpgradeCfg();
        private static DirVersionInfo mDirVersionInfo = new DirVersionInfo();
        private static Mutex mFileVersionJsonMtx = new Mutex();
        private static string mDirVersionJsonStr = "";
        private bool mIsStop = false;
        private Thread mCheckFileVersionTh;
        private static Webserver mServer;

        public UpdateServer(UpgradeCfg cfg)
        {
            mUpgradeCfg = cfg;
        }
        public void Stop()
        {
            mIsStop = true;
            if (mCheckFileVersionTh?.IsAlive ?? false)
            {
                mCheckFileVersionTh.Join();
            }
        }

        private void InitHttpServer()
        {
            mServer = new Webserver(mUpgradeCfg.Address, mUpgradeCfg.Port, false, null, null, DefaultRoute);
            mServer.Settings.Headers.Host = mUpgradeCfg.HttpServerHost;
            mServer.Start();
            Console.WriteLine("HttpServerLite listening on {0}", mUpgradeCfg.HttpServerHost);
        }

        public static async Task DefaultRoute(HttpContext ctx)
        {
            string url = ctx.Request.Url.WithoutQuery;
            if (url.StartsWith("/file/"))
            {
                string filePath = "";
                lock (mUpgradeCfg.Lock)
                {
                    filePath = mUpgradeCfg.SyncDir + "/" + url.Substring("/file/".Length);
                }
                SendFile(ctx, filePath, "application/text");
                return;
            }
            else if (url.StartsWith("/get-files-version"))
            {
                ctx.Response.StatusCode = 200;
                mFileVersionJsonMtx.WaitOne();
                ctx.Response.ContentLength = mDirVersionJsonStr.Length;
                ctx.Response.ContentType = "application/json";
                ctx.Response.Send(mDirVersionJsonStr);
                mFileVersionJsonMtx.ReleaseMutex();
                return;
            }
            else
            {
                string resp = "Welcome AutoUpgrade Server!";
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentLength = resp.Length;
                ctx.Response.ContentType = "text/plain";
                ctx.Response.Send(resp);
            }
        }

        public static void SendFile(HttpContext ctx, string file, string contentType)
        {
            long contentLen = new FileInfo(file).Length;
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                ctx.Response.ContentType = contentType;
                ctx.Response.StatusCode = 200;
                ctx.Response.Send(contentLen, fs);
            }
        }

        public void Init()
        {
            InitServerCheckFileVersion();
            InitHttpServer();
        }

        private void InitServerCheckFileVersion()
        {
            mCheckFileVersionTh = new Thread(new ThreadStart(() =>
            {
                long lastTick = DateTime.Now.ToFileTimeUtc() / 10000;
                ServerCheckFileVersionProcess();
                while (!mIsStop)
                {
                    long freeTime = (DateTime.Now.ToFileTimeUtc() / 10000) - lastTick;
                    int checkIntravel = 5000;
                    lock (mUpgradeCfg.Lock)
                    {
                        checkIntravel = mUpgradeCfg.CheckIntravel;
                    }
                    if (freeTime > checkIntravel)
                    {
                        ServerCheckFileVersionProcess();
                        lastTick = DateTime.Now.ToFileTimeUtc() / 10000;
                    }
                    Thread.Sleep(50);
                }
            }));
            mCheckFileVersionTh.Start();
        }

        private static string getJsonByObject(Object obj)
        {
            //实例化DataContractJsonSerializer对象，需要待序列化的对象类型
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            //实例化一个内存流，用于存放序列化后的数据
            MemoryStream stream = new MemoryStream();
            //使用WriteObject序列化对象
            serializer.WriteObject(stream, obj);
            //写入内存流中
            byte[] dataBytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(dataBytes, 0, (int)stream.Length);
            //通过UTF8格式转换为字符串
            return Encoding.UTF8.GetString(dataBytes);
        }

        private void ServerCheckFileVersionProcess()
        {
            string upgradeBaseDir = "";
            lock (mUpgradeCfg.Lock)
            {
                upgradeBaseDir = mUpgradeCfg.SyncDir;
            }
            mDirVersionInfo.Clear();
            GetFilesVersionInfo(upgradeBaseDir, mDirVersionInfo);
            mFileVersionJsonMtx.WaitOne();
            mDirVersionJsonStr = getJsonByObject(mDirVersionInfo);
            mFileVersionJsonMtx.ReleaseMutex();
        }

        private void GetFilesVersionInfo(string dirPath, DirVersionInfo dirInfo)
        {
            try
            {
                if (!Directory.Exists(dirPath))
                {
                    return;
                }
                string[] files = Directory.GetFiles(dirPath);
                string[] dirs = Directory.GetDirectories(dirPath);
                foreach (var filePath in files)
                {
                    try
                    {
                        string fileName = CUtil.GetFileName(filePath);
                        FileVersionInfo fvi = new FileVersionInfo();
                        fvi.md5 = CUtil.GetMD5HashFromFile(filePath);
                        fvi.name = fileName;
                        if (dirInfo.files == null)
                        {
                            dirInfo.files = new List<FileVersionInfo>();
                        }
                        dirInfo.files.Add(fvi);
                    }
                    catch (Exception e)
                    {

                    }
                }
                foreach (var filePath in dirs)
                {
                    try
                    {
                        string fileName = CUtil.GetFileName(filePath);
                        if (dirInfo.dirs == null)
                        {
                            dirInfo.dirs = new List<DirVersionInfo>();
                        }
                        DirVersionInfo subDir = new DirVersionInfo();
                        subDir.name = fileName;
                        GetFilesVersionInfo(filePath, subDir);
                        dirInfo.dirs.Add(subDir);
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            catch (Exception exp)
            {

            }
        }

    }
}
