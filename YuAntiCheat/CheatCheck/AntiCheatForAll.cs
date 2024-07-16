using AmongUs.GameOptions;
using Hazel;
using System.Linq;
using YuAntiCheat.Get;

namespace YuAntiCheat;

internal class AntiCheatForAll
{
    public static int MeetingTimes = 0;
    public static int DeNum = 0;
    
    public static bool ReceiveRpc(PlayerControl pc, byte callId, MessageReader reader)
    {
        if (pc == null || reader == null || pc.AmOwner) return false;
        try
        {
            MessageReader sr = MessageReader.Get(reader);
            var rpc = (RpcCalls)callId;
            switch (rpc)
            {
                case RpcCalls.SetName:
                    string name = sr.ReadString();
                    if (sr.BytesRemaining > 0 && sr.ReadBoolean()) return false;
                    if (GetPlayer.IsInGame)
                    {
                        Main.Logger.LogWarning($"非法修改玩家【{pc.GetClientId()}:{pc.GetRealName()}】的游戏名称，已驳回");
                        return true;
                    }
                    if (
                        ((name.Contains("<size") || name.Contains("size>")) && name.Contains("?") && !name.Contains("color")) ||
                        name.Length > 160 ||
                        name.Count(f => f.Equals("\"\\n\"")) > 3 ||
                        name.Count(f => f.Equals("\n")) > 3 ||
                        name.Count(f => f.Equals("\r")) > 3 ||
                        name.Contains("░") ||
                        name.Contains("▄") ||
                        name.Contains("█") ||
                        name.Contains("▌") ||
                        name.Contains("▒") ||
                        name.Contains("习近平") ||
                        name.Contains("毛泽东") ||
                        name.Contains("周恩来") ||
                        name.Contains("邓小平") ||
                        name.Contains("江泽民") ||
                        name.Contains("胡锦涛") ||
                        name.Contains("台湾") || 
                        name.Contains("台独") || 
                        name.Contains("温家宝") || 
                        name.Contains("共产党") // 游戏名字屏蔽词
                        )
                    {
                        Main.Logger.LogWarning($"非法修改玩家【{pc.GetClientId()}:{pc.GetRealName()}】的游戏名称，已驳回");
                        return true;
                    }
                    break;
                
                case RpcCalls.SetNamePlateStr:
                    if (GetPlayer.IsInGame)
                    {
                        Main.Logger.LogWarning($"非法修改玩家【{pc.GetClientId()}:{pc.GetRealName()}】的游戏名称，已驳回");
                        return true;
                    }

                    break;
                
                case RpcCalls.SetTasks:
                    if (GetPlayer.IsMeeting || GetPlayer.IsLobby || GetPlayer.IsInGame || pc.GetClient() != AmongUsClient.Instance.GetHost())
                    {
                        Main.Logger.LogWarning($"【{pc.GetClientId()}:{pc.GetRealName()}】非法设置玩家的任务，已驳回");
                        return true;
                    }
                    break;
                
                case RpcCalls.SetRole:
                    var role = (RoleTypes)sr.ReadUInt16();
                    var canOverrideRole = sr.ReadBoolean();
                    if (GetPlayer.IsLobby && (role is RoleTypes.CrewmateGhost or RoleTypes.ImpostorGhost))
                    {
                        Main.Logger.LogWarning($"非法设置玩家【{pc.GetClientId()}:{pc.GetRealName()}】的状态为幽灵，已驳回");
                        return true;
                    }
                    break;
                
                case RpcCalls.SendChat:
                    var text = sr.ReadString();
                    if (text.StartsWith("/")) return false;
                    if (GetPlayer.IsInGame && GetPlayer.IsMeeting && pc.Data.IsDead)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法聊天，已驳回");
                        return true;
                    }
                    if (
                        text.Contains("░") ||
                        text.Contains("▄") ||
                        text.Contains("█") ||
                        text.Contains("▌") ||
                        text.Contains("▒") ||
                        text.Contains("习近平") ||
                        text.Contains("毛泽东") ||
                        text.Contains("周恩来") ||
                        text.Contains("邓小平") ||
                        text.Contains("江泽民") ||
                        text.Contains("胡锦涛") ||
                        text.Contains("温家宝") ||
                        text.Contains("台湾") || 
                        text.Contains("台独") || 
                        text.Contains("共产党") // 游戏名字屏蔽词
                    )
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】发送非法消息，已驳回");
                        return true;
                    }
                    break;
                
                case RpcCalls.StartMeeting:
                    MeetingTimes++;
                    if ((GetPlayer.IsMeeting && MeetingTimes > 3) || GetPlayer.IsLobby)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法召集会议：【null】，已驳回");
                        return true;
                    }
                    break;
                
                case RpcCalls.ReportDeadBody:
                    var p1 = GetPlayer.GetPlayerById(sr.ReadByte());
                    if (p1 != null && GetPlayer.IsLobby) //&& !PlayerState.IsDead(p1))
                    {
                        Main.Logger.LogWarning(
                            $"玩家【{pc.GetClientId()}:{pc.GetRealName()}】在大厅报告尸体：【{p1?.GetRealName() ?? "null"}】，已驳回");
                        return true;
                    }
                    if (p1 != null && !p1.Data.IsDead)
                    {
                        Main.Logger.LogWarning(
                            $"玩家【{pc.GetClientId()}:{pc.GetRealName()}】报告活人尸体：【{p1?.GetRealName() ?? "null"}】，已驳回");
                        return true;
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
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法设置颜色，已驳回");
                        return true;
                    }
                    break;
                
                case RpcCalls.CheckMurder:
                case RpcCalls.MurderPlayer:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || (pc.Data.RoleType != RoleTypes.Impostor && pc.Data.RoleType != RoleTypes.Shapeshifter && pc.Data.RoleType != RoleTypes.Phantom))
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法击杀，已驳回");
                        return true;
                    }
                    break;

                case RpcCalls.CheckShapeshift:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Shapeshifter)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法变形请求，已驳回");
                        return true;
                    }
                    break;

                case RpcCalls.RejectShapeshift:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Shapeshifter)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法取消变形，已驳回");
                        return true;
                    }
                    break;
                
                case RpcCalls.CheckVanish:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Phantom)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法隐身请求，已驳回");
                        return true;
                    }
                    break;
                
                case RpcCalls.StartVanish:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Phantom)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法隐身，已驳回");
                        return true;
                    }
                    break;

                case RpcCalls.CheckAppear:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Phantom)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法显形请求，已驳回");
                        return true;
                    }
                    break;

                case RpcCalls.StartAppear:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Phantom)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法显形，已驳回");
                        return true;
                    }
                    break;
                
                case RpcCalls.SetLevel:
                    if (GetPlayer.IsInGame)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法设置等级，已驳回");
                        return true;
                    }
                    break;

                case RpcCalls.EnterVent:
                    if (!(pc.Data.RoleType == RoleTypes.Engineer||pc.Data.RoleType == RoleTypes.Impostor||pc.Data.RoleType == RoleTypes.Shapeshifter||pc.Data.RoleType == RoleTypes.Phantom))
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法进入管道，已驳回");
                        return true;
                    }
                    break;
            }

            switch (callId)
            {
                case 101:
                    var AUMChat = sr.ReadString();
                    Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】AUMChat，内容：{AUMChat}");
                    return true;
                
                case 13:
                    if (GetPlayer.IsInGame && GetPlayer.IsMeeting && pc.Data.IsDead)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法聊天，已驳回");
                        return true;
                    }
                    break;

                case 7:
                    if (!AmongUsClient.Instance.AmHost) break;
                    if (!GetPlayer.IsLobby)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法设置颜色，已驳回");
                        return true;
                    }
                    break;

                case 11:
                    MeetingTimes++;
                    if ((GetPlayer.IsMeeting && MeetingTimes > 3) || GetPlayer.IsLobby)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法召集会议：【null】，已驳回");
                        return true;
                    }
                    break;

                case 5:
                    string name = sr.ReadString();
                    if (GetPlayer.IsInGame)
                    {
                        Main.Logger.LogWarning($"非法修改玩家【{pc.GetClientId()}:{pc.GetRealName()}】的游戏名称，已驳回");
                        return true;
                    }
                    break;
                
                case 12:
                case 47:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || (pc.Data.RoleType != RoleTypes.Impostor && pc.Data.RoleType != RoleTypes.Shapeshifter && pc.Data.RoleType != RoleTypes.Phantom))
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法击杀，已驳回");
                        return true;
                    }
                    break;

                case 41:
                    if (GetPlayer.IsInGame)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法设置宠物，已驳回");
                        return true;
                    }
                    break;

                case 40:
                    if (GetPlayer.IsInGame)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法设置皮肤，已驳回");
                        return true;
                    }
                    break;

                case 42:
                    if (GetPlayer.IsInGame)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法设置面部装扮，已驳回");
                        return true;
                    }
                    break;

                case 39:
                    if (GetPlayer.IsInGame)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法设置帽子，已驳回");
                        return true;
                    }
                    break;

                case 43:
                    if (sr.BytesRemaining > 0 && sr.ReadBoolean()) return false;
                    if (GetPlayer.IsInGame)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法设置名称，已驳回");
                        return true;
                    }
                    break;

                case 38:
                    if (GetPlayer.IsInGame)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法设置等级，已驳回");
                        return true;
                    }
                    break; 

                case 55:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Shapeshifter)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法变形请求，已驳回");
                        return true;
                    }
                    break;

                case 56:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Shapeshifter)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法取消变形，已驳回");
                        return true;
                    }
                    break;

                case 62:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Phantom)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法隐身请求，已驳回");
                        return true;
                    }
                    break;

                case 63:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Phantom)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法隐身，已驳回");
                        return true;
                    }
                    break;

                case 64:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Phantom)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法显形请求，已驳回");
                        return true;
                    }
                    break;

                case 65:
                    if (GetPlayer.IsLobby || pc.Data.IsDead || pc.Data.RoleType != RoleTypes.Phantom)
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法显形，已驳回");
                        return true;
                    }
                    break;

                case 19:
                    if (!(pc.Data.RoleType == RoleTypes.Engineer||pc.Data.RoleType == RoleTypes.Impostor||pc.Data.RoleType == RoleTypes.Shapeshifter||pc.Data.RoleType == RoleTypes.Phantom))
                    {
                        Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】非法进入管道，已驳回");
                        return true;
                    }
                    break;
            }
        }
        catch
        {
            //
        }
        return false;
    }

    //Thanks to https://github.com/0xDrMoe/TownofHost-Enhanced/blob/main/Modules/EAC.cs
    public static bool RpcUpdateSystemCheck(PlayerControl pc, SystemTypes systemType, byte amount)
    {
        //Update system rpc can not get received by playercontrol.handlerpc
        var Mapid = GetPlayer.GetActiveMapId();
        Main.Logger.LogWarning("Check sabotage RPC" + ", PlayerName: " + pc.GetRealName() + ", SabotageType: " + systemType.ToString() + ", amount: " + amount.ToString());
        if (!AmongUsClient.Instance.AmHost) return false;

        if (pc == null)
        {
            Main.Logger.LogWarning("PlayerControl is null");
            return true;
        }

        if (systemType == SystemTypes.Sabotage) //Normal sabotage using buttons
        {
            if (!(pc.Data.RoleType is RoleTypes.Impostor or RoleTypes.Shapeshifter or RoleTypes.ImpostorGhost or RoleTypes.Phantom))
            {
                Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】以非伪装者身份破坏A，已驳回");
                return true;
            }
        } //Cheater directly send 128 systemtype rpc
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
            if (amount >= 5) //0 - 4 normal lights. other sabotage, Should never be sent by client
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
            //Airship use heli sabotage /Other use 64,65 | 32,33
        }
        else if (systemType == SystemTypes.HeliSabotage)
        {
            if (Mapid != 4) goto YesCheat;
            else if (!(amount == 64 || amount == 65 || amount == 16 || amount == 17 || amount == 32 || amount == 33)) goto YesCheat;
        }
        else if (systemType == SystemTypes.MushroomMixupSabotage)
        {
            goto YesCheat;
            //Normal clients will never directly send MushroomMixupSabotage
        }

        if (GetPlayer.IsMeeting && MeetingHud.Instance.state != MeetingHud.VoteStates.Animating || GetPlayer.isExiling)
        {
            Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】会议期间非法破坏D，已驳回");
            return true;
        }
        //There may be cases where a player is fixing reactor and a meeting start, triggering EAC check in meeting

        return false;

    YesCheat:
        {
            Main.Logger.LogWarning($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】直接发包非法破坏C，已驳回");
            return true;
        }
    }
}
