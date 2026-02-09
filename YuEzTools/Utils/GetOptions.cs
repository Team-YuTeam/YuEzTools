using AmongUs.GameOptions;
using System;
using InnerNet;
using YuEzTools.Helpers;
using YuEzTools.Modules;
using YuEzTools.UI;

namespace YuEzTools.Utils;

public static class RoleTypesExtensions
{
    /// <summary>
    /// 将RoleTypes枚举转换为配置映射的键名
    /// </summary>
    /// <param name="roleType">游戏原生角色枚举</param>
    /// <returns>枚举的字符串名称（如Tracker）</returns>
    public static string ToConfigKey(this RoleTypes roleType)
    {
        // 直接返回枚举的字符串名称，作为配置映射的键
        return roleType.ToString();
    }
}


public class GetOptions
{
    // 任务相关选项
    public static int GetShortTaskNum() =>
        Main.NormalOptions.TryGetInt(Int32OptionNames.NumShortTasks, out var a) ? a : default;
    public static int GetCommonTaskNum() =>
        Main.NormalOptions.TryGetInt(Int32OptionNames.NumCommonTasks, out var a) ? a : default;
    public static int GetLongTaskNum() =>
        Main.NormalOptions.TryGetInt(Int32OptionNames.NumLongTasks, out var a) ? a : default;
    
    // 基本游戏机制选项
    // 玩家速度和移动
    public static float GetPlayerSpeedMod() =>
        Main.NormalOptions.TryGetFloat(FloatOptionNames.PlayerSpeedMod, out var a) ? a : default;
    public static float GetCrewLightMod() =>
        Main.NormalOptions.TryGetFloat(FloatOptionNames.CrewLightMod, out var a) ? a : default;
    public static float GetImpostorLightMod() =>
        Main.NormalOptions.TryGetFloat(FloatOptionNames.ImpostorLightMod, out var a) ? a : default;
    
    // 击杀机制
    public static float GetKillCooldown() =>
        Main.NormalOptions.TryGetFloat(FloatOptionNames.KillCooldown, out var a) ? a : default;
    public static int GetKillDistance() =>
        Main.NormalOptions.TryGetInt(Int32OptionNames.KillDistance, out var a) ? a : default;
    
    // 会议设置
    public static int GetNumEmergencyMeetings() =>
        Main.NormalOptions.TryGetInt(Int32OptionNames.NumEmergencyMeetings, out var a) ? a : default;
    public static int GetEmergencyCooldown() =>
        Main.NormalOptions.TryGetInt(Int32OptionNames.EmergencyCooldown, out var a) ? a : default;
    public static int GetDiscussionTime() =>
        Main.NormalOptions.TryGetInt(Int32OptionNames.DiscussionTime, out var a) ? a : default;
    public static int GetVotingTime() =>
        Main.NormalOptions.TryGetInt(Int32OptionNames.VotingTime, out var a) ? a : default;
    
    // 游戏规则
    public static bool GetConfirmImpostor() =>
        Main.NormalOptions.TryGetBool(BoolOptionNames.ConfirmImpostor, out var a) ? a : default;
    public static bool GetVisualTasks() =>
        Main.NormalOptions.TryGetBool(BoolOptionNames.VisualTasks, out var a) ? a : default;
    public static bool GetAnonymousVotes() =>
        Main.NormalOptions.TryGetBool(BoolOptionNames.AnonymousVotes, out var a) ? a : default;
    public static int GetTaskBarMode() =>
        Main.NormalOptions.TryGetInt(Int32OptionNames.TaskBarMode, out var a) ? a : default;
    public static int GetMaxPlayers() =>
        Main.NormalOptions.TryGetInt(Int32OptionNames.MaxPlayers, out var a) ? a : default;
    public static int GetNumImpostors() =>
        Main.NormalOptions.TryGetInt(Int32OptionNames.NumImpostors, out var a) ? a : default;
    public static int GetTag() =>
        Main.NormalOptions.TryGetInt(Int32OptionNames.Tag, out var a) ? a : default;
    
    // 船员
    // 科学家
    public static float GetScientistCooldown() =>
        Main.NormalOptions.TryGetFloat(FloatOptionNames.ScientistCooldown, out var a) ? a : default;
    public static float GetScientistBatteryCharge() =>
        Main.NormalOptions.TryGetFloat(FloatOptionNames.ScientistBatteryCharge, out var a) ? a : default;
    
    // 工程师
    public static float GetEngineerCooldown() =>
        Main.NormalOptions.TryGetFloat(FloatOptionNames.EngineerCooldown, out var a) ? a : default;
    public static float GetEngineerInVentMaxTime() =>
        Main.NormalOptions.TryGetFloat(FloatOptionNames.EngineerInVentMaxTime, out var a) ? a : default;
    
    // 噪音制造者
    public static float GetNoisemakerAlertDuration() =>
        Main.NormalOptions.TryGetFloat(FloatOptionNames.NoisemakerAlertDuration, out var a) ? a : default;
    public static bool GetNoisemakerImpostorAlert() =>
        Main.NormalOptions.TryGetBool(BoolOptionNames.NoisemakerImpostorAlert, out var a) ? a : default;

    // 追踪者
    public static float GetTrackerCooldown() =>
        Main.NormalOptions.TryGetFloat(FloatOptionNames.TrackerCooldown, out var a) ? a : default;
    public static float GetTrackerDuration() =>
        Main.NormalOptions.TryGetFloat(FloatOptionNames.TrackerDuration, out var a) ? a : default;
    public static float GetTrackerDelay() =>
        Main.NormalOptions.TryGetFloat(FloatOptionNames.TrackerDelay, out var a) ? a : default;
    
    // 侦探
    public static float GetDetectiveSuspectLimit() =>
        Main.NormalOptions.TryGetFloat(FloatOptionNames.DetectiveSuspectLimit, out var a) ? a : default;
    
    // 内鬼
    // 毒蛇
    public static float GetViperDissolveTime() =>
        Main.NormalOptions.TryGetFloat(FloatOptionNames.ViperDissolveTime, out var a) ? a : default;
    
    // 变形者
    public static float GetShapeshifterCooldown() =>
	    Main.NormalOptions.TryGetFloat(FloatOptionNames.ShapeshifterCooldown, out var a) ? a : default;
    public static float GetShapeshifterDuration() =>
	    Main.NormalOptions.TryGetFloat(FloatOptionNames.ShapeshifterDuration, out var a) ? a : default;
    public static bool GetShapeshifterLeaveSkin() =>
	    Main.NormalOptions.TryGetBool(BoolOptionNames.ShapeshifterLeaveSkin, out var a) ? a : default;
    
    // 幻影
    public static float GetPhantomCooldown() =>
	    Main.NormalOptions.TryGetFloat(FloatOptionNames.PhantomCooldown, out var a) ? a : default;
    public static float GetPhantomDuration() =>
	    Main.NormalOptions.TryGetFloat(FloatOptionNames.PhantomDuration, out var a) ? a : default;

    // 灵魂
    // 守护天使
    public static float GetGuardianAngelCooldown() =>
	    Main.NormalOptions.TryGetFloat(FloatOptionNames.GuardianAngelCooldown, out var a) ? a : default;
    public static float GetProtectionDurationSeconds() =>
	    Main.NormalOptions.TryGetFloat(FloatOptionNames.ProtectionDurationSeconds, out var a) ? a : default;
    public static bool GetImpostorsCanSeeProtect() =>
	    Main.NormalOptions.TryGetBool(BoolOptionNames.ImpostorsCanSeeProtect, out var a) ? a : default;

    
    // 游戏设置选项
    public static byte GetMapId() =>
        Main.NormalOptions.TryGetByte(ByteOptionNames.MapId, out var a) ? a : default;
    public static int GetRulesPreset() =>
        Main.NormalOptions.TryGetInt(Int32OptionNames.RulePreset, out var a) ? a : default;
    
    // ToDo:躲猫猫模式选项
    
        // ========== 核心修改：适配游戏原生RoleTypes枚举 ==========
    // 配置项模型：展示名 + 获取值的方法 + 单位
    private class RoleConfigItem
    {
        public string ShowName; // UI展示的配置名（如追踪冷却）
        public Func<object> GetValue; // 获取配置值的方法
        public string Unit; // 单位（如s、无、次）
    }

    // 初始化所有角色的专属配置映射（键直接用枚举名，和游戏原生一致）
    private static readonly Dictionary<string, List<RoleConfigItem>> _roleConfigMap = new()
    {
        #region 追踪者 Tracker (枚举值10)
        [RoleTypes.Tracker.ToConfigKey()] = new List<RoleConfigItem>()
        {
            new() { ShowName = "TrackerCooldown", GetValue = () => GetTrackerCooldown(), Unit = "unit.second" },
            new() { ShowName = "TrackerDuration", GetValue = () => GetTrackerDuration(), Unit = "unit.second" },
            new() { ShowName = "TrackerDelay", GetValue = () => GetTrackerDelay(), Unit = "unit.second" }
        },
        #endregion

        #region 科学家 Scientist (枚举值2)
        [RoleTypes.Scientist.ToConfigKey()] = new List<RoleConfigItem>()
        {
            new() { ShowName = "ScientistCooldown", GetValue = () => GetScientistCooldown(), Unit = "unit.second" },
            new() { ShowName = "ScientistBatteryCharge", GetValue = () => GetScientistBatteryCharge(), Unit = "unit.percent" }
        },
        #endregion

        #region 工程师 Engineer (枚举值3)
        [RoleTypes.Engineer.ToConfigKey()] = new List<RoleConfigItem>()
        {
            new() { ShowName = "EngineerCooldown", GetValue = () => GetEngineerCooldown(), Unit = "unit.second" },
            new() { ShowName = "EngineerInVentCooldown", GetValue = () => GetEngineerInVentMaxTime(), Unit = "unit.second" }
        },
        #endregion

        #region 变形者 Shapeshifter (枚举值5)
        [RoleTypes.Shapeshifter.ToConfigKey()] = new List<RoleConfigItem>()
        {
            new() { ShowName = "ShapeshifterCooldown", GetValue = () => GetShapeshifterCooldown(), Unit = "unit.second" },
            new() { ShowName = "ShapeshifterDuration", GetValue = () => GetShapeshifterDuration(), Unit = "unit.second" },
            new() { ShowName = "ShapeshifterLeaveSkin", GetValue = () => GetString(GetShapeshifterLeaveSkin() ? "yes" : "no"), Unit = "" }
        },
        #endregion

        #region 守护天使 GuardianAngel (枚举值4)
        [RoleTypes.GuardianAngel.ToConfigKey()] = new List<RoleConfigItem>()
        {
            new() { ShowName = "GuardianAngelCooldown", GetValue = () => GetGuardianAngelCooldown(), Unit = "unit.second" },
            new() { ShowName = "GuardianAngelDuration", GetValue = () => GetProtectionDurationSeconds(), Unit = "unit.second" },
            new() { ShowName = "GuardianAngelImpostorSeeProtect", GetValue = () => GetString(GetImpostorsCanSeeProtect() ? "yes" : "no"), Unit = "" }
        },
        #endregion

        #region 噪音制造者 Noisemaker (枚举值8)
        [RoleTypes.Noisemaker.ToConfigKey()] = new List<RoleConfigItem>()
        {
            new() { ShowName = "NoisemakerAlertDuration", GetValue = () => GetNoisemakerAlertDuration(), Unit = "unit.second" },
            new() { ShowName = "NoisemakerImpostorAlert", GetValue = () => GetString(GetNoisemakerImpostorAlert() ? "yes" : "no"), Unit = "" }
        },
        #endregion

        #region 侦探 Detective (枚举值12)
        [RoleTypes.Detective.ToConfigKey()] = new List<RoleConfigItem>()
        {
            new() { ShowName = "DetectiveSuspectLimit", GetValue = () => GetDetectiveSuspectLimit(), Unit = "people" }
        },
        #endregion

        #region 毒蛇 Viper (枚举值18)
        [RoleTypes.Viper.ToConfigKey()] = new List<RoleConfigItem>()
        {
            new() { ShowName = "ViperDissolveTime", GetValue = () => GetViperDissolveTime(), Unit = "unit.second" }
        },
        #endregion

        #region 幻影 Phantom (枚举值9)
        [RoleTypes.Phantom.ToConfigKey()] = new List<RoleConfigItem>()
        {
            new() { ShowName = "PhantomCooldown", GetValue = () => GetPhantomCooldown(), Unit = "unit.second" },
            new() { ShowName = "PhantomDuration", GetValue = () => GetPhantomDuration(), Unit = "unit.second" }
        },
        #endregion

        #region 普通内鬼 Impostor (枚举值1)
        [RoleTypes.Impostor.ToConfigKey()] = new List<RoleConfigItem>()
        {
            new() { ShowName = "GameKillCooldown", GetValue = () => GetKillCooldown(), Unit = "unit.second" },
            new() { ShowName = "GameKillDistance", GetValue = () => GetString(GetKillDistance() switch { 0 => "distance.short", 1 => "distance.mid", 2 => "distance.long", _ => "default" }), Unit = "" },
            new() { ShowName = "GameImpostorLight", GetValue = () => GetImpostorLightMod(), Unit = "unir.multiplier" }
        },
        #endregion

        #region 普通船员 Crewmate (枚举值0)
        [RoleTypes.Crewmate.ToConfigKey()] = new List<RoleConfigItem>()
        {
            new() { ShowName = "GameCrewLight", GetValue = () => GetCrewLightMod(), Unit = "unit.multiplier" },
            new() { ShowName = "GameNumMeetings", GetValue = () => GetNumEmergencyMeetings(), Unit = "unit.time" },
            new() { ShowName = "GameEmergencyCooldown", GetValue = () => GetEmergencyCooldown(), Unit = "unit.second" }
        }
        #endregion
    };

    // ========== 对外暴露：适配枚举的快捷获取方法 ==========
    /// <summary>
    /// 按游戏原生RoleTypes枚举获取专属配置（推荐）
    /// </summary>
    /// <param name="roleType">游戏原生角色枚举</param>
    /// <returns>配置键值对（展示名：值+单位）</returns>
    public static Dictionary<string, string> GetRoleConfigForShow(RoleTypes roleType)
    {
        return GetRoleConfigForShow(roleType.ToConfigKey());
    }

    /// <summary>
    /// 按角色名获取专属配置（兼容旧逻辑）
    /// </summary>
    /// <param name="roleName">角色名（如Tracker）</param>
    /// <returns>配置键值对</returns>
    public static Dictionary<string, string> GetRoleConfigForShow(string roleName)
    {
        var result = new Dictionary<string, string>();
        if (!_roleConfigMap.TryGetValue(roleName, out var configItems)) return result;

        foreach (var item in configItems)
        {
            try
            {
                var value = item.GetValue();
                if (value is float f) value = Math.Round(f, 1);
                result[item.ShowName] = string.IsNullOrEmpty(item.Unit) ? $"{value}" : ($"{value}" + GetString(item.Unit));
            }
            catch { /* 单个配置出错不影响整体 */ }
        }
        return result;
    }

    /// <summary>
    /// 按玩家实例自动识别角色（游戏原生逻辑）
    /// </summary>
    /// <param name="player">玩家实例</param>
    /// <returns>配置键值对</returns>
    public static Dictionary<string, string> GetRoleConfigForShow(PlayerControl player)
    {
        if (player == null) return new Dictionary<string, string>();
        RoleTypes playerRole = player.GetRole();
        return GetRoleConfigForShow(playerRole);
    }
}