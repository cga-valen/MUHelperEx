using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using PacketDotNet;
using SharpPcap;

namespace MUHelperEx {
    public class Game {
        #region field
        private Process process;
        private int port; // 本地端口

        private string acc;
        private string pwd;
        private string realm;
        private int server;
        private string name;
        private int level;
        private int masterLevel;
        private int x;
        private int y;
        private int currentHP;
        private int maxHP;
        private int currentSD;
        private int maxSD;
        private int currentMP;
        private int maxMP;
        private int currentAG;
        private int maxAG;
        private int safeArea;
        private int lastSafeArea;
        private bool isSync = false;//用于同步第一次两个安全区数值
        private int zen;
        private int rhea;
        private int target;

        private string gridRealm;
        private int gridHPpercent;
        private int gridMPpercent;
        private int gridSDpercent;
        private int gridAGpercent;
        private string gridXY;
        private int gridLevel;
        private string gridStatus;

        // 缓存列表
        private List<RawCapture> bufferList;
        // 挨个网卡抓
        public List<ICaptureDevice> naList = new List<ICaptureDevice>();

        // 分析数据的线程
        Thread analyzerThread;
        // 监测掉线的线程
        Thread monitorThread;
        // 线程锁
        object threadLock = new object();

        private bool isStartAnalyzer = false;
        private bool isStartMonitor = false;
        // 收到的报文计数
        private long packetRecv = 0;

        public delegate void ServerMessageHandler(Game game, string text);
        public event ServerMessageHandler MessageDone;

        public delegate void PlayerOfflineHandler(Game game);
        public event PlayerOfflineHandler PlayerOffline;
        #endregion

        #region 
        public Process Process
        {
            get
            {
                return process;
            }

            set
            {
                process = value;
            }
        }

        public int Port
        {
            get
            {
                return port;
            }

            set
            {
                if (port != value) {
                    // 赋值
                    port = value;
                    // 更新filter
                    this.updatePort(value);
                }
            }
        }

        public string Acc
        {
            get
            {
                return acc;
            }

            set
            {
                acc = value;
            }
        }

        public string Pwd
        {
            get
            {
                return pwd;
            }

            set
            {
                pwd = value;
            }
        }

        public string Realm
        {
            get
            {
                return realm;
            }

            set
            {
                realm = value;
            }
        }

        public int Server
        {
            get
            {
                return server;
            }

            set
            {
                server = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public int Level
        {
            get
            {
                return level;
            }

            set
            {
                level = value;
            }
        }

        public int MasterLevel
        {
            get
            {
                return masterLevel;
            }

            set
            {
                masterLevel = value;
            }
        }

        public int X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

        public int CurrentHP
        {
            get
            {
                return currentHP;
            }

            set
            {
                currentHP = value;
            }
        }

        public int MaxHP
        {
            get
            {
                return maxHP;
            }

            set
            {
                maxHP = value;
            }
        }

        public int CurrentSD
        {
            get
            {
                return currentSD;
            }

            set
            {
                currentSD = value;
            }
        }

        public int MaxSD
        {
            get
            {
                return maxSD;
            }

            set
            {
                maxSD = value;
            }
        }

        public int CurrentMP
        {
            get
            {
                return currentMP;
            }

            set
            {
                currentMP = value;
            }
        }

        public int MaxMP
        {
            get
            {
                return maxMP;
            }

            set
            {
                maxMP = value;
            }
        }

        public int CurrentAG
        {
            get
            {
                return currentAG;
            }

            set
            {
                currentAG = value;
            }
        }

        public int MaxAG
        {
            get
            {
                return maxAG;
            }

            set
            {
                maxAG = value;
            }
        }

        public int SafeArea
        {
            get
            {
                return safeArea;
            }

            set
            {
                safeArea = value;
            }
        }

        public int Zen
        {
            get
            {
                return zen;
            }

            set
            {
                zen = value;
            }
        }

        public int Rhea
        {
            get
            {
                return rhea;
            }

            set
            {
                rhea = value;
            }
        }

        public int Target
        {
            get
            {
                return target;
            }

            set
            {
                target = value;
            }
        }

        public string GridRealm
        {
            get
            {
                return gridRealm;
            }

            set
            {
                gridRealm = value;
            }
        }

        public int GridHPpercent
        {
            get
            {
                return gridHPpercent;
            }

            set
            {
                gridHPpercent = value;
            }
        }

        public string GridXY
        {
            get
            {
                return gridXY;
            }

            set
            {
                gridXY = value;
            }
        }

        public int GridLevel
        {
            get
            {
                return gridLevel;
            }

            set
            {
                gridLevel = value;
            }
        }

        public int GridMPpercent
        {
            get
            {
                return gridMPpercent;
            }

            set
            {
                gridMPpercent = value;
            }
        }

        public int GridSDpercent
        {
            get
            {
                return gridSDpercent;
            }

            set
            {
                gridSDpercent = value;
            }
        }

        public int GridAGpercent
        {
            get
            {
                return gridAGpercent;
            }

            set
            {
                gridAGpercent = value;
            }
        }

        public string GridStatus
        {
            get
            {
                return gridStatus;
            }

            set
            {
                gridStatus = value;
            }
        }

        public int LastSafeArea
        {
            get
            {
                return lastSafeArea;
            }

            set
            {
                lastSafeArea = value;
            }
        }

        public bool IsSync
        {
            get
            {
                return isSync;
            }

            set
            {
                isSync = value;
            }
        }

        public bool IsStartAnalyzer
        {
            get
            {
                return isStartAnalyzer;
            }

            set
            {
                isStartAnalyzer = value;
            }
        }

        #endregion

        /// <summary>
        /// 从所有网卡上开始抓包
        /// </summary>
        public void startCapture() {
            Debug.WriteLine("[+]一个游戏( " + port + " )准备开始抓包...");
            this.bufferList = new List<RawCapture>();
            this.clear();// 清理原有的数据
            this.isStartAnalyzer = true;
            this.startAnalyzer();// 启动分析线程
            naList = new List<ICaptureDevice>();

            //foreach (ICaptureDevice device in CaptureDeviceManager.getInstance()) {
            var devices = CaptureDeviceManager.getInstance();
            for (int i = 0; i < devices.Count; i++) {
                if (devices[i].Description.Contains("VMware") || devices[i].Description.Contains("TAP-Windows Adapter")) {
                    Debug.WriteLine("[-]端口( " + port + " )在" + devices[i].Description + " 已忽略...");
                    continue; //跳过虚拟网卡和SS-TAP
                }
                // 这里必须使用CaptureDeviceList.New()[i]获取新实例, 虽然很慢, 但是没有解决办法
                // 如果使用devices[i] 那表示使用的还是同一块网卡, 绑定多个过滤器, 并不是多网卡实例 会导致多个Game收到同样的内容
                ICaptureDevice device = CaptureDeviceList.New()[i];
                //ICaptureDevice device = devices[i];

                try {
                    device.OnPacketArrival += new PacketArrivalEventHandler(this.device_OnPacketArrival);
                    device.Open(Settings.devMode, Settings.readTimeOut);
                    device.Filter = "port " + this.port;
                    device.StartCapture();
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
                naList.Add(device);
                Debug.WriteLine("[+]端口( " + port + " )在" + device.Description + " 初始化完毕...");
            }
            this.isStartMonitor = true;
            this.startMonitor();// 启动掉线监测
            Debug.WriteLine("[+]一个游戏( " + port + " )抓包已启动完毕...");
        }

        public void stopCapture() {
            try {
                foreach (ICaptureDevice device in naList) {
                    if (device != null /*&& device.Started*/) {
                        device.StopCapture();
                        device.Close();
                    }
                    this.isStartAnalyzer = false;
                    if (this.analyzerThread != null) {
                        this.analyzerThread.Abort();
                    }
                    this.isStartMonitor = false;
                    if (this.monitorThread != null) {
                        this.monitorThread.Abort();
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void device_OnPacketArrival(object sender, CaptureEventArgs e) {
            lock (threadLock) {
                this.bufferList.Add(e.Packet);
                packetRecv++;// 计数 长时间不变化表示掉线
                //Debug.WriteLine("[+]" + e.Device.Description + " 收到消息...");
            }
        }

        private void clear() {
            if (this.bufferList != null) {
                this.bufferList.Clear();
            }
        }

        private void startAnalyzer() {
            analyzerThread = new Thread(new ThreadStart(analyserThreadMethod));
            analyzerThread.IsBackground = true;
            analyzerThread.Start();
        }

        private void startMonitor() {
            monitorThread = new Thread(new ThreadStart(monitorThreadMethod));
            monitorThread.IsBackground = true;
            monitorThread.Start();
        }

        //数据分析线程
        private void analyserThreadMethod() {
            while (isStartAnalyzer) {
                bool isShouldSleep = true;
                lock (threadLock) {
                    if (this.bufferList.Count != 0)
                        isShouldSleep = false;
                }
                if (isShouldSleep) {
                    Thread.Sleep(200);
                } else {
                    List<RawCapture> tmpPacketList;
                    // 获取数据
                    lock (threadLock) {
                        tmpPacketList = this.bufferList;
                        // 构建新列表
                        this.bufferList = new List<RawCapture>();
                    }
                    foreach (RawCapture p in tmpPacketList) {
                        this.handlePacket(p);
                    }
                }
            }
        }

        private void monitorThreadMethod() {
            while (isStartMonitor) {
                // 每分钟检测一次
                Thread.Sleep(60 * 1000);
                Debug.WriteLine("[*]这一分钟, " + this.Name + " 收到了" + packetRecv + "个报文...");
                if (packetRecv == 0) {
                    // 掉线
                    isStartMonitor = false;// 掉线了还监控啥啊
                    // 产生事件
                    PlayerOffline?.Invoke(this);
                } else {
                    packetRecv = 0;
                }
            }
        }

        /// <summary>
        /// 核心: 解析报文
        /// </summary>
        /// <param name="rawPacket"></param>
        private void handlePacket(RawCapture rawPacket) {
            try {
                // 构建通用数据包
                Packet generalPacket = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
                // 只要以太网包
                if (rawPacket.LinkLayerType == LinkLayers.Ethernet) {
                    // 封装以太网包
                    EthernetPacket e = EthernetPacket.GetEncapsulated(generalPacket);
                    // 只要IPV4
                    if (e.Type == EthernetPacketType.IpV4) {
                        IPv4Packet ipv4 = (IPv4Packet)IpPacket.GetEncapsulated(generalPacket);
                        // 只要TCP包
                        if (ipv4.NextHeader == IPProtocolType.TCP) {
                            // 将通用数据包封装为TCP包
                            TcpPacket tcp = TcpPacket.GetEncapsulated(generalPacket);

                            byte[] tmp = tcp.PayloadData;// 数据
                            if (tmp != null && tmp.Length > 4) {
                                // 处理粘包
                                int i = 0;
                                do {
                                    int len = -1;
                                    if (i + 4 > tmp.Length) {
                                        //len = 1;
                                        break;
                                    } else if (tmp[i] == 0xC1) {
                                        len = tmp[i + 1];
                                        // C1 0D包长度必须大于13
                                        if (tmp[i + 2] == 0x0D && len >= 13) {
                                            // 0D包第3字节后有10字节0x00 直接跳过
                                            // 这10字节应该是发送者名字, 服务器消息则没有
                                            byte[] msg = new byte[len - 13];
                                            Buffer.BlockCopy(tmp, i + 13, msg, 0, msg.Length);
                                            string text = Encoding.Default.GetString(msg).Split('\0')[0];
                                            // 处理获取到的服务器消息
                                            MessageDone?.Invoke(this, text);
                                        }
                                    } else if (tmp[i] == 0xC2 || tmp[i] == 0xC4) {
                                        // TODO: 这里有个半包异常 懒得处理了
                                        // C2和C4 第2-3字节规定了长度
                                        len = tmp[i + 1] << 8 | tmp[i + 2];
                                    } else if (tmp[i] == 0xC3) {
                                        // C1和C3 第2字节规定了长度
                                        len = tmp[i + 1];
                                    } else {
                                        len = 1;
                                    }
                                    // 防止半包或网络不稳收到C100 造成死循环
                                    if (len == 0)
                                        len = 1;
                                    i += len;// 跳过len字节
                                } while (i < tmp.Length);
                            }
                        }
                    }
                }
            } catch (Exception e) {
            }
        }

        /// <summary>
        /// 更新过滤器
        /// </summary>
        /// <param name="port"></param>
        public void updatePort(int port) {
            foreach (ICaptureDevice device in naList) {
                if (device != null && port != -1) {
                    device.Filter = "port " + port;
                    Debug.WriteLine("[+]" + device.Description + "的过滤器已更新: ( " + device.Filter + " )...");
                }
            }
        }

    }
}
