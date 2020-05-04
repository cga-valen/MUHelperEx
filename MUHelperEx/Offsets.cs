namespace MUHelperEx
{
    /// <summary>
    /// 梦想奇迹 S14-2 版本基址偏移量
    /// Presents By MarsEla
    /// </summary>
    public class Offsets
    {

        /// <summary>
        /// <para>角色名偏移 S15已更新</para>
        /// <para>Client.dll + 0x32114</para>
        /// </summary>
        public const int nameOffset = 0x32114;

        /// <summary>
        /// <para>等级偏移1 S15已更新</para>
        /// <para>main.exe + 0xA1CCB3C</para>
        /// </summary>
        public const int levelOffset1 = 0xA1CCB3C;

        /// <summary>
        /// <para>等级偏移2 S15已更新</para>
        /// <para>无偏移</para>
        /// </summary>
        public const int levelOffset2 = 0x0;

        /// <summary>
        /// <para>大师等级偏移 S15已更新</para>
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
        /// <para>x坐标偏移 S15已更新</para>
        /// <para>main.exe + 0x80FE490</para>
        /// </summary>
        public const int xOffset = 0x80FE490;

        /// <summary>
        /// <para>y坐标偏移 S15已更新</para>
        /// <para>main.exe + 0x80FE48C</para>
        /// </summary>
        public const int yOffset = 0x80FE48C;

        /// <summary>
        /// <para>当前血量偏移 S15已更新</para>
        /// <para>Client.dll + 0x32190</para>
        /// </summary>
        public const int currentHPOffset = 0x32190;

        /// <summary>
        /// <para>最大血量偏移 S15已更新</para>
        /// <para>Client.dll + 0x32194</para>
        /// </summary>
        public const int maxHPOffset = 0x32194;

        /// <summary>
        /// <para>当前MP偏移 S15已更新</para>
        /// <para>Client.dll + 0x32198</para>
        /// </summary>
        public const int currentMPOffset = 0x32198;

        /// <summary>
        /// <para>最大MP偏移 S15已更新</para>
        /// <para>Client.dll + 0x3219C</para>
        /// </summary>
        public const int maxMPOffset = 0x3219C;

        /// <summary>
        /// <para>当前SD偏移 S15已更新</para>
        /// <para>Client.dll + 0x321A0</para>
        /// </summary>
        public const int currentSDOffset = 0x321A0;

        /// <summary>
        /// <para>最大SD偏移 S15已更新</para>
        /// <para>Client.dll + 0x321A4</para>
        /// </summary>
        public const int maxSDOffset = 0x321A4;

        /// <summary>
        /// <para>当前AG偏移 S15已更新</para>
        /// <para>Client.dll + 0x321A8</para>
        /// </summary>
        public const int currentAGOffset = 0x321A8;

        /// <summary>
        /// <para>最大AG偏移 S15已更新</para>
        /// <para>Client.dll + 0x321AC</para>
        /// </summary>
        public const int maxAGOffset = 0x321AC;

        /// <summary>
        /// <para>安全区偏移1 S15已更新</para>
        /// <para>main.exe + 0xA0F1620</para>
        /// </summary>
        public const int safeAreaOffset1 = 0xA0F1620;

        /// <summary>
        /// <para>安全区偏移2 S15已更新</para>
        /// <para>0x04</para>
        /// <para>0不在安全区  1在安全区</para>
        /// </summary>
        public const int safeAreaOffset2 = 0x04;

        /// <summary>
        /// <para>金币偏移1 S15已更新</para>
        /// <para>main.exe + 0xA0711D0</para>
        /// </summary>
        public const int zenOffset1 = 0xA1CCB38;

        /// <summary>
        /// <para>金币偏移2 S15已更新</para>
        /// <para>0x2734</para>
        /// </summary>
        public const int zenOffset2 = 0x2734;

        /// <summary>
        /// <para>瑞币偏移 在金币基址上偏移 S15已更新</para>
        /// <para>0x2738</para>
        /// </summary>
        public const int rheaOffset1 = 0x2738;

        /// <summary>
        /// <para>账号偏移 S15</para>
        /// <para>Client.dll + 0x32148</para>
        /// </summary>
        public const int accOffset = 0x32148;

        /// <summary>
        /// <para>密码偏移 S15</para>
        /// <para>Client.dll + 0x32154</para>
        /// </summary>
        public const int pwdOffset = 0x32154;

        /// <summary>
        /// <para>大区偏移 String[12] S15已更新</para>
        /// <para>main.exe + 0xA1C80F0</para>
        /// </summary>
        public const int realmOffset = 0xA1C80F0;

        /// <summary>
        /// <para>线路偏移 int S15</para>
        /// <para>main.exe + 0xA1C81F0</para>
        /// </summary>
        public const int serverOffset = 0xA1C81F0; // 和上个版本相差15B968

        /// <summary>
        /// <para>目标ID偏移 S15</para>
        /// <para>main.exe + 0x13A7968</para>
        /// </summary>
        public const int targetOffset = 0x13A7968;

        //TODO:main.exe+9E9A7F4  是否骑狼 15骑 14没骑
    }
}
