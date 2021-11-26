using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdater
{
    public class Models
    {

        [Serializable]
        public class DirVersionInfo
        {
            public string name;
            public List<DirVersionInfo> dirs = new List<DirVersionInfo>();
            public List<FileVersionInfo> files = new List<FileVersionInfo>();

            public void Clear()
            {
                dirs?.Clear();
                files?.Clear();
            }
        }

        [Serializable]
        public class FileVersionInfo
        {
            public string name;
            public string md5;      //md5为空，md5计算失败，有可能是文件打开失败
        }
    }
}
