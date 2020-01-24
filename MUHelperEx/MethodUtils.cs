using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;

namespace MUHelperEx {
    public class MethodUtils {
        private static string[] BossSpawnText = new string[] { "美杜莎事件开始", "通往孵化魔地的入口已经开启了", "冰霜巨蛛已经登场了", "尼克斯BOSS已经重生", "水晶球已经在菲利亚君主房间刷新" };
        private static string[] BossDieText = new string[] { "Boss天魔菲尼斯被", "Boss马格里姆被", "Boss辛维斯特被", "Boss昆顿被", "消灭了冰霜巨蛛", "Boss菲利亚君主被", "击杀了黑暗之神", "message 500" };
        private static string[] BossNextText = new string[] { "360分钟后孵化魔地将开启" };

        public static bool isBossSpawnMsg(string msg) {
            foreach (string str in BossSpawnText) {
                if (msg.Contains(str)) {
                    return true;
                }
            }
            return false;
        }

        public static bool isBossDieMsg(string msg) {
            foreach (string str in BossDieText) {
                if (msg.Contains(str)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 初始化人物 通过进程id寻找端口号 并将进程对象存入Game对象
        /// </summary>
        public static Game GetCharacter(Process p) {
            // 只要有进程即可 后续通过更新线程获取数据
            Game character = new Game();
            character.Process = p;
            // 获取游戏进程监听的端口
            character.Port = GetPortByPID(p.Id);
            return character;
        }
        /// <summary>
        /// 使用ManagedIpHelper通过进程号获取端口 
        /// 无监听端口时返回-1
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static int GetPortByPID(int pid) {
            foreach (TcpRow tcpRow in ManagedIpHelper.GetExtendedTcpTable(true)) {
                if (tcpRow.ProcessId == pid) {
                    // 游戏只监听了一个TCP端口
                    return tcpRow.LocalEndPoint.Port;
                }
            }
            return -1;
        }

        public static string getCPUID() {
            /*
            string cpuInfo = "";
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc) {
                cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
            }
            return cpuInfo;
            */
            return FingerPrint.Value();
        }

        public static string HttpGet(string Url) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            request.ContentType = "application/json;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            retString = AES.DecodeAES(retString);
            return retString;
        }

        public static string HttpPost(string url, Dictionary<string, string> dic) {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            #region 添加Post 参数
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic) {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream()) {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
                result = reader.ReadToEnd();
            }
            return result;
        }
    }
}
