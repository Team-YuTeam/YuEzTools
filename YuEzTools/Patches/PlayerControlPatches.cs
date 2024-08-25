using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AmongUs.GameOptions;
using HarmonyLib;
using TMPro;
using UnityEngine;
using YuEzTools.Get;
using YuEzTools.Modules;
using static YuEzTools.Translator;
using YuEzTools.Get;
using YuEzTools.Utils;
using Object = UnityEngine.Object;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
class MurderPlayerPatch
{
    public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl tg)
    {
        // if (Toggles.AutoStartGame && __instance == tg && __instance == PlayerControl.LocalPlayer) return true;
        if (GetPlayer.IsLobby || tg.Data.IsDead || __instance.Data.IsDead || __instance.GetPlayerRoleTeam() != RoleTeam.Impostor)
        {
            if (!Main.HackerList.Contains(__instance.GetClientId())) Main.HackerList.Add(__instance.GetClientId());
            Main.HasHacker = true;
            Logger.Fatal(
                "Hacker Murder " + __instance.GetRealName() +
                $"{"好友编号：" + __instance.GetClient().FriendCode + "/名字：" + __instance.GetRealName() + "/ProductUserId：" + __instance.GetClient().ProductUserId}",
                "RPCHandle");
            //Main.PlayerStates[__instance.GetClient().Id].IsHacker = true;
            SendChat.Prefix(__instance);
            Utils.Utils.AddHacker(__instance.GetClient());
            if (!Toggles.SafeMode && !AmongUsClient.Instance.AmHost &&
                GameStartManagerPatch.roomMode == RoomMode.Plus25)
            {
                Main.Logger.LogInfo("Try Kick" + __instance.GetRealName());
                KickHackerPatch.KickPlayer(__instance);
                return false;
            }
            //PlayerControl Host = AmongUsClient.Instance.GetHost();
            else if (AmongUsClient.Instance.AmHost)
            {
                Main.Logger.LogInfo("Host Try ban " + __instance.GetRealName());
                AmongUsClient.Instance.KickPlayer(__instance.GetClientId(), true);
                if (AmongUsClient.Instance.AmHost && !Toggles.SafeMode)
                {
                    tg.Revive();
                    if(GetPlayer.IsLobby) tg.RpcSetRole(RoleTypes.Crewmate,true);
                            
                    Main.Logger.LogWarning($"尝试复活{tg.GetRealName()}");
                }
                Main.Logger.LogWarning($"玩家【{__instance.GetClientId()}:{__instance.GetRealName()}】非法击杀，已驳回");
                
                if (GetPlayer.IsInGame)
                {
                    Main.Logger.LogInfo("Host Try end game with room " +
                                        GameStartManager.Instance.GameRoomNameCode.text);
                    try
                    {
                        GameManager.Instance.RpcEndGame(GameOverReason.ImpostorDisconnect, false);

                    }
                    catch (System.Exception e)
                    {
                        Logger.Error(e.ToString(), "Session");
                    }

                    Main.HasHacker = false;
                }

                return false;
            }

            return false;
        }

        return true;
    }

    public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
    {
        if (target.GetPlayerData().Killer != null) return;
        target.GetPlayerData().SetDeadReason(DeadReasonData.Kill);
        target.GetPlayerData().SetKiller(__instance);
        target.GetPlayerData().SetDead();
        __instance.AddKillCount();
    }
}
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
class PlayerStartPatch
{
    public static void Postfix(PlayerControl __instance)
    {
        var AddText = Object.Instantiate(__instance.cosmetics.nameText);
        AddText.transform.SetParent(__instance.cosmetics.nameText.transform);
        AddText.transform.localPosition = new Vector3(0f, -1.7f, 0f);
        AddText.text = "AddText";
        AddText.fontSize = 1.5f;
        AddText.gameObject.name = "AddText";
        AddText.gameObject.SetActive(false);
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
class FixedUpdatePatch
{
    public static void Postfix(PlayerControl __instance)
    {
        if (__instance == null) return;

        try
        {
            // var color ="#ffffff";
            var name = "";
            var addText = __instance.cosmetics.nameText.transform.Find("AddText").GetComponent<TextMeshPro>();

            if (GetPlayer.IsLobby)
            {
                if (__instance.FriendCode.IsDevUser())
                    name = __instance.FriendCode.GetDevUser().GetTag();

                if (Toggles.ShowInfoInLobby)
                {
                    name =
                        $"<size=70%><color=#33EEFF>Lv.{__instance.GetClient().PlayerLevel} {__instance.GetClient().PlatformData.Platform.GetPlatformText()} {__instance.GetClient().Id}</color></size>\n" +
                        $"<size=65%><color=#33FF91>{__instance.PlayerId} {__instance.GetClient().FriendCode} {__instance.GetClient().GetHashedPuid()}</color></size>";
                }
            }

            if (GetPlayer.IsInGame)
            {
                // color = Utils.Utils.GetRoleHtmlColor(__instance.Data.RoleType);
                if (__instance == PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Data.IsDead || (__instance.IsImpostor() && PlayerControl.LocalPlayer.IsImpostor()))
                {
                    name = Utils.Utils.ColorString(Utils.Utils.GetRoleColor32(__instance.Data.RoleType),
                        __instance.GetRoleName());
                    __instance.cosmetics.nameText.text = Utils.Utils.ColorString(Utils.Utils.GetRoleColor32(__instance.Data.RoleType)
                        , __instance.cosmetics.nameText.text);
                    name += "\n" + __instance.PlayerId.GetKillOrTaskCountText();
                }

                if (PlayerControl.LocalPlayer.Data.IsDead && __instance.Data.IsDead)
                    name += $"\n{Utils.Utils.GetDeadText(__instance)}";
            }

            if (Main.HackerList.Contains(__instance.GetClientId()) &&
                !__instance.cosmetics.nameText.text.Has($"<color=#3FBAFF>[{GetString("Hacker")}]</color>"))
            {
                __instance.cosmetics.nameText.text += $"<color=#3FBAFF>[{GetString("Hacker")}]</color>";
            }

            addText.text = name;

            if (name != "")
            {
                addText.gameObject.SetActive(true);
            }
        }
        catch
        {
            Logger.Warn("被抛出来了...","PlayerControlPatches.FixedUpdate");
        }
        

        // __instance.cosmetics.nameText.text = name + "\n";
        // __instance.cosmetics.nameText.alignment = TextAlignmentOptions.Center;
    }
}
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
class CompleteTaskPatch
{
    public static void Postfix(PlayerControl __instance)
    {
        __instance.AddTaskCount();
    }
}
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetTasks))] 
class PlayerControlSetTasksPatch
{
    public static int TaskCount = 0;
    public static void Postfix([HarmonyArgument(0)] Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo.TaskInfo> Tasks)
    {
        TaskCount = Tasks.Count;
    }
}
[HarmonyPatch(typeof(CustomNetworkTransform), nameof(CustomNetworkTransform.SnapTo))]
[HarmonyPatch(typeof(CustomNetworkTransform), nameof(CustomNetworkTransform.RpcSnapTo))]
class SnapToPatch
{
    public static bool Prefix(Vector2 position)
    {
        Logger.Warn($"违规！进行了违规的传送...","SnapToPatch");
        // Logger.Warn($"违规！{__instance.GetRealName()} 进行了违规的传送...","SnapToPatch");
        // if(!Main.HackerList.Contains(__instance.GetClientId())) Main.HackerList.Add(__instance.GetClientId());
        // Main.HasHacker = true;
        // Logger.Fatal("Hacker " + __instance.GetRealName() + $"{"好友编号："+__instance.GetClient().FriendCode+"/名字："+__instance.GetRealName()+"/ProductUserId："+__instance.GetClient().ProductUserId}","SnapToPatch");
        // //Main.PlayerStates[__instance.GetClient().Id].IsHacker = true;
        // SendChat.Prefix(__instance);
        // Utils.Utils.AddHacker(__instance.GetClient());
        // if(!Toggles.SafeMode && !AmongUsClient.Instance.AmHost && GameStartManagerPatch.roomMode == RoomMode.Plus25)
        // {
        //     Main.Logger.LogInfo("Try Kick" + __instance.GetRealName());
        //     KickHackerPatch.KickPlayer(__instance);
        //     return false;
        // }
        // //PlayerControl Host = AmongUsClient.Instance.GetHost();
        // else if (AmongUsClient.Instance.AmHost)
        // {
        //     Main.Logger.LogInfo("Host Try ban " + __instance.GetRealName());
        //     AmongUsClient.Instance.KickPlayer(__instance.GetClientId(), true);
        //     if(GetPlayer.IsInGame)
        //     {
        //         Main.Logger.LogInfo("Host Try end game with room " +
        //                             GameStartManager.Instance.GameRoomNameCode.text);
        //         try
        //         {
        //             GameManager.Instance.RpcEndGame(GameOverReason.ImpostorDisconnect, false);
        //
        //         }
        //         catch (System.Exception e)
        //         {
        //             Logger.Error(e.ToString(), "SnapToPatch");
        //         }
        //         Main.HasHacker = false;
        //     }
        //     return false;
        // }
        return false;
    }
}


// // [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetRoleInvisibility))]
// // class SetRoleInvisibility
// // {
// //     public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] bool isActive, [HarmonyArgument(1)] bool animate)
// //     {
// //         if (isActive)
// //         {
// //             DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 2);
// //             defaultInterpolatedStringHandler.AppendFormatted(__instance.Data.PlayerName);
// //             defaultInterpolatedStringHandler.AppendLiteral(" Has Vanished as Phantom, did animate: ");
// //             defaultInterpolatedStringHandler.AppendFormatted<bool>(animate);
// //             Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear(), "EventLog");
// //         }
// //         else
// //         {
// //             DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 2);
// //             defaultInterpolatedStringHandler.AppendFormatted(__instance.Data.PlayerName);
// //             defaultInterpolatedStringHandler.AppendLiteral(" Has Appeared as Phantom, did animate: ");
// //             defaultInterpolatedStringHandler.AppendFormatted<bool>(animate);
// //             Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear(), "EventLog");
// //         }
// //     }
// // }
// // [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetRole))]
// [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetRole))]
// public static class RpcSetRoleReplacer
// {
//     public static bool doReplace = false;
//     public static Dictionary<byte, CustomRpcSender> senders;
//     public static Dictionary<PlayerControl, RoleTypes> StoragedData = [];
//     // List of Senders that do not require additional writing because SetRoleRpc has already been written by another process such as Position Desync
//     public static List<CustomRpcSender> OverriddenSenderList;
//     public static bool Prefix()
//     {
//         return !doReplace;
//     }
//     public static void Release()
//     {
//         foreach (var sender in senders)
//         {
//             if (OverriddenSenderList.Contains(sender.Value)) continue;
//             if (sender.Value.CurrentState != CustomRpcSender.State.InRootMessage)
//                 throw new InvalidOperationException("A CustomRpcSender had Invalid State.");
//
//             foreach (var (seer, roleType) in StoragedData)
//             {
//                 try
//                 {
//                     seer.SetRole(roleType, false);
//                     sender.Value.AutoStartRpc(seer.NetId, (byte)RpcCalls.SetRole, GetPlayer.GetPlayerById(sender.Key).GetClientId())
//                         .Write((ushort)roleType)
//                         .Write(false)
//                         .EndRpc();
//                 }
//                 catch
//                 { }
//             }
//             sender.Value.EndMessage();
//         }
//         doReplace = false;
//     }
//     public static void Initialize()
//     {
//         StoragedData = [];
//         OverriddenSenderList = [];
//         doReplace = true;
//     }
//     public static void StartReplace(Dictionary<byte, CustomRpcSender> senders)
//     {
//         RpcSetRoleReplacer.senders = senders;
//         doReplace = true;
//     }
// }
//
// [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
// internal class SelectRolesPatch
// {
//     public static void Prefix()
//     {
//         if (!AmongUsClient.Instance.AmHost) return;
//         
//         RpcSetRoleReplacer.Initialize();
//         
//
//         if (Toggles.AutoStartGame)
//         {
//             PlayerControl.LocalPlayer.SetRole(RoleTypes.CrewmateGhost,false);
//         }
//         // foreach (var pc in Main.AllPlayerControls)
//         // {
//         //     pc.SetRole(RoleTypes.Scientist,false);
//         //     if (Toggles.AutoStartGame && AmongUsClient.Instance.AmHost && pc.AmOwner)
//         //     {
//         //         PlayerControl.LocalPlayer.SetRole(RoleTypes.CrewmateGhost,false);
//         //     }
//         //     else
//         //     {
//         //         // RpcSetRoleReplacer.StoragedData[pc] = role;
//         //         pc.SetRole(RoleTypes.Impostor, false);
//         //     }
//         // }
//         
//     }
//     public static void Postfix()
//     {
//         if (!AmongUsClient.Instance.AmHost) return;
//
//         // Override RoleType for others players
//         foreach (var (pc, role) in RoleAssign.RoleResult)
//         {
//             if (pc == null || role.IsDesyncRole()) continue;
//
//             RpcSetRoleReplacer.StoragedData.Add(pc, role.GetRoleTypes());
//
//             Logger.Warn($"Set original role type => {pc.GetRealName()}: {role} => {role.GetRoleTypes()}", "Override Role Select");
//         }
//         //There is a delay of 0.8 seconds because after the player exits during the assign of desync roles, either a black screen will occur or the Scientist role will be set
//         _ = new LateTask(() => {
//
//             try
//             {
//                 
//                 // Set roles
//                 // SetRolesAfterSelect();
//                 RpcSetRoleReplacer.Release(); //Write RpcSetRole for all players
//                 RpcSetRoleReplacer.senders.Do(kvp => kvp.Value.SendMessage());
//
//                 // Assign tasks again
//                 ShipStatus.Instance.Begin();
//             }
//             catch (Exception ex)
//             { 
//                 YuEzTools.Logger.Error("Set Roles After Select In LateTask","fpzy");
//                 YuEzTools.Logger.Error(ex.ToString(),"fpzy");
//             }
//         }, 1f, "Set Role Types After Select");
//     }
// }
[HarmonyPatch(typeof(VoteBanSystem), nameof(VoteBanSystem.AddVote))]
internal class VoteBanSystemPatch
{
    public static bool Prefix(/*VoteBanSystem __instance, */[HarmonyArgument(0)] int srcClient, [HarmonyArgument(1)] int clientId)
    {
        // if (!AmongUsClient.Instance.AmHost) return true;

        return false;
    }
}
