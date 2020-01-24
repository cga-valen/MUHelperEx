using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraBars.Alerter;
using Newtonsoft.Json;
using PacketDotNet;
using SharpPcap;

namespace MUHelperEx {
    public partial class frmMain : DevExpress.XtraBars.Ribbon.RibbonForm {
        #region Native Method
        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        public static extern int SetWindowText(IntPtr hwnd, string lpString);
        #endregion

        //private string host = "http://192.168.1.13:8080";
        private string host = "http://218.60.22.31:8080";
        private string auth = "/vcs/v3/user/auth/";
        private string heartbeat = "/vcs/v3/user/";
        private string death = "/vcs/v3/death";
        private string offline = "/vcs/v3/offline";
        // 游戏主进程名 去掉后缀
        const string gameName = "main";
        // 游戏人物列表
        List<Game> gameList = new List<Game>();
        // 更新数据线程
        Thread UpdateThread;
        // 更新游戏多开线程
        Thread UpdateGameThread;
        // VCS
        Thread VCSThread;

        delegate void ServerMessageDelegate(DevExpress.XtraEditors.ListBoxControl listbox, string msg);
        delegate void GridViewUpdateDelegate();
        // 最后一条服务器消息
        Dictionary<string, string> lastServerMsg = new Dictionary<string, string>();

        // uid
        string uid = MethodUtils.getCPUID();
        // token
        string token = "";

        // 询问关闭
        bool allowClose = false;
        int disconnect = 0;

        /// <summary>
        /// 时间, 服务器, 线路, 信息
        /// </summary>
        const string LOG_FORMAT = "{0} > [{1}{2}线] {3}";

        public frmMain() {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e) {

            Debug.WriteLine("[*]CDM初始化中...");
            CaptureDeviceManager.getInstance();
            Debug.WriteLine("[+]CDM初始化完毕...");

            // 初始化游戏进程
            Process[] processes = Process.GetProcessesByName(gameName);
            foreach (Process p in processes) {
                gameList.Add(MethodUtils.GetCharacter(p));
            }
            Debug.WriteLine("[+]已启动的游戏搜寻完毕. found: " + gameList.Count);

            // 启动更新线程 读取内存
            UpdateThread = new Thread(UpdateInfo);
            UpdateThread.IsBackground = true;
            UpdateThread.Start();
            Debug.WriteLine("[+]读取内存进程启动完毕...");

            // 启动寻找游戏线程 每5秒寻找一次游戏
            UpdateGameThread = new Thread(GetGameWindow);
            UpdateGameThread.IsBackground = true;
            UpdateGameThread.Start();
            Debug.WriteLine("[+]游戏寻找进程启动完毕...");

            this.gridControl1.DataSource = gameList; // 设置数据源
            this.columnSDPercent.Visible = false; // 隐藏SD条

            // 游戏抓包启动
            /*
            int count = 0;
            foreach (Game g in gameList) {
                if (g.Port != -1 && !g.IsStartAnalyzer) {
                    g.startCapture();
                    g.MessageDone += handleMessage;
                    count++;
                }
            }
            Debug.WriteLine("[+]" + count + "个游戏的抓包已启动(" + (gameList.Count - count) + "个未启动)...");
            // 这里不用手动启动了 搜寻线程中会自动启动所有不是 -1端口 且 没启动抓包的进程
            */

            // listBoxControl1.Items.Add("若此处消息无法正常显示, 请在\"网络\"选项中调整网卡设置...");
            listBoxControl1.Items.Add("所有的新用户可以免费试用7天...");
            listBoxControl1.Items.Add("挂机助手与硬件绑定, 请在需要使用的电脑上进行充值操作...");
            listBoxControl1.Items.Add("新版挂机助手核心代码全部重写. 稳定性更好, 兼容性更强. 但首次截获数据耗时变长...");
            listBoxControl1.Items.Add("如果需要死亡/掉线通知, 请务必记得填写邮箱地址, 且要将发件人设置为收件白名单, 否则只能收到一封邮件...");
            listBoxControl1.Items.Add("发件人地址: 3511238627@qq.com");
            listBoxControl2.Items.Add("此处记录Boss死亡/刷新时间, 请在上方勾选记录Boss信息...");
            ActiveUser ac = null;
            try {
                string json = MethodUtils.HttpGet(host + auth + uid);
                ac = JsonConvert.DeserializeObject<ActiveUser>(json);
                barStaticItemExpire.Caption = "可用时间: " + ac.countdown;
                this.token = ac.token;
            } catch (Exception) {
                this.kill();
                MessageBox.Show("网络验证失败, 此版本已经停止服务... 请更新至最新版", "错误");
                Application.Exit();
            }
            Debug.WriteLine("[+]VCS账号校验完毕...");
            if (ac.status) {
                // 启动VCS线程
                VCSThread = new Thread(vcs);
                VCSThread.IsBackground = true;
                VCSThread.Start();
                allowClose = true;
                Debug.WriteLine("[+]VCS账号可用, VCS线程启动...");
            } else {
                // 开启时已经过期
                this.kill();
                MessageBox.Show("您的账号已到期, 所有功能暂停使用, 请充值后继续...", "提示");
                Debug.WriteLine("[-]VCS账号已过期, 所有线程已摧毁...");
            }

        }

        /// <summary>
        /// 每5秒寻找一次游戏 看是否有新进程开启
        /// TODO: 修改成apihook的形式 hook进程创建
        /// </summary>
        private void GetGameWindow() {
            while (true) {
                // 获取所有gameName进程
                Process[] processes = Process.GetProcessesByName(gameName);
                // 遍历进程
                foreach (Process p in processes) {
                    // 用于指示是否是已经启动的游戏
                    bool oldGame = false;// 默认都是新启动的
                    foreach (Game c in gameList) {
                        // 列表中已经存在了
                        if (c.Process.Id == p.Id) {
                            // 旧游戏
                            oldGame = true;
                        }
                    }

                    // 发现新游戏
                    if (!oldGame) {
                        // 加入gameList
                        gameList.Add(MethodUtils.GetCharacter(p));
                        Debug.WriteLine("[+]UpdateGameThread找到一个新游戏: pid->" + p.Id);
                        // TODO: 是否在这里启动抓包?
                    }
                }

                // 重新遍历一次维护的游戏进程
                for (int i = 0; i < gameList.Count; i++) {// 不能用foreach 初始化慢 如果集合改变了会出现异常
                    Game c = gameList[i];
                    // 进程还存活
                    if (c != null && !c.Process.HasExited) {
                        // 根据pid重新获取端口
                        c.Port = MethodUtils.GetPortByPID(c.Process.Id);
                        // 如果端口不是-1, 且游戏还没启动分析
                        if (c.Port != -1 && !c.IsStartAnalyzer) {
                            this.Invoke(new ServerMessageDelegate(listboxAddMsg), this.listBoxControl1, string.Format("{0} > [新游戏@{1}] {2}", DateTime.Now.ToString(), c.Port, "消息获取线程正在初始化...大约需要10-20秒..."));
                            // 开始抓包
                            c.startCapture();// 此方法目前及其慢
                            // 处理事件
                            c.MessageDone += handleMessage;
                            c.PlayerOffline += C_PlayerOffline;
                            this.Invoke(new ServerMessageDelegate(listboxAddMsg), this.listBoxControl1, string.Format("{0} > [新游戏@{1}] {2}", DateTime.Now.ToString(), c.Port, "消息获取线程初始化完毕, 正在截获消息..."));
                            Debug.WriteLine("[+]UpdateGameThread启动了[ " + c.Port + " ]端口的抓包...");
                        }
                    }
                }
                Debug.WriteLine("[*]周期性: 寻找游戏窗口完毕, 休眠5秒...");
                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// 处理抓到的消息
        /// </summary>
        /// <param name="game"></param>
        /// <param name="msg"></param>
        private void handleMessage(Game game, string text) {
            // S14-2版本中不再显示此信息 那也放着吧
            if (text.Contains("你的角色大师等级已经超过系统限制")) {
                return;
            }
            string realm = game.Realm;// 服务器
            int server = game.Server;// 线路
            string mapKey = realm + server; // 存入map的key

            // 第一次添加key
            if (!lastServerMsg.ContainsKey(mapKey)) {
                lastServerMsg.Add(mapKey, text);
            }
            // 先从map中获取value 看看和上一次是否重复
            if (lastServerMsg[mapKey] == text) {
                return;
            }
            // 不是第一次 更新value
            lastServerMsg[mapKey] = text;//按照服务器+线路 存入新消息

            string formatMsg = string.Format(LOG_FORMAT, DateTime.Now.ToString(), realm, server, text);

            this.Invoke(new ServerMessageDelegate(listboxAddMsg), this.listBoxControl1, formatMsg);
            if (MethodUtils.isBossSpawnMsg(text) && Settings.bBossRespawnAlert) {
                AlertControl alertControl1 = new AlertControl();
                alertControl1.AutoFormDelay = Settings.PopupDelayTime * 1000 * 5;
                alertControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
                alertControl1.FormLocation = DevExpress.XtraBars.Alerter.AlertFormLocation.BottomRight;
                // alertControl1.Show(this, DateTime.Now.ToLongTimeString() + " > " + server + "线Boss刷新!", text);
                BeginInvoke(new MethodInvoker(delegate () {
                    AlertInfo info = new AlertInfo(DateTime.Now.ToLongTimeString() + " > " + server + "线Boss刷新!", text);
                    alertControl1.Show(this, info);
                }));
            }
            if (MethodUtils.isBossSpawnMsg(text) && Settings.bBoosRespawnRecord) {
                this.Invoke(new ServerMessageDelegate(listboxAddMsg), this.listBoxControl2, string.Format(LOG_FORMAT, DateTime.Now.ToString(), realm, server, text));
            }

            if (MethodUtils.isBossDieMsg(text) && Settings.bBossDieRecord) {
                this.Invoke(new ServerMessageDelegate(listboxAddMsg), this.listBoxControl2, string.Format(LOG_FORMAT, DateTime.Now.ToString(), realm, server, text));
            }
        }

        /// <summary>
        /// 读内存
        /// </summary>
        private void UpdateInfo() {
            while (true) {
                for (int i = 0; i < gameList.Count; i++) {
                    try {
                        Game character = gameList[i];
                        // 游戏退出 从List中移除
                        if (character.Process.HasExited) {
                            character.stopCapture();
                            gameList.Remove(character);
                            BeginInvoke(new MethodInvoker(delegate () {
                                // 更新数据源
                                this.gridControl1.DataSource = null;
                                this.gridControl1.DataSource = gameList;
                            }));
                            continue;
                        }

                        #region Read Memory
                        VMemory vm = new VMemory(character.Process);
                        // 服务器
                        character.Realm = vm.ReadString(vm.ExeBase, Offsets.realmOffset, 12);
                        // 线路
                        character.Server = vm.ReadInt32(vm.ExeBase, Offsets.serverOffset);
                        // 角色名
                        character.Name = vm.ReadString(vm.ExeBase, Offsets.nameOffset, Offsets.nameOffset2, 10);
                        // 基本等级0-400
                        character.Level = vm.ReadInt32(vm.ExeBase, Offsets.levelOffset1, Offsets.levelOffset2);
                        // 400之后大师等级
                        character.MasterLevel = vm.ReadInt32(vm.ExeBase, Offsets.levelOffset1, Offsets.masterLvlOffset);
                        // 坐标
                        character.X = vm.ReadInt32(vm.ExeBase, Offsets.xOffset);
                        character.Y = vm.ReadInt32(vm.ExeBase, Offsets.yOffset);
                        // 血量
                        character.CurrentHP = vm.ReadInt32(vm.DllBase, Offsets.currentHPOffset);
                        character.MaxHP = vm.ReadInt32(vm.DllBase, Offsets.maxHPOffset);
                        // 蓝量
                        character.CurrentMP = vm.ReadInt32(vm.DllBase, Offsets.currentMPOffset);
                        character.MaxMP = vm.ReadInt32(vm.DllBase, Offsets.maxMPOffset);
                        // 防护值
                        character.CurrentSD = vm.ReadInt32(vm.DllBase, Offsets.currentSDOffset);
                        character.MaxSD = vm.ReadInt32(vm.DllBase, Offsets.maxSDOffset);
                        // AG
                        character.CurrentAG = vm.ReadInt32(vm.DllBase, Offsets.currentAGOffset);
                        character.MaxAG = vm.ReadInt32(vm.DllBase, Offsets.maxAGOffset);
                        // 是否在安全区内 0不在  1在
                        character.LastSafeArea = character.SafeArea;
                        character.SafeArea = vm.ReadByte(vm.ExeBase, Offsets.safeAreaOffset1, Offsets.safeAreaOffset2);
                        if (!character.IsSync) {
                            //第一次同步一下两个结果
                            character.IsSync = true;
                            character.LastSafeArea = character.SafeArea;
                        }
                        // 金币
                        character.Zen = vm.ReadInt32(vm.ExeBase, Offsets.zenOffset1, Offsets.zenOffset2);
                        // 瑞币
                        // character.Rhea = vm.ReadInt32(vm.ExeBase, Offsets.zenOffset1, Offsets.rheaOffset1);
                        // 账号
                        // character.Acc = vm.ReadString(vm.DllBase, Offsets.accOffset, 10);
                        // 密码
                        // character.Pwd = vm.ReadString(vm.DllBase, Offsets.pwdOffset, 20);
                        // 当前目标
                        character.Target = vm.ReadInt32(vm.ExeBase, Offsets.targetOffset);
                        #endregion

                        // 表格数据
                        character.GridRealm = string.Format("{0} - {1}线", character.Realm, character.Server);
                        character.GridHPpercent = Convert.ToInt32(Convert.ToDouble(character.CurrentHP) / character.MaxHP * 100);
                        character.GridMPpercent = Convert.ToInt32(Convert.ToDouble(character.CurrentMP) / character.MaxMP * 100);
                        character.GridSDpercent = Convert.ToInt32(Convert.ToDouble(character.CurrentSD) / character.MaxSD * 100);
                        character.GridAGpercent = Convert.ToInt32(Convert.ToDouble(character.CurrentAG) / character.MaxAG * 100);
                        character.GridXY = string.Format("{0},{1}", character.X, character.Y);
                        character.GridLevel = character.Level + character.MasterLevel;
                        // 0不在安全区  1在安全区
                        if (character.SafeArea == 0) {
                            character.GridStatus = "正常, 安全区外";
                            if (character.Target != -1) {
                                character.GridStatus += " 攻击->" + character.Target;
                            }
                        } else if (character.SafeArea == 1) {
                            character.GridStatus = "安全区内站岗";
                        } else {
                            character.GridStatus = "异常";
                        }
                        if (character.Zen < 300000000) {
                            character.GridStatus += ", 金币不足3亿";
                        }
                        // 检测到返回安全区
                        if (character.SafeArea != character.LastSafeArea && character.LastSafeArea == 0) {
                            // 弹窗
                            if (Settings.bCharacterDeadPopup) {
                                AlertControl alertControl1 = new AlertControl();
                                alertControl1.AutoFormDelay = Settings.PopupDelayTime * 1000;
                                alertControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
                                alertControl1.FormLocation = DevExpress.XtraBars.Alerter.AlertFormLocation.BottomRight;
                                BeginInvoke(new MethodInvoker(delegate () {
                                    AlertInfo info = new AlertInfo(DateTime.Now.ToShortTimeString() + " 死亡警报", character.Name + "@等级" + character.GridLevel + " 已死亡...");
                                    alertControl1.Show(this, info);
                                }));
                            }

                            string mailto = barEditItemEmail.EditValue.ToString().Trim();
                            if (mailto != "") {
                                Dictionary<string, string> param = new Dictionary<string, string>();
                                param.Add("mailto", mailto);
                                param.Add("username", character.Name);
                                Debug.WriteLine("[*]死亡通知邮件发送中...-> " + mailto);
                                MethodUtils.HttpPost(host + death, param);
                                Debug.WriteLine("[*]死亡通知邮件发送完毕...-> " + mailto);
                            }
                        }

                        // 关闭句柄
                        vm.Close();
                        // 窗体标题提示血量
                        if (Settings.bGameTitleModify) {
                            SetWindowText(character.Process.MainWindowHandle, string.Format("{0}: {1:N1}%", character.Name, Convert.ToDouble(character.CurrentHP) / character.MaxHP * 100));
                        }

                        // TODO: 获取人物经验值
                    } catch (Exception) {
                    }
                }
                // 刷新数据源
                //gridControl1.RefreshDataSource();
                this.Invoke(new GridViewUpdateDelegate(GridViewUpdate));
                if (Settings.UpdateThreadSleepTime < 500) {
                    Settings.UpdateThreadSleepTime = 500;
                }
                Thread.Sleep(Settings.UpdateThreadSleepTime);
            }
        }

        #region Minor method
        /// <summary>
        /// 刷新表格数据源
        /// </summary>
        private void GridViewUpdate() {
            gridControl1.RefreshDataSource();
        }

        /// <summary>
        /// 修改下方listbox中的内容
        /// </summary>
        /// <param name="listbox"></param>
        /// <param name="msg"></param>
        private void listboxAddMsg(DevExpress.XtraEditors.ListBoxControl listbox, string msg) {
            listbox.Items.Add(msg);
            listbox.SelectedIndex = listbox.Items.Count - 1;
        }

        /// <summary>
        /// kill没什么好说的
        /// </summary>
        private void kill() {
            // 摧毁内存数据更新线程
            if (UpdateThread != null && UpdateThread.IsAlive) {
                UpdateThread.Abort();
            }
            // 摧毁线程
            if (UpdateGameThread != null && UpdateGameThread.IsAlive) {
                UpdateGameThread.Abort();
            }
            // 摧毁VCS
            if (VCSThread != null && VCSThread.IsAlive) {
                VCSThread.Abort();
            }
            // 摧毁抓包线程
            this.Stop();

            // 将游戏窗体标题改回去
            foreach (Game c in gameList) {
                SetWindowText(c.Process.MainWindowHandle, c.Realm);
            }
        }

        /// <summary>
        /// 停止抓包
        /// </summary>
        private void Stop() {
            foreach (Game g in gameList) {
                g.stopCapture();
            }
        }

        private void C_PlayerOffline(Game game) {
            string mailto = barEditItemEmail.EditValue.ToString().Trim();
            if (mailto != "") {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("mailto", mailto);
                param.Add("username", game.Name);
                Debug.WriteLine("[!]掉线通知邮件发送中...-> " + mailto);
                MethodUtils.HttpPost(host + offline, param);
                Debug.WriteLine("[!]掉线通知邮件发送完毕...-> " + mailto);
            }
        }
        #endregion

        #region Event
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e) {
            if (allowClose && gameList.Count != 0) {
                DialogResult result = MessageBox.Show("确认退出吗?\r\n退出后挂机助手将不再继续记录信息", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.Cancel) {
                    e.Cancel = true;
                    return;
                }
            }
            // 摧毁内存数据更新线程
            if (UpdateThread != null && UpdateThread.IsAlive) {
                UpdateThread.Abort();
            }
            // 摧毁线程
            if (UpdateGameThread != null && UpdateGameThread.IsAlive) {
                UpdateGameThread.Abort();
            }
            if (VCSThread != null && VCSThread.IsAlive) {
                VCSThread.Abort();
            }
            // 摧毁抓包线程
            this.Stop();
            // 将游戏窗体标题改回去
            foreach (Game c in gameList) {
                SetWindowText(c.Process.MainWindowHandle, c.Realm);
            }
        }

        private void barCheckItemGameTitle_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            Settings.bGameTitleModify = this.barCheckItemGameTitle.Checked;
            if (!Settings.bGameTitleModify) {
                foreach (Game c in gameList) {
                    SetWindowText(c.Process.MainWindowHandle, c.Realm);
                }
            }
        }

        private void barCheckItemMP_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            this.columnMPPercent.Visible = this.barCheckItemMP.Checked;
        }

        private void barCheckItemAG_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            this.columnAGPercent.Visible = this.barCheckItemAG.Checked;
        }

        private void barCheckItemSD_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            this.columnSDPercent.Visible = this.barCheckItemSD.Checked;
        }

        private void barCheckItemDeadPopWindow_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            Settings.bCharacterDeadPopup = this.barCheckItemDeadPopWindow.Checked;
        }

        private void barButtonItemAbout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            frmAbout about = new frmAbout();
            about.ShowDialog();
        }

        private void barCheckItemBossAlert_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            Settings.bBossRespawnAlert = this.barCheckItemBossAlert.Checked;
        }

        private void barCheckItemBossDie_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            Settings.bBossDieRecord = this.barCheckItemBossDie.Checked;
        }

        private void barCheckItemBossSpawn_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            Settings.bBoosRespawnRecord = this.barCheckItemBossSpawn.Checked;
        }

        private void barEditItemPopupDelay_EditValueChanged(object sender, EventArgs e) {
            Settings.PopupDelayTime = (int)this.barEditItemPopupDelay.EditValue;
        }

        private void vcs() {
            while (true) {
                try {
                    string json = MethodUtils.HttpGet(host + heartbeat + this.uid + "/" + this.token);
                    ActiveUser ac = JsonConvert.DeserializeObject<ActiveUser>(json);
                    if (!ac.status) {
                        this.Invoke(new ServerMessageDelegate(listboxAddMsg), this.listBoxControl1, "=========================");
                        this.Invoke(new ServerMessageDelegate(listboxAddMsg), this.listBoxControl1, "可用时间消耗完毕, 请充值后使用...");
                        this.Invoke(new ServerMessageDelegate(listboxAddMsg), this.listBoxControl1, "=========================");
                        gridControl1.DataSource = null;
                        this.kill();
                    }
                    barStaticItemExpire.Caption = "可用时间: " + ac.countdown;
                    //Debug.WriteLine("HB发送完毕 : ac.status=" + ac.status + "  休眠60秒...");
                    disconnect = 0;// 成功一次置零
                } catch (Exception) {
                    if (disconnect > 5) {
                        MessageBox.Show("权限校验失败, 如果持续出现此错误, 则表示此版本已停止服务...", "即将退出");
                        Application.Exit();
                    } else {
                        disconnect++;
                    }
                }
                Thread.Sleep(60 * 1000);
            }
        }
        #endregion
    }
}
