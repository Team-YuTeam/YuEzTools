using AmongUs.GameOptions;
using Hazel;
using System;
using InnerNet;
using YuEzTools.Modules;
using YuEzTools.Patches;
using YuEzTools.UI;
using YuEzTools.Utils;

namespace YuEzTools.AntiCheat;

internal class AntiCheatForAll
{
    public static int MeetingTimes = 0;
    public static int DeNum = 0;
    public static bool ReceiveRpc(PlayerControl pc, byte callId, MessageReader reader)
    {
        bool checkRPC = false;
        if (pc == null || reader == null) return false;
        try
        {
            MessageReader sr = MessageReader.Get(reader);
            var rpc = (RpcCalls)callId;
            // if (callId is 7 or 5 or 41 or 39 or 40 or 42 or 43 or 38 or 18 or 13)
            // {
            //
            // }
            // else if(!Main.JoinedPlayer.Contains(pc) && AmongUsClient.Instance.AmHost)
            // {
            //     SendInGamePatch.SendInGame(string.Format(GetString("notJoinedSendRPC"), callId, pc.GetRealName()));
            //     Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】未进入但发送RPC，无效！！！，已驳回", "AntiCheatForAll");
            //     return true;
            // }
            checkRPC = CheckRPC(pc, callId, sr, rpc);
            if (checkRPC && Toggles.AutoReportHacker)
            {
                AmongUsClient.Instance.ReportPlayer(pc.GetClientId(),ReportReasons.Cheating_Hacking);
                Info($"已尝试向Among Us官方举报【{pc.GetRealName()}-{pc.GetClient().ProductUserId}】","AntiCheatForAll");
            }
                
        }
        catch { }
        return checkRPC;
    }

    public static bool CheckRPC(PlayerControl pc, byte callId, MessageReader sr,RpcCalls rpc)
    {
        if (!Enum.IsDefined(typeof(RpcCalls), callId))
            {
                CustomTips.Show(string.Format(GetString("notFindRPC"), callId),TipsCode.AntiCheat);
                // SendInGamePatch.SendInGame(string.Format(GetString("notFindRPC"), callId));
                Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】RPC无效！！！，已驳回", "AntiCheatForAll");
                return true;
            }
            switch (rpc)
            {
                case RpcCalls.SetName:
                case RpcCalls.CheckName:
                    string name = sr.ReadString();
                    if (sr.BytesRemaining > 0 && sr.ReadBoolean()) return false;
                    if (GetPlayer.IsInGame)
                    {
                        Warn($"在游戏内非法修改玩家【{pc.GetClientId()}:{pc.GetRealName()}】的游戏名称，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    if (name.Contains('░') ||
                        name.Contains('▄') ||
                        name.Contains('█') ||
                        name.Contains('▌') ||
                        name.Contains('▒') ||
                        name.Contains("习近平") ||
                        name.Contains("毛泽东") ||
                        name.Contains("周恩来") ||
                        name.Contains("邓小平") ||
                        name.Contains("江泽民") ||
                        name.Contains("胡锦涛") ||
                        name.Contains("台湾") ||
                        name.Contains("台独") ||
                        name.Contains("温家宝") ||
                        name.Contains("共产党") ||
                        name.Contains("Ez", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("Hack", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("Cheat", StringComparison.OrdinalIgnoreCase))
                        // 游戏名字屏蔽词
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】的游戏名称包含*屏蔽词*，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case RpcCalls.SetNamePlateStr:
                    if (GetPlayer.IsInGame)
                    {
                        Warn($"非法修改玩家【{pc.GetClientId()}:{pc.GetRealName()}】的游戏名称，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case RpcCalls.SendChatNote:
                    if (GetPlayer.IsLobby)
                    {
                        Warn($"【{pc.GetClientId()}:{pc.GetRealName()}】非法发送投票信息，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case RpcCalls.SetScanner:
                    if (GetPlayer.IsLobby)
                    {
                        Warn($"【{pc.GetClientId()}:{pc.GetRealName()}】非法扫描，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case RpcCalls.SetTasks:
                    if (GetPlayer.IsMeeting || GetPlayer.IsLobby || GetPlayer.IsInGame || pc.GetClient() != AmongUsClient.Instance.GetHost())
                    {
                        Warn($"【{pc.GetClientId()}:{pc.GetRealName()}】非法设置玩家的任务", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case RpcCalls.SetRole:
                    var role = (RoleTypes)sr.ReadUInt16();
                    var canOverrideRole = sr.ReadBoolean();
                    if (GetPlayer.IsLobby && (role is RoleTypes.CrewmateGhost or RoleTypes.ImpostorGhost))
                    {
                        Warn($"非法设置玩家【{pc.GetClientId()}:{pc.GetRealName()}】的状态为幽灵，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case RpcCalls.SendChat:
                    var text = sr.ReadString();
                    if (GetPlayer.IsInGame && !GetPlayer.IsMeeting && !pc.Data.IsDead)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法聊天，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    if (text.Contains('░') ||
                        text.Contains('▄') ||
                        text.Contains('█') ||
                        text.Contains('▌') ||
                        text.Contains('▒') ||
                        text.Contains("习近平") ||
                        text.Contains("毛泽东") ||
                        text.Contains("周恩来") ||
                        text.Contains("邓小平") ||
                        text.Contains("江泽民") ||
                        text.Contains("胡锦涛") ||
                        text.Contains("温家宝") ||
                        text.Contains("台湾") ||
                        text.Contains("台独") ||
                        text.Contains("共产党") || // 游戏名字屏蔽词
                        text.Contains("EzHacked", StringComparison.OrdinalIgnoreCase) ||
                        text.Contains("Ez Hacked", StringComparison.OrdinalIgnoreCase))
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】发送非法消息，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case RpcCalls.StartMeeting:
                    MeetingTimes++;
                    if (GetPlayer.IsLobby || GetPlayer.IsHideNSeek)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法召集会议：【null】，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case RpcCalls.ReportDeadBody:
                    var p1 = GetPlayer.GetPlayerById(sr.ReadByte());
                    if (p1 != null && GetPlayer.IsLobby)
                    {
                        Warn(
                            $"玩家【{pc.GetClientId()}:{pc.GetRealName()}】在大厅报告尸体：【{p1?.GetRealName() ?? "null"}】，已驳回", "AntiCheatForAll");
                        return true;
                    }

                    if (p1 != null && GetPlayer.IsHideNSeek)
                    {
                        Warn(
                            $"玩家【{pc.GetClientId()}:{pc.GetRealName()}】在躲猫猫报告尸体：【{p1?.GetRealName() ?? "null"}】，已驳回", "AntiCheatForAll");
                        return true;
                    }

                    if (p1 != null && !p1.Data.IsDead)
                    {
                        Warn(
                            $"玩家【{pc.GetClientId()}:{pc.GetRealName()}】报告活人尸体：【{p1?.GetRealName() ?? "null"}】，已驳回", "AntiCheatForAll");
                        return true;
                    }

                    RpcReportDeadBodyCheck(pc);
                    if (ReportTimes.TryGetValue(pc.PlayerId, out int rtimes))
                    {
                        // 我们都知道，一局游戏最大只有15人，而就算内鬼为1人，那也不可能达到14次尸体报告（一个人）
                        if (rtimes > 14)
                        {
                            Fatal($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】报告尸体满14次，已驳回", "AntiCheatForAll");
                            return true;
                        }
                    }
                    break;

                case RpcCalls.SetColor:
                case RpcCalls.CheckColor:
                    if (!AmongUsClient.Instance.AmHost) break;
                    var color = sr.ReadByte();
                    if (pc.Data.DefaultOutfit.ColorId != -1 &&
                        (Main.AllPlayerControls.Where(x => x.Data.DefaultOutfit.ColorId == color).Count() >= 5
                    || !GetPlayer.IsLobby || color < 0 || color > 18))
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法设置颜色，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case RpcCalls.MurderPlayer:
                case RpcCalls.CheckMurder:
                    var id = sr.ReadByte();
                    if (GetPlayer.IsLobby || pc.Data.IsDead || !pc.IsImpostor())
                    {
                        if (AmongUsClient.Instance.AmHost && !Toggles.SafeMode)
                        {
                            id.GetPlayerDataById().pc.Revive();
                            if (GetPlayer.IsLobby) GetPlayer.GetPlayerById(id).RpcSetRole(RoleTypes.Crewmate, true);
                            Warn($"尝试复活{id.GetPlayerDataById().pc.GetRealName()}", "AntiCheatForAll");
                        }
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法击杀，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case RpcCalls.CheckShapeshift:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Shapeshifter)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法变形请求，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case RpcCalls.RejectShapeshift:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Shapeshifter)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法取消变形，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case RpcCalls.StartVanish:
                case RpcCalls.CheckVanish:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Phantom)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法隐身，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case RpcCalls.StartAppear:
                case RpcCalls.CheckAppear:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Phantom)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法显形，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case RpcCalls.SetLevel:
                    if (GetPlayer.IsInGame)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法设置等级，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case RpcCalls.EnterVent:
                    if (!(pc.Data.RoleType == RoleTypes.Engineer || pc.Data.RoleType == RoleTypes.Impostor || pc.Data.RoleType == RoleTypes.Shapeshifter || pc.Data.RoleType == RoleTypes.Phantom))
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法进入管道，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;
            }

            switch (callId)
            {
                case 13:
                    if (GetPlayer.IsInGame && !GetPlayer.IsMeeting && !pc.Data.IsDead)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法聊天，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case 7:
                case 8:
                    if (!AmongUsClient.Instance.AmHost) break;
                    if (!GetPlayer.IsLobby)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法设置颜色，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case 11:
                    MeetingTimes++;
                    if (GetPlayer.IsLobby || GetPlayer.IsHideNSeek)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法召集会议：【null】，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case 5:
                    string name = sr.ReadString();
                    if (GetPlayer.IsInGame)
                    {
                        Warn($"非法修改玩家【{pc.GetClientId()}:{pc.GetRealName()}】的游戏名称，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case 12:
                case 47:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || !pc.IsImpostor())
                    {
                        var id = sr.ReadByte();
                        if (AmongUsClient.Instance.AmHost && !Toggles.SafeMode)
                        {
                            id.GetPlayerDataById().pc.Revive();
                            if (GetPlayer.IsLobby) GetPlayer.GetPlayerById(id).RpcSetRole(RoleTypes.Crewmate, true);
                            Warn($"尝试复活{id.GetPlayerDataById().pc.GetRealName()}", "AntiCheatForAll");
                        }
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法击杀，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case 28:
                    // 据说是以前的破坏rpc
                    break;

                case 41:
                    if (GetPlayer.IsInGame)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法设置宠物，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case 40:
                    if (GetPlayer.IsInGame)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法设置皮肤，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case 42:
                    if (GetPlayer.IsInGame)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法设置面部装扮，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case 39:
                    if (GetPlayer.IsInGame)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法设置帽子，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case 43:
                    if (sr.BytesRemaining > 0 && sr.ReadBoolean()) return false;
                    if (GetPlayer.IsInGame)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法设置名称，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case 38:
                    if (GetPlayer.IsInGame)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法设置等级，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case 55:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Shapeshifter)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法变形请求，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case 56:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Shapeshifter)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法取消变形，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case 62:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Phantom)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法隐身请求，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case 63:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Phantom)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法隐身，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case 64:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Phantom)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法显形请求，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case 65:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Phantom)
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法显形，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;

                case 19:
                    if (!(pc.Data.RoleType == RoleTypes.Engineer || pc.Data.RoleType == RoleTypes.Impostor || pc.Data.RoleType == RoleTypes.Shapeshifter || pc.Data.RoleType == RoleTypes.Phantom))
                    {
                        Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法进入管道，已驳回", "AntiCheatForAll");
                        return true;
                    }
                    break;
            }

            return false;
    }
    public static Dictionary<byte, int> ReportTimes = [];

    public static bool RpcReportDeadBodyCheck(PlayerControl player)
    {
        if (!ReportTimes.ContainsKey(player.PlayerId))
        {
            ReportTimes.Add(player.PlayerId, 0);
        }
        ReportTimes[player.PlayerId]++;
        return false;
    }

    public static bool RpcUpdateSystemCheck(PlayerControl player, SystemTypes systemType, byte amount)
    {
        // 更新系统 rpc 无法被 playercontrol.handlerpc 接收
        var Mapid = GetPlayer.GetActiveMapId();
        Info("Check sabotage RPC" + ", PlayerName: " + player.GetRealName() + ", SabotageType: " + systemType.ToString() + ", amount: " + amount.ToString(), "AntiCheatForAll");
        // if (!AmongUsClient.Instance.AmHost) return false;
        Info("触发飞船事件！" + player.GetRealName() + $"是{player.GetPlayerRoleTeam()}阵营！", "AntiCheatForAll");
        if (player == null) return false;

        /*if (systemType == SystemTypes.Sabotage) //使用正常的破坏按钮
        {
            if (GetPlayer.GetPlayerRoleTeam(player) != RoleTeam.Impostor)
            {
            Logger.Fatal($"玩家【{player.GetClientId()}:{player.GetRealName()}】非法破坏A，已驳回", "AntiCheatForAll");
            return true;
            }
        }*/
        //外挂直接发送 128 个系统型 rpc
        else if (systemType == SystemTypes.LifeSupp)
        {
            if (Mapid != 0 && Mapid != 1 && Mapid != 3) goto YesCheat;
            else if (amount != 64 && amount != 65) goto YesCheat;
        }

        else if (systemType == SystemTypes.Comms)
        {
            if (amount == 0)
            {
                if (Mapid == 1 || Mapid == 5) goto YesCheat;
            }
            else if (amount == 64 || amount == 65 || amount == 32 || amount == 33 || amount == 16 || amount == 17)
            {
                if (!(Mapid == 1 || Mapid == 5)) goto YesCheat;
            }
            else goto YesCheat;
        }

        else if (systemType == SystemTypes.Electrical)
        {
            if (Mapid == 5) goto YesCheat;
            if (amount >= 5) // 0 - 4个正常灯。其他的破坏，不应该由客户发送
            {
                goto YesCheat;
            }
        }

        else if (systemType == SystemTypes.Laboratory)
        {
            if (Mapid != 2) goto YesCheat;
            else if (!(amount == 64 || amount == 65 || amount == 32 || amount == 33)) goto YesCheat;
        }

        else if (systemType == SystemTypes.Reactor)
        {
            if (Mapid == 2 || Mapid == 4) goto YesCheat;
            else if (!(amount == 64 || amount == 65 || amount == 32 || amount == 33)) goto YesCheat;
            // 飞艇使用直升机破坏/其他用途64,65 | 32,33
        }

        else if (systemType == SystemTypes.HeliSabotage)
        {
            if (Mapid != 4) goto YesCheat;
            else if (!(amount == 64 || amount == 65 || amount == 16 || amount == 17 || amount == 32 || amount == 33)) goto YesCheat;
        }

        else if (systemType == SystemTypes.MushroomMixupSabotage)
        {
            goto YesCheat;
            // 普通客户永远不会直接发送MushroomMixupSabotage
        }

        if (GetPlayer.IsMeeting && MeetingHud.Instance.state != MeetingHud.VoteStates.Animating || GetPlayer.IsExilling)
        {
            Fatal($"玩家【{player.GetClientId()}:{player.GetRealName()}非法破坏D，已驳回", "AntiCheatForAll");
            return true;
        }
        // 可能会出现这样的情况：玩家正在修复反应堆，而会议开始了，从而触发会议中的 AntiCheatForAll 检查
        return false;

    YesCheat:
        {
            Fatal($"玩家【{player.GetClientId()}:{player.GetRealName()}】非法破坏C，已驳回", "AntiCheatForAll");
            return true;
        }
    }
    public static bool RpcUpdateSystemCheckFHS(PlayerControl player, SystemTypes systemType, byte amount)
    {
        // 更新系统 rpc 无法被 playercontrol.handlerpc 接收
        _ = GetPlayer.GetActiveMapId();
        Info("Check sabotage RPC" + ", PlayerName: " + player.GetRealName() + ", SabotageType: " + systemType.ToString() + ", amount: " + amount.ToString(), "AntiCheatForAll");
        // if (!AmongUsClient.Instance.AmHost) return false;
        Info("触发飞船事件！" + player.GetRealName() + $"是{player.GetPlayerRoleTeam()}阵营！", "AntiCheatForAll");
        if (player == null) return false;

        if (systemType == SystemTypes.Sabotage || systemType == SystemTypes.LifeSupp || systemType == SystemTypes.Comms || systemType == SystemTypes.Electrical || systemType == SystemTypes.Laboratory || systemType == SystemTypes.Reactor || systemType == SystemTypes.HeliSabotage || systemType == SystemTypes.MushroomMixupSabotage) //使用破坏
        {
            goto YesCheat;
        }

        if (GetPlayer.IsMeeting && MeetingHud.Instance.state != MeetingHud.VoteStates.Animating || GetPlayer.IsExilling)
        {
            Fatal($"玩家【{player.GetClientId()}:{player.GetRealName()}非法破坏D，已驳回", "AntiCheatForAll");
            return true;
        }
        // 可能会出现这样的情况：玩家正在修复反应堆，而会议开始了，从而触发会议中的 AntiCheatForAll 检查
        return false;

    YesCheat:
        {
            Fatal($"玩家【{player.GetClientId()}:{player.GetRealName()}】在躲猫猫非法破坏，已驳回", "AntiCheatForAll");
            return true;
        }
    }
}
