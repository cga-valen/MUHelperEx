namespace MUHelperEx
{
    /// <summary>
    /// 梦想奇迹 S14-2 版本基址偏移量
    /// Presents By MarsEla
    /// </summary>
    public class Offsets
    {

        /// <summary>
        /// <para>角色名偏移 S14-2已更新</para>
        /// <para>main.exe + 0xA0711D4</para>
        /// <para>偏移0x54</para>
        /// </summary>
        public const int nameOffset = 0xA0711D4;
        public const int nameOffset2 = 0x54;

        /// <summary>
        /// <para>等级偏移1 S14-2已更新</para>
        /// <para>main.exe + 0xA0711D4</para>
        /// </summary>
        public const int levelOffset1 = 0xA0711D4;

        /// <summary>
        /// <para>等级偏移2 S14-2已更新</para>
        /// <para>无偏移</para>
        /// </summary>
        public const int levelOffset2 = 0x0;

        /// <summary>
        /// <para>大师等级偏移 S14-2已更新</para>
        /// <para>0x18</para>
        /// </summary>
        public const int masterLvlOffset = 0x18;

        /// <summary>
        /// <para>当前经验偏移 S14-2没有验证 不确定是否正确 应该是08 经验也分普通经验和 大师经验</para>
        /// <para>0x20</para>
        /// </summary>
        public const int currentExpOffset = 0x20;

        /// <summary>
        /// <para>升级经验偏移 S14-2没有验证 不确定是否正确</para>
        /// <para>0x28</para>
        /// </summary>
        public const int totalExpOffset = 0x28;

        /// <summary>
        /// <para>x坐标偏移 S14-2已更新</para>
        /// <para>main.exe + 0x806D580</para>
        /// </summary>
        public const int xOffset = 0x806D580;

        /// <summary>
        /// <para>y坐标偏移 S14-2已更新</para>
        /// <para>main.exe + 0x806D57C</para>
        /// </summary>
        public const int yOffset = 0x806D57C;

        /// <summary>
        /// <para>当前血量偏移 S14-2已更新</para>
        /// <para>Client.dll + 0x31170</para>
        /// </summary>
        public const int currentHPOffset = 0x31170;

        /// <summary>
        /// <para>最大血量偏移 S14-2已更新</para>
        /// <para>Client.dll + 0x31174</para>
        /// </summary>
        public const int maxHPOffset = 0x31174;

        /// <summary>
        /// <para>当前MP偏移 S14-2已更新</para>
        /// <para>Client.dll + 0x31178</para>
        /// </summary>
        public const int currentMPOffset = 0x31178;

        /// <summary>
        /// <para>最大MP偏移 S14-2已更新</para>
        /// <para>Client.dll + 0x3117C</para>
        /// </summary>
        public const int maxMPOffset = 0x3117C;

        /// <summary>
        /// <para>当前SD偏移 S14-2已更新</para>
        /// <para>Client.dll + 0x31180</para>
        /// </summary>
        public const int currentSDOffset = 0x31180;

        /// <summary>
        /// <para>最大SD偏移 S14-2已更新</para>
        /// <para>Client.dll + 0x31184</para>
        /// </summary>
        public const int maxSDOffset = 0x31184;

        /// <summary>
        /// <para>当前AG偏移 S14-2已更新</para>
        /// <para>Client.dll + 0x31188</para>
        /// </summary>
        public const int currentAGOffset = 0x31188;

        /// <summary>
        /// <para>最大AG偏移 S14-2已更新</para>
        /// <para>Client.dll + 0x3118C</para>
        /// </summary>
        public const int maxAGOffset = 0x3118C;

        /// <summary>
        /// <para>安全区偏移1 S14-2已更新</para>
        /// <para>main.exe + 0x983956C</para>
        /// </summary>
        public const int safeAreaOffset1 = 0x983956C;

        /// <summary>
        /// <para>安全区偏移2 S14-2已更新</para>
        /// <para>0x04</para>
        /// <para>0不在安全区  1在安全区</para>
        /// </summary>
        public const int safeAreaOffset2 = 0x04;

        /// <summary>
        /// <para>金币偏移1 S14-2已更新</para>
        /// <para>main.exe + 0xA0711D0</para>
        /// </summary>
        public const int zenOffset1 = 0xA0711D0;

        /// <summary>
        /// <para>金币偏移2 S14-2已更新</para>
        /// <para>0x26BC</para>
        /// </summary>
        public const int zenOffset2 = 0x26BC;

        /// <summary>
        /// <para>瑞币偏移 在金币基址上偏移 S14-2已更新</para>
        /// <para>0x26C0</para>
        /// </summary>
        public const int rheaOffset1 = 0x26C0;

        /// <summary>
        /// <para>账号偏移</para>
        /// <para>Client.dll + 0x34178</para>
        /// </summary>
        public const int accOffset = 0x34178;

        /// <summary>
        /// <para>密码偏移</para>
        /// <para>Client.dll + 0x34184</para>
        /// </summary>
        public const int pwdOffset = 0x34184;

        /// <summary>
        /// <para>大区偏移 String[12] S14-2已更新</para>
        /// <para>main.exe + 0xA06C788</para>
        /// </summary>
        public const int realmOffset = 0xA06C788;

        /// <summary>
        /// <para>线路偏移 int S14-2已更新</para>
        /// <para>main.exe + 0xA06C888</para>
        /// </summary>
        public const int serverOffset = 0xA06C888;

        /// <summary>
        /// <para>目标ID偏移</para>
        /// <para>main.exe + 0x1317968</para>
        /// </summary>
        public const int targetOffset = 0x1317968;

        //TODO:main.exe+9E9A7F4  是否骑狼 15骑 14没骑
    }
}
