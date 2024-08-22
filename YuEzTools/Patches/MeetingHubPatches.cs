// using HarmonyLib;
// using UnityEngine;
// using UnityEngine.ProBuilder;
// using YuEzTools.Modules;
// using YuEzTools.Utils;
//
// namespace YuEzTools.Patches;
//
// [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start)),HarmonyPriority(Priority.First)]
// public static class StartMeetingHubPatch
// {
//     public static void Postfix(MeetingHud __instance)
//     { 
//         Logger.Msg("------会议启动-------","StartMeetingHubPatch");
//         foreach (var ps in __instance.playerStates)
//         {
//             var AddText = GameObject.Instantiate(ps.NameText);
//             AddText.text = "";
//             AddText.enabled = false;
//             AddText.transform.SetParent(ps.NameText.transform);
//             AddText.transform.localPosition = new Vector3(0f, -0.18f, 0f);
//             AddText.fontSize = 1.5f;
//             AddText.gameObject.name = "AddText";
//             AddText.enableWordWrapping = false;
//
//             var name = "";
//             var color ="#ffffff";
//             var pc = ps.TargetPlayerId.GetPlayerDataById().pc;
//             if (pc == PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Data.IsDead)
//             {
//                 name = Utils.Utils.ColorString(Utils.Utils.GetRoleColor32(pc.Data.RoleType), pc.GetRoleName());
//                 ps.NameText.text = Utils.Utils.ColorString(Utils.Utils.GetRoleColor32(pc.Data.RoleType), pc.cosmetics.nameText.text);
//                 name += " ";
//                 name += pc.PlayerId.GetKillOrTaskCountText();
//             }
//             
//             if (PlayerControl.LocalPlayer.Data.IsDead && pc.Data.IsDead)
//                 name += $" {Utils.Utils.GetDeadText(pc)}";
//             
//             if (name != "")
//             {
//                 ps.ColorBlindName.transform.localPosition -= new Vector3(1.35f, 0f, 0f);
//                 AddText.text = name;
//                 AddText.SetActive(true);
//             }
//         }
//     }
// }