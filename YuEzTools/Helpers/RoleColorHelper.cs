using AmongUs.GameOptions;
using UnityEngine;

namespace YuEzTools.Helpers;

public static class RoleColorHelper
{
    // 定义角色颜色映射关系，统一管理所有角色的颜色
    private static readonly Dictionary<RoleTypes, RoleColorData> _roleColorMap = new Dictionary<RoleTypes, RoleColorData>
    {
        /*=== 船员阵营 === */
        { RoleTypes.Crewmate, new RoleColorData(30, 144, 255, "#1E90FF") },// 船员 => 道奇蓝
        { RoleTypes.Noisemaker, new RoleColorData(0, 191, 255, "#00BFFF") },// 大嗓门 => 深天蓝
        { RoleTypes.Scientist, new RoleColorData(0, 255, 255, "#00FFFF") },// 科学家 => 青色
        { RoleTypes.Engineer, new RoleColorData(127, 255, 170, "#7FFFAA") },// 工程师 => 绿玉
        { RoleTypes.Tracker, new RoleColorData(0, 128, 128, "#008080") },// 侦察兵 => 水鸭色
        { RoleTypes.Detective, new RoleColorData(45, 255, 33, "#2DFF21") },// 侦探 => 青绿色
        
        /*=== 内鬼阵营 === */
        { RoleTypes.Impostor, new RoleColorData(255, 0, 0, "#FF0000") },// 内鬼 => 纯红
        { RoleTypes.Shapeshifter, new RoleColorData(255, 69, 0, "#FF4500") },// 变形 => 橙红
        { RoleTypes.Phantom, new RoleColorData(250, 128, 114, "#FA8072") },// 隐身 => 鲜肉
        { RoleTypes.Viper, new RoleColorData(250, 0, 255, "#FA00FF") },// 毒蛇 => 浅粉色
        
        /*=== 灵魂阵营 === */
        { RoleTypes.CrewmateGhost, new RoleColorData(220, 220, 220, "#DCDCDC") },// 船员灵魂 => 亮灰色
        { RoleTypes.GuardianAngel, new RoleColorData(240, 128, 128, "#F08080") },// 天使 => 淡珊瑚
        { RoleTypes.ImpostorGhost, new RoleColorData(255, 228, 225, "#FFE4E1") }// 内鬼灵魂 => 薄雾玫瑰
    };

    // 默认颜色（当找不到对应角色时使用）
    private static readonly RoleColorData _defaultColor = new RoleColorData(128, 128, 128, "#808080"); // 灰色

    /// <summary>
    /// 获取角色对应的Color
    /// </summary>
    public static Color GetRoleColor(RoleTypes rt)
    {
        var colorData = GetRoleColorData(rt);
        return new Color(
            colorData.R / 255f, 
            colorData.G / 255f, 
            colorData.B / 255f
        );
    }

    /// <summary>
    /// 获取角色对应的Color32
    /// </summary>
    public static Color32 GetRoleColor32(RoleTypes rt)
    {
        var colorData = GetRoleColorData(rt);
        return new Color32(
            colorData.R, 
            colorData.G, 
            colorData.B, 
            byte.MaxValue
        );
    }

    /// <summary>
    /// 获取角色对应的Hex颜色值
    /// </summary>
    public static string GetRoleColorHex(RoleTypes rt)
    {
        return GetRoleColorData(rt).HtmlHex;
    }

    /// <summary>
    /// 获取角色的颜色数据
    /// </summary>
    private static RoleColorData GetRoleColorData(RoleTypes rt)
    {
        return _roleColorMap.GetValueOrDefault(rt, _defaultColor);
    }

    /// <summary>
    /// 角色颜色数据结构，存储统一的颜色信息
    /// </summary>
    private struct RoleColorData
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
        public string HtmlHex { get; }

        public RoleColorData(byte r, byte g, byte b, string htmlHex)
        {
            R = r;
            G = g;
            B = b;
            HtmlHex = htmlHex;
        }
    }
}
