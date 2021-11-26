using Anna;
using HttpServerLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Security.Cryptography;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;

namespace AutoUpdater
{
    public partial class AutoUpdater : Form
    {

        private NotifyIcon notifyIcon = null;
        public static UpgradeCfg mUpgradeCfg = new UpgradeCfg();
        public bool mIsStop = false;
        private UpdateServer mServer = null;
        private UpdateClient mClient = null;

        public AutoUpdater()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.app;
            this.StartPosition = FormStartPosition.CenterScreen;
            if (LoadConfig())
            {
                if (mUpgradeCfg.UseForServer)
                {
                    mServer = new UpdateServer(mUpgradeCfg);
                    mServer.Init();
                    this.Text = "AutoUpgrader - server";
                }
                else
                {
                    mClient = new UpdateClient(mUpgradeCfg);
                    mClient.Init();
                    this.Text = "AutoUpgrader - client";
                }
                if (Directory.Exists(mUpgradeCfg.SyncDir))
                {
                    mUpgradeCfg.SyncDir = Directory.GetParent(mUpgradeCfg.SyncDir).FullName + "\\" + CUtil.GetDirName(mUpgradeCfg.SyncDir);
                }
                tbHttpServerHost.Text = mUpgradeCfg.HttpServerHost;
                tbCheckIntravel.Text = (mUpgradeCfg.CheckIntravel / 1000).ToString();
                tbAutoUpgradeFold.Text = mUpgradeCfg.SyncDir;
                InitialTray();
            }
            else
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(-100, -100);
                this.FormBorderStyle = FormBorderStyle.None;
                this.MinimumSize = new Size(1, 1);
                this.MaximumSize = new Size(1, 1);
                this.Size = new Size(1, 1);
                Task.Run(async () =>
                {
                    await Task.Delay(100);
                    this.Invoke(new ThreadStart(() =>
                    {
                        MessageBox.Show("配置初始失败!请检查文件\"config.json\"");
                        Process.GetCurrentProcess().Kill();
                    }));
                });
            }
        }

        private bool LoadConfig()
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream("config.json", FileMode.Open);
                TextReader textReader = new StreamReader(fs, Encoding.Default);
                var cfgDict = JsonConvert.DeserializeObject<Dictionary<String, Object>>(textReader.ReadToEnd());
                if (cfgDict.ContainsKey("use-for-server"))
                {
                    if (cfgDict["use-for-server"].GetType() == typeof(bool))
                    {
                        mUpgradeCfg.UseForServer = (bool)cfgDict["use-for-server"];
                    }
                    else if (cfgDict["use-for-server"].GetType() == typeof(Int64))
                    {
                        mUpgradeCfg.UseForServer = ((Int64)cfgDict["use-for-server"]) != 0;
                    }
                }
                if (cfgDict.ContainsKey("address"))
                {
                    mUpgradeCfg.Address = (string)cfgDict["address"];
                }
                if (cfgDict.ContainsKey("port") && cfgDict["port"].GetType() == typeof(Int64))
                {
                    mUpgradeCfg.Port = (int)((Int64)cfgDict["port"]);
                }
                if (cfgDict.ContainsKey("check-intravel-millisecond") && cfgDict["check-intravel-millisecond"].GetType() == typeof(Int64))
                {
                    mUpgradeCfg.CheckIntravel = (int)((Int64)cfgDict["check-intravel-millisecond"]);
                }
                if (mUpgradeCfg.UseForServer)
                {
                    if (cfgDict.ContainsKey("server-sync-dir"))
                    {
                        mUpgradeCfg.SyncDir = (string)cfgDict["server-sync-dir"];
                    }
                }
                else
                {
                    if (cfgDict.ContainsKey("client-sync-dir"))
                    {
                        mUpgradeCfg.SyncDir = (string)cfgDict["client-sync-dir"];
                    }
                }
                mUpgradeCfg.HttpServerHost = String.Format("http://{0}:{1}", mUpgradeCfg.Address, mUpgradeCfg.Port);

            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                fs?.Close();
            }
            return true;
        }

        /// <summary>
        /// 托盘初始化函数
        /// </summary>
        private void InitialTray()
        {
            //实例化一个NotifyIcon对象  
            notifyIcon = new NotifyIcon();
            //托盘图标显示的内容
            notifyIcon.Text = this.Text;
            //注意：下面的路径可以是绝对路径、相对路径。但是需要注意的是：文件必须是一个.ico格式  

            notifyIcon.Icon = Properties.Resources.app;
            //true表示在托盘区可见，false表示在托盘区不可见  
            notifyIcon.Visible = true;

            //托盘图标气泡显示的内容  
            //notifyIcon.BalloonTipText = "正在后台运行";
            //气泡显示的时间（单位是毫秒）  
            //notifyIcon.ShowBalloonTip(2000);

            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseClick);
            ////设置二级菜单  
            //MenuItem setting1 = new MenuItem("二级菜单1");
            //MenuItem setting2 = new MenuItem("二级菜单2");  
            //MenuItem setting = new MenuItem("一级菜单", new MenuItem[]{setting1,setting2});
            MenuItem exit = new MenuItem("Exit");
            exit.Click += new EventHandler(exit_Click);
            ////关联托盘控件
            MenuItem[] childen = new MenuItem[] { exit };
            notifyIcon.ContextMenu = new ContextMenu(childen);
        }

        private void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //鼠标左键单击  
            if (e.Button == MouseButtons.Left)
            {
                //如果窗体是可见的，那么鼠标左击托盘区图标后，窗体为不可见  
                if (this.Visible == true)
                {
                    this.Visible = false;
                }
                else
                {
                    this.Visible = true;
                    this.Activate();
                }
            }
        }

        private void exit_Click(object sender, EventArgs e)
        {
            //退出程序  
            mIsStop = true;
            mServer?.Stop();
            mClient?.Stop();
            System.Environment.Exit(0);
        }

        private void AutoUpdater_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void tbAutoUpgradeFold_MouseClick(object sender, MouseEventArgs e)
        {
            return;
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (DialogResult.OK == dlg.ShowDialog())
            {
                tbAutoUpgradeFold.Text = dlg.SelectedPath;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            lock (mUpgradeCfg.Lock)
            {
                if (mUpgradeCfg.UseForServer)
                {
                    try
                    {
                        int seconds = int.Parse(tbCheckIntravel.Text);
                        mUpgradeCfg.CheckIntravel = seconds * 1000;
                        mUpgradeCfg.SyncDir = tbAutoUpgradeFold.Text;
                        tbCheckIntravel.Text = (mUpgradeCfg.CheckIntravel / 1000).ToString();
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show("参数错误");
                    }
                }
                else
                {
                    try
                    {
                        mUpgradeCfg.HttpServerHost = tbHttpServerHost.Text;
                        if (mClient?.TryConnectToServer() ?? false)
                        {
                            MessageBox.Show("连接失败");
                        }
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show("参数错误");
                    }
                }
            }
        }

        private void AutoUpdater_Load(object sender, EventArgs e)
        {

        }
    }
}
