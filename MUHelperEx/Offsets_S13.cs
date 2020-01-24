namespace MUHelperEx
{
    /// <summary>
    /// 梦想奇迹 S13-2 版本基址偏移量
    /// </summary>
    public class Offsets_S13
    {

        /// <summary>
        /// <para>角色名偏移</para>
        /// <para>Client.dll + 0x34144</para>
        /// </summary>
        public const int nameOffset = 0x34144;

        /// <summary>
        /// <para>等级偏移1</para>
        /// <para>main.exe + 0x9e9a944</para>
        /// </summary>
        public const int levelOffset1 = 0x9e9a944;

        /// <summary>
        /// <para>等级偏移2</para>
        /// <para>无偏移</para>
        /// </summary>
        public const int levelOffset2 = 0x0;

        /// <summary>
        /// <para>大师等级偏移</para>
        /// <para>0x18</para>
        /// </summary>
        public const int masterLvlOffset = 0x18;

        /// <summary>
        /// <para>当前经验偏移</para>
        /// <para>0x20</para>
        /// </summary>
        public const int currentExpOffset = 0x20;

        /// <summary>
        /// <para>升级经验偏移</para>
        /// <para>0x28</para>
        /// </summary>
        public const int totalExpOffset = 0x28;

        /// <summary>
        /// <para>x坐标偏移</para>
        /// <para>main.exe + 0x86121c0</para>
        /// </summary>
        public const int xOffset = 0x86121c0;

        /// <summary>
        /// <para>y坐标偏移</para>
        /// <para>main.exe + 0x86121bc</para>
        /// </summary>
        public const int yOffset = 0x86121bc;

        /// <summary>
        /// <para>当前血量偏移</para>
        /// <para>Client.dll + 0x341b4</para>
        /// </summary>
        public const int currentHPOffset = 0x341b4;

        /// <summary>
        /// <para>最大血量偏移</para>
        /// <para>Client.dll + 0x341b8</para>
        /// </summary>
        public const int maxHPOffset = 0x341b8;

        /// <summary>
        /// <para>当前MP偏移</para>
        /// <para>Client.dll + 0x341bc</para>
        /// </summary>
        public const int currentMPOffset = 0x341bc;

        /// <summary>
        /// <para>最大MP偏移</para>
        /// <para>Client.dll + 0x341c0</para>
        /// </summary>
        public const int maxMPOffset = 0x341c0;

        /// <summary>
        /// <para>当前SD偏移</para>
        /// <para>Client.dll + 0x341c4</para>
        /// </summary>
        public const int currentSDOffset = 0x341c4;

        /// <summary>
        /// <para>最大SD偏移</para>
        /// <para>Client.dll + 0x341c8</para>
        /// </summary>
        public const int maxSDOffset = 0x341c8;

        /// <summary>
        /// <para>当前AG偏移</para>
        /// <para>Client.dll + 0x341cc</para>
        /// </summary>
        public const int currentAGOffset = 0x341cc;

        /// <summary>
        /// <para>最大AG偏移</para>
        /// <para>Client.dll + 0x341d0</para>
        /// </summary>
        public const int maxAGOffset = 0x341d0;

        /// <summary>
        /// <para>安全区偏移1</para>
        /// <para>main.exe + 0x7f804d4</para>
        /// </summary>
        public const int safeAreaOffset1 = 0x7f804d4;

        /// <summary>
        /// <para>安全区偏移2</para>
        /// <para>0x04</para>
        /// <para>0不在安全区  1在安全区</para>
        /// </summary>
        public const int safeAreaOffset2 = 0x04;

        /// <summary>
        /// <para>金币偏移1</para>
        /// <para>main.exe + 0x9e9a940</para>
        /// </summary>
        public const int zenOffset1 = 0x9e9a940;

        /// <summary>
        /// <para>金币偏移2</para>
        /// <para>0x22a4</para>
        /// </summary>
        public const int zenOffset2 = 0x22a4;

        /// <summary>
        /// <para>瑞币偏移 在金币基址上偏移</para>
        /// <para>0x22a8</para>
        /// </summary>
        public const int rheaOffset1 = 0x22a8;

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
        /// <para>大区偏移 String[12]</para>
        /// <para>main.exe + 0x9e95f18</para>
        /// </summary>
        public const int realmOffset = 0x9e95f18;

        /// <summary>
        /// <para>线路偏移 int</para>
        /// <para>main.exe + 0x9e96018</para>
        /// </summary>
        public const int serverOffset = 0x9e96018;

        /// <summary>
        /// <para>目标ID偏移</para>
        /// <para>main.exe + 0x1240ac0</para>
        /// </summary>
        public const int targetOffset = 0x1240ac0;

        //TODO:main.exe+9E9A7F4  是否骑狼 15骑 14没骑
    }
}
