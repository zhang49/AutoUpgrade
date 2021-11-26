using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdater
{
    [Serializable]
    public class UpgradeCfg
    {
        public string SyncDir = "D:/";
        public int CheckIntravel = 5 * 1000;
        public string Address = "";
        public int Port = 9000;
        public bool UseForServer = false;

        [NonSerialized]
        public string HttpServerHost = "http://localhost:9000";
        [NonSerialized]
        public readonly Object Lock = new object();
    }
}
