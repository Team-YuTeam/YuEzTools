using AmongUs.GameOptions;
using YuEzTools.Attributes;
using UnityEngine;
using YuEzTools.Helpers;
using YuEzTools.Utils;

namespace YuEzTools.Modules;

public class ModPlayerData
{
    // 这里是全部人的ModPlayerData
    public static Dictionary<byte, ModPlayerData> AllPlayerDataForMod;
    // 当前玩家MPD的PC获取
    public PlayerControl pc { get; private set; }

    // 基本信息
    public string Name { get; private set; }
    public int ColorId { get; private set; }
    public Color Color { get; private set; }
    public RoleTypes role { get; private set; }
    public RoleTypes? RoleAfterDeath { get; private set; }
    public bool RoleHas { get; private set; }

    // 其他
    public bool IsDead { get; private set; }
    public DeadReasonData DeadReason  { get; private set; }
    public ModPlayerData Killer  { get; private set; }
    public bool IsDisconnected { get; private set; }
    public bool IsExiled { get; private set; }
    public bool IsByKilled { get; private set; }
    public string PUID { get; private set; }

    public ModPlayerData(PlayerControl Player, string name, int colorid,Color color)
    {
        pc = Player;
        Name = name;
        ColorId = colorid;
        Color = color;
        IsDead = false;
        IsExiled = false;
        IsByKilled = false;
        DeadReason = DeadReasonData.Alive;
        PUID = pc.GetClient().ProductUserId;
    }

    [GameModuleInitializer]
    public static void Init()
    {
        AllPlayerDataForMod = new();
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            var colorId = player.Data.DefaultOutfit.ColorId;
            var id = player.PlayerId;
            var color = player.Data.Color;
            var data = new ModPlayerData(
                player,
                player.GetRealName(),
                colorId,
                color);
            AllPlayerDataForMod[id] = data;
        }
    }

    // 到后面，便是获取了
    public static ModPlayerData GetModPlayerDataById(byte id) => AllPlayerDataForMod[id] ?? null;
    public static string GetModPlayerDataName(byte id) => GetModPlayerDataById(id).Name;

    // 功能
    public void SetDead() => IsDead = true;
    public void SetExiled() => IsExiled = true;
    public void SetDeadReason(DeadReasonData deathReason)
    {
        if (DeadReason == DeadReasonData.Alive)
            DeadReason = deathReason;
    }
    public void SetDisconnected()
    {
        IsDisconnected = true;
        SetDead();
        SetDeadReason(DeadReasonData.Disconnect);
    }
    public void SetRole(RoleTypes Role)
    {
        if (!RoleHas)
            role = Role;
        else
            RoleAfterDeath = Role;

        RoleHas = true;
    }
    public void SetKiller(PlayerControl killer)
    {
        IsByKilled = true;
        Killer = killer.GetPlayerData();
        SetDeadReason(DeadReasonData.Kill);
    }
    public static int GetLongestNameByteCount() => AllPlayerDataForMod.Values.Select(data => data.Name.GetByteCount()).OrderByDescending(byteCount => byteCount).FirstOrDefault();
}

static class PlayerControlData
{
    public static ModPlayerData GetPlayerData(this PlayerControl pc) =>
        ModPlayerData.GetModPlayerDataById(pc.PlayerId) ?? null;
    public static ModPlayerData GetPlayerDataById(this byte PlayerId) =>
        ModPlayerData.GetModPlayerDataById(PlayerId) ?? null;
    public static string GetPlayerName(this PlayerControl pc) => ModPlayerData.GetModPlayerDataName(pc.PlayerId);
    public static RoleTypes GetRole(this PlayerControl pc) => pc.Data.RoleType;
    public static string GetRoleName(this PlayerControl pc) => pc.Data.Role.NiceName;
    public static void SetDisconnected(this PlayerControl pc) => pc.GetPlayerData().SetDisconnected();
    public static void SetRole(this PlayerControl pc, RoleTypes role) => pc.GetPlayerData().SetRole(role);
    public static bool IsImpostor(this PlayerControl pc) => pc.GetPlayerRoleTeam() == RoleTeam.Impostor;
}
// 死亡原因
public enum DeadReasonData
{
    Alive,
    Exile,
    Kill,
    Disconnect
}