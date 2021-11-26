# AutoUpgrades
### 自动把服务器的文件同步到客户端
config.json说明
{
	"说明":"use-for-server: true 程序作为服务器，false 程序作为客户端；address、port：服务器的ip以及端口；check-intravel-millisecond：文件检查间隔(ms)；server-sync-dir、client-sync-dir：需要同步的文件夹（把服务器的文件同步到客户端）",
	"use-for-server":true,
	"address":"127.0.0.1",
	"port":9000,
	"check-intravel-millisecond":8000,
	"server-sync-dir":"d:\\FunInVR\\XdPlayer",
	"client-sync-dir":"d:\\test\\upgrade-test-dir"
}
