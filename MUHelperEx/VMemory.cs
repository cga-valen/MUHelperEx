using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MUHelperEx {
    /// <summary>
    /// 内存操作
    /// </summary>
    public class VMemory
    {
        private const string exeName = "main.exe";
        private const string dllName = "Client.dll";
        private Process mainProcess;// 读取的进程
        private IntPtr exeBase;// main.exe模块地址
        private IntPtr dllBase;// Client.dll模块地址
        private IntPtr processHandle;// 打开的进程句柄

        public IntPtr ExeBase
        {
            get
            {
                return exeBase;
            }
        }

        public IntPtr DllBase
        {
            get
            {
                return dllBase;
            }
        }

        public VMemory(Process pProcess)
        {
            this.mainProcess = pProcess;
            // 获取进程内所有模块
            ProcessModuleCollection modules = pProcess.Modules;
            // main.exe模块
            ProcessModule exeModule = null;
            // Client.dll模块
            ProcessModule dllModule = null;
            // 枚举模块 找出用到的两个
            foreach (ProcessModule module in modules)
            {
                if (module.ModuleName == exeName)
                {
                    exeModule = module;
                }
                else if (module.ModuleName == dllName)
                {
                    dllModule = module;
                }
            }
            // 保存模块地址
            this.dllBase = dllModule.BaseAddress;
            this.exeBase = exeModule.BaseAddress;

            // 打开进程
            this.processHandle = OpenProcess(0x1f0fff, false, this.mainProcess.Id);
            if (this.processHandle == IntPtr.Zero)
            {
                MessageBox.Show("权限不足! 请使用管理员身份运行...");
                Application.Exit();
            }
        }

        public bool Close()
        {
            return CloseHandle(this.processHandle);
        }

        #region Read Methods
        public bool ReadBoolean(IntPtr pOffset)
        {
            try
            {
                return BitConverter.ToBoolean(this.ReadByteArray(pOffset, 1), 0);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public byte ReadByte(IntPtr pOffset)
        {
            return this.ReadByteArray(pOffset, 1)[0];
        }

        public byte ReadByte(IntPtr pModule, int pOffset)
        {
            return this.ReadByteArray((IntPtr)(pModule.ToInt32() + pOffset), 1)[0];
        }

        public byte ReadByte(IntPtr pModule, int pOffset1, int pOffset2)
        {
            IntPtr pointer = (IntPtr)this.ReadUInt32(pModule, pOffset1);
            return this.ReadByte(pointer, pOffset2);
        }

        public byte[] ReadByteArray(IntPtr pOffset, uint pSize)
        {
            try
            {
                uint num;
                VirtualProtectEx(this.processHandle, pOffset, (UIntPtr)pSize, 4, out num);
                byte[] lpBuffer = new byte[pSize];
                ReadProcessMemory(this.processHandle, pOffset, lpBuffer, pSize, 0);
                VirtualProtectEx(this.processHandle, pOffset, (UIntPtr)pSize, num, out num);
                return lpBuffer;
            }
            catch (Exception)
            {
                return new byte[1];
            }
        }

        public char ReadChar(IntPtr pOffset)
        {
            try
            {
                return BitConverter.ToChar(this.ReadByteArray(pOffset, 1), 0);
            }
            catch (Exception)
            {
                return ' ';
            }
        }

        public double ReadDouble(IntPtr pOffset)
        {
            try
            {
                return BitConverter.ToDouble(this.ReadByteArray(pOffset, 8), 0);
            }
            catch (Exception)
            {
                return 0.0;
            }
        }

        public float ReadFloat(IntPtr pOffset)
        {
            try
            {
                return BitConverter.ToSingle(this.ReadByteArray(pOffset, 4), 0);
            }
            catch (Exception)
            {
                return 0f;
            }
        }

        public short ReadInt16(IntPtr pOffset)
        {
            try
            {
                return BitConverter.ToInt16(this.ReadByteArray(pOffset, 2), 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int ReadInt32(IntPtr pOffset)
        {
            try
            {
                return BitConverter.ToInt32(this.ReadByteArray(pOffset, 4), 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int ReadInt32(IntPtr pModule, int pOffset)
        {
            return this.ReadInt32((IntPtr)(pModule.ToInt32() + pOffset));
        }

        public int ReadInt32(IntPtr pModule, int pOffset1, int pOffset2)
        {
            IntPtr pointer = (IntPtr)this.ReadUInt32(pModule, pOffset1);
            return this.ReadInt32(pointer, pOffset2);
        }

        public long ReadInt64(IntPtr pOffset)
        {
            try
            {
                return BitConverter.ToInt64(this.ReadByteArray(pOffset, 8), 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int ReadInteger(IntPtr pOffset)
        {
            try
            {
                return BitConverter.ToInt32(this.ReadByteArray(pOffset, 4), 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public long ReadLong(IntPtr pOffset)
        {
            try
            {
                return BitConverter.ToInt64(this.ReadByteArray(pOffset, 8), 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public short ReadShort(IntPtr pOffset)
        {
            try
            {
                return BitConverter.ToInt16(this.ReadByteArray(pOffset, 2), 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public string ReadString(IntPtr pOffset, uint pSize)
        {
            try
            {
                return Encoding.Default.GetString(this.ReadByteArray(pOffset, pSize), 0, (int)pSize).Split('\0')[0];
            }
            catch (Exception)
            {
                return "";
            }
        }
        public string ReadString(IntPtr pModule, int pOffset, uint pSize)
        {
            return this.ReadString((IntPtr)(pModule.ToInt32() + pOffset), pSize);
        }
        public string ReadString(IntPtr pModule, int pOffset1, int pOffset2, uint pSize)
        {
            IntPtr pointer = (IntPtr)this.ReadUInt32(pModule, pOffset1);
            return this.ReadString(pointer, pOffset2, pSize);
        }

        public string ReadStringASCII(IntPtr pOffset, uint pSize)
        {
            try
            {
                return Encoding.ASCII.GetString(this.ReadByteArray(pOffset, pSize), 0, (int)pSize);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string ReadStringUnicode(IntPtr pOffset, uint pSize)
        {
            try
            {
                return Encoding.Unicode.GetString(this.ReadByteArray(pOffset, pSize), 0, (int)pSize);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public ushort ReadUInt16(IntPtr pOffset)
        {
            try
            {
                return BitConverter.ToUInt16(this.ReadByteArray(pOffset, 2), 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public uint ReadUInt32(IntPtr pOffset)
        {
            try
            {
                return BitConverter.ToUInt32(this.ReadByteArray(pOffset, 4), 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public uint ReadUInt32(IntPtr pModule, int pOffset)
        {
            return ReadUInt32((IntPtr)(pModule.ToInt32() + pOffset));
        }

        public ulong ReadUInt64(IntPtr pOffset)
        {
            try
            {
                return BitConverter.ToUInt64(this.ReadByteArray(pOffset, 8), 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public uint ReadUInteger(IntPtr pOffset)
        {
            try
            {
                return BitConverter.ToUInt32(this.ReadByteArray(pOffset, 4), 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public long ReadULong(IntPtr pOffset)
        {
            try
            {
                return (long)BitConverter.ToUInt64(this.ReadByteArray(pOffset, 8), 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public ushort ReadUShort(IntPtr pOffset)
        {
            try
            {
                return BitConverter.ToUInt16(this.ReadByteArray(pOffset, 2), 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion

        #region Write Methods
        public bool WriteByteArray(IntPtr pOffset, byte[] pBytes)
        {
            try
            {
                uint num;
                VirtualProtectEx(this.processHandle, pOffset, (UIntPtr)pBytes.Length, 4, out num);
                bool flag = WriteProcessMemory(this.processHandle, pOffset, pBytes, (uint)pBytes.Length, 0);
                VirtualProtectEx(this.processHandle, pOffset, (UIntPtr)pBytes.Length, num, out num);
                return flag;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool WriteBoolean(IntPtr pOffset, bool pData)
        {
            return this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }

        public bool WriteByte(IntPtr pOffset, byte pData)
        {
            return this.WriteByteArray(pOffset, BitConverter.GetBytes((short)pData));
        }

        public bool WriteChar(IntPtr pOffset, char pData)
        {
            return this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }

        public bool WriteDouble(IntPtr pOffset, double pData)
        {
            return this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }

        public bool WriteFloat(IntPtr pOffset, float pData)
        {
            return this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }

        public bool WriteInt16(IntPtr pOffset, short pData)
        {
            return this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }

        public bool WriteInt32(IntPtr pOffset, int pData)
        {
            return this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }

        public bool WriteInt64(IntPtr pOffset, long pData)
        {
            return this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }

        public bool WriteInteger(IntPtr pOffset, int pData)
        {
            return this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }

        public bool WriteLong(IntPtr pOffset, long pData)
        {
            return this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }

        public bool WriteShort(IntPtr pOffset, short pData)
        {
            return this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }

        public bool WriteStringASCII(IntPtr pOffset, string pData)
        {
            return this.WriteByteArray(pOffset, Encoding.ASCII.GetBytes(pData));
        }

        public bool WriteStringUnicode(IntPtr pOffset, string pData)
        {
            return this.WriteByteArray(pOffset, Encoding.Unicode.GetBytes(pData));
        }

        public bool WriteUInt16(IntPtr pOffset, ushort pData)
        {
            return this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }

        public bool WriteUInt32(IntPtr pOffset, uint pData)
        {
            return this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }

        public bool WriteUInt64(IntPtr pOffset, ulong pData)
        {
            return this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }

        public bool WriteUInteger(IntPtr pOffset, uint pData)
        {
            return this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }

        public bool WriteULong(IntPtr pOffset, ulong pData)
        {
            return this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }

        public bool WriteUShort(IntPtr pOffset, ushort pData)
        {
            return this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }
        #endregion

        #region Enum
        [Flags]
        private enum ProcessAccessFlags : uint
        {
            All = 0x1f0fff,
            CreateThread = 2,
            DupHandle = 0x40,
            QueryInformation = 0x400,
            SetInformation = 0x200,
            Synchronize = 0x100000,
            Terminate = 1,
            VMOperation = 8,
            VMRead = 0x10,
            VMWrite = 0x20
        }

        private enum VirtualMemoryProtection : uint
        {
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_GUARD = 0x100,
            PAGE_NOACCESS = 1,
            PAGE_NOCACHE = 0x200,
            PAGE_READONLY = 2,
            PAGE_READWRITE = 4,
            PAGE_WRITECOPY = 8,
            PROCESS_ALL_ACCESS = 0x1f0fff
        }
        #endregion

        #region Native Methods
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint dwSize, uint lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, uint lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        #endregion
    }
}
