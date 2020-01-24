using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using SharpPcap;

namespace MUHelperEx {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Process instance = RunningInstance();
            if (instance == null) {
                // 获取网卡信息
                try {
                    string ver = SharpPcap.Version.VersionString;
                    Debug.WriteLine("[+]网卡信息获取成功, 版本: " + ver);
                } catch (Exception ex) {
                    MessageBox.Show("请先安装压缩包中的WinPcap\r\nWin10用户请使用兼容模式安装WinPcap\r\n\r\n" + ex.Message, "网卡获取失败");
                    Application.Exit();
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                BonusSkins.Register();
                Application.Run(new frmMain());
            } else {
                HandleRunningInstance(instance);
            }
        }

        public static Process RunningInstance() {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            foreach (Process process in processes) {
                if (process.Id != current.Id) {
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName) {
                        return process;
                    }
                }
            }
            return null;
        }

        public static void HandleRunningInstance(Process instance) {
            ShowWindowAsync(instance.MainWindowHandle, 1);
            SetForegroundWindow(instance.MainWindowHandle);
        }
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
