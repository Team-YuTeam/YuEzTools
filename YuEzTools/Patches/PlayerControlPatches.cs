using HarmonyLib;
using TMPro;
using UnityEngine;
using YuEzTools.Get;
using YuEzTools.Modules;
using static YuEzTools.Translator;
using YuEzTools.Get;
using YuEzTools.Utils;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
class MurderPlayerPatch
{
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

        // var color ="#ffffff";
        var name = "";
        var addText = __instance.cosmetics.nameText.transform.Find("AddText").GetComponent<TextMeshPro>();

        try
        {
            if (GetPlayer.IsLobby)
            {
                if (__instance.FriendCode.IsDevUser())
                    name += __instance.FriendCode.GetDevUser().GetTag();
               
                if (Toggles.ShowInfoInLobby)
                {
                    name += $"<size=70%><color=#33EEFF>Lv.{__instance.GetClient().PlayerLevel} {__instance.GetClient().PlatformData.Platform.GetPlatformText()} {__instance.GetClient().Id}</color></size>\n" +
                           $"<size=65%><color=#33FF91>{__instance.PlayerId} {__instance.GetClient().FriendCode} {__instance.GetClient().GetHashedPuid()}</color></size>";
                }
            }

            if (GetPlayer.IsInGame)
            {
                name = "";
                if (__instance == PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Data.IsDead)
                {
                    name += Utils.Utils.ColorString(Utils.Utils.GetRoleColor32(__instance.Data.RoleType), __instance.GetRoleName()  + "\n");
                    __instance.cosmetics.nameText.text = Utils.Utils.ColorString(Utils.Utils.GetRoleColor32(__instance.Data.RoleType), __instance.cosmetics.nameText.text);
                    name += __instance.PlayerId.GetKillOrTaskCountText();
                }
            
                if (PlayerControl.LocalPlayer.Data.IsDead && __instance.Data.IsDead)
                    name += $"\n{Utils.Utils.GetDeadText(__instance)}";
            }

            if (Main.HackerList.Contains(__instance))
            {
                __instance.cosmetics.nameText.text += $"<color=#3FBAFF>[{GetString("Hacker")}]</color>";
            }

            if(name != "" || GetPlayer.IsInGame)
            {
                addText.text = name;
                addText.gameObject.SetActive(true);
            }
        }
        catch{}
        

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