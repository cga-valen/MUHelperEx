using SharpPcap;

namespace MUHelperEx {
    public static class Settings {
        public static int readTimeOut = 1000;// 抓包超时时间
        public static DeviceMode devMode = DeviceMode.Promiscuous;//数据采集模式 混杂模式
        public static int UpdateThreadSleepTime = 500;
        public static bool bGameTitleModify = true;
        public static bool bCharacterDeadPopup = true;
        public static int PopupDelayTime = 10;//秒
        // boss刷新提示
        public static bool bBossRespawnAlert = true;
        // boos刷新时间记录
        public static bool bBoosRespawnRecord = true;
        // boss死亡记录
        public static bool bBossDieRecord = true;

        //精准IP
        public static bool bPrecisionIP = true;
        //是否可用
        public static bool bUseable = true;
    }
}
