using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading;
using static AutoUpdater.Models;

namespace AutoUpdater
{
    public class UpdateClient
    {
        private static UpgradeCfg mUpgradeCfg = new UpgradeCfg();
        private bool mIsStop = false;
        private Thread mCheckFileVersionTh;

        public UpdateClient(UpgradeCfg cfg)
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


        public bool TryConnectToServer()
        {
            if (String.IsNullOrEmpty(CUtil.HttpGetJson(mUpgradeCfg.HttpServerHost, "")))
            {
                return false;
            }
            return true;
        }

        public void Init()
        {
            mCheckFileVersionTh = new Thread(new ThreadStart(() =>
            {
                long lastTick = DateTime.Now.ToFileTimeUtc() / 10000;
                ClientCheckFileVersionProcess();
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
                        ClientCheckFileVersionProcess();
                        lastTick = DateTime.Now.ToFileTimeUtc() / 10000;
                    }
                    Thread.Sleep(50);
                }
            }));
            mCheckFileVersionTh.Start();
        }


        private void DealDownload(string parentDir, DirVersionInfo dirs)
        {
            foreach (var dInfo in dirs.dirs)
            {
                string dirPath = parentDir + dInfo.name + "/";
                DealDownload(dirPath, dInfo);
            }
            string baseDir = "";
            string httpServerHost = "";

            lock (mUpgradeCfg.Lock)
            {
                baseDir = mUpgradeCfg.SyncDir;
                httpServerHost = mUpgradeCfg.HttpServerHost;
            }
            foreach (var fInfo in dirs.files)
            {
                string url = httpServerHost + "/file" + parentDir + fInfo.name;
                string filePath = baseDir + parentDir + fInfo.name;
                do
                {
                    if (File.Exists(filePath))
                    {
                        string md5 = CUtil.GetMD5HashFromFile(filePath);
                        if (String.IsNullOrEmpty(md5) || String.IsNullOrEmpty(fInfo.md5) || md5 == fInfo.md5)
                        {
                            break;
                        }
                    }
                    CUtil.HttpGetFile(url, filePath, true);
                } while (false);
            }
        }

        private void ClientCheckFileVersionProcess()
        {
            string baseUrl = "";
            lock (mUpgradeCfg.Lock)
            {
                baseUrl = mUpgradeCfg.HttpServerHost;
            }
            string str = CUtil.HttpGetJson(baseUrl + "/get-files-version", "");
            if (!String.IsNullOrEmpty(str))
            {
                DirVersionInfo dirs = JsonConvert.DeserializeObject<DirVersionInfo>(str);
                DealDownload("/", dirs);
            }
        }

    }
}
