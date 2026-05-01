namespace YuEzTools.Modules;

public class Toggles
{
    public static bool DarkMode { get => Main.DarkModeConfig.Value; set => Main.DarkModeConfig.Value = value; }
    public static bool ShowPlayTimes { get => Main.ShowPlayTimes.Value; set => Main.ShowPlayTimes.Value = value; }

    public static bool ShowCommit { get => Main.ShowCommitConfig.Value; set => Main.ShowCommitConfig.Value = value; }
    public static bool ShowModText { get => Main.ShowModTextConfig.Value; set => Main.ShowModTextConfig.Value = value; }
    public static bool ShowIsSafe { get => Main.ShowIsSafeConfig.Value; set => Main.ShowIsSafeConfig.Value = value; }
    public static bool ShowIsDark { get => Main.ShowIsDarkConfig.Value; set => Main.ShowIsDarkConfig.Value = value; }
    public static bool ShowPing { get => Main.ShowPingConfig.Value; set => Main.ShowPingConfig.Value = value; }
    public static bool ShowFPS { get => Main.ShowFPSConfig.Value; set => Main.ShowFPSConfig.Value = value; }
    public static bool ShowServer { get => Main.ShowServerConfig.Value; set => Main.ShowServerConfig.Value = value; }
    public static bool ShowIsAutoExit { get => Main.ShowIsAutoExitConfig.Value; set => Main.ShowIsAutoExitConfig.Value = value; }
    public static bool ShowRoomTime { get => Main.ShowRoomTimeConfig.Value; set => Main.ShowRoomTimeConfig.Value = value; }
    public static bool ShowUTC { get => Main.ShowUTCConfig.Value; set => Main.ShowUTCConfig.Value = value; }
    public static bool ShowLocalNowTime { get => Main.ShowLocalNowTimeConfig.Value; set => Main.ShowLocalNowTimeConfig.Value = value; }
    public static bool ShowGM { get => Main.ShowGMConfig.Value; set => Main.ShowGMConfig.Value = value; }

    public static bool WinTextSize { get => Main.WinTextSize.Value; set => Main.WinTextSize.Value = value; }

    public static bool EnableAntiCheat { get => Main.EnableAntiCheatConfig.Value; set => Main.EnableAntiCheatConfig.Value = value; }
    public static bool SafeMode { get => Main.SafeModeConfig.Value; set => Main.SafeModeConfig.Value = value; }
    public static bool AutoExit { get => Main.AutoExitConfig.Value; set => Main.AutoExitConfig.Value = value; }
    public static bool KickNotLogin { get => Main.KickNotLoginConfig.Value; set => Main.KickNotLoginConfig.Value = value; }
    public static bool AutoReportHacker { get => Main.AutoReportHackerConfig.Value; set => Main.AutoReportHackerConfig.Value = value; }

    public static bool DumpLog { get => Main.DumpLogConfig.Value; set => Main.DumpLogConfig.Value = value; }
    public static bool OpenGameDic { get => Main.OpenGameDicConfig.Value; set => Main.OpenGameDicConfig.Value = value; }
    public static bool CloseMusicOfOr { get => Main.CloseMusicOfOrConfig.Value; set => Main.CloseMusicOfOrConfig.Value = value; }
    public static bool reShowRoleT { get => Main.reShowRoleTConfig.Value; set => Main.reShowRoleTConfig.Value = value; }
    public static bool ShowInfoInLobby { get => Main.ShowInfoInLobbyConfig.Value; set => Main.ShowInfoInLobbyConfig.Value = value; }

    public static bool ExitGame { get => Main.ExitGameConfig.Value; set => Main.ExitGameConfig.Value = value; }
    public static bool RealBan { get => Main.RealBanConfig.Value; set => Main.RealBanConfig.Value = value; }
    public static bool HorseMode { get => Main.HorseModeConfig.Value; set => Main.HorseModeConfig.Value = value; }
    public static bool LongMode { get => Main.LongModeConfig.Value; set => Main.LongModeConfig.Value = value; }

    public static bool ChangeDownTimerToZero { get => Main.ChangeDownTimerToZeroConfig.Value; set => Main.ChangeDownTimerToZeroConfig.Value = value; }
    public static bool AbolishDownTimer { get => Main.AbolishDownTimerConfig.Value; set => Main.AbolishDownTimerConfig.Value = value; }
    public static bool AutoStartGame { get => Main.AutoStartGameConfig.Value; set => Main.AutoStartGameConfig.Value = value; }
    public static bool ServerAllHostOrNoHost { get => Main.ServerAllHostOrNoHostConfig.Value; set => Main.ServerAllHostOrNoHostConfig.Value = value; }
    public static bool ChangeDownTimerTo114514 { get => Main.ChangeDownTimerTo114514Config.Value; set => Main.ChangeDownTimerTo114514Config.Value = value; }

    public static bool FPSPlus { get => Main.FPSPlusConfig.Value; set => Main.FPSPlusConfig.Value = value; }
    
#if DEBUG
    public static bool NotEndGame { get => Main.NotEndGameConfig.Value; set => Main.NotEndGameConfig.Value = value; }
#endif
}
