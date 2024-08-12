using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmongUs.GameOptions;
using TMPro;
using YuEzTools.Modules;
using YuEzTools.Patches;
using YuEzTools.UI;
using UnityEngine;
using YuEzTools.Attributes;
using static YuEzTools.Translator;
using YuEzTools.Templates;
using YuEzTools.Get;
using static YuEzTools.Logger;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGame))]
internal class CoStartGamePatch
{
    public static void Postfix()
    {
        GameModuleInitializerAttribute.InitializeAll();
        if (AmongUsClient.Instance.AmHost && Main.HasHacker)
        {
            Logger.Info("Host Try end game with room " +
                        GameStartManager.Instance.GameRoomNameCode.text,"StartPatch");
            try
            {
                GameManager.Instance.RpcEndGame(GameOverReason.ImpostorDisconnect, false);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString(), "StartPatch");
            }
            Main.HasHacker = false;
        }
    }
}

[HarmonyPatch(typeof(IntroCutscene))]//    [HarmonyPatch(nameof(IntroCutscene.CoBegin)), HarmonyPrefix]
class StartPatch
{    
    [HarmonyPatch(nameof(IntroCutscene.CoBegin)), HarmonyPrefix]
    public static void Prefix()
    {
        GetPlayer.numImpostors = 0;
        GetPlayer.numCrewmates = 0;
        int c = 0;
        Logger.Info("== 游戏开始 ==","StartPatch");
        foreach (var pc1 in Main.AllPlayerControls)
        {
            //Logger.Info("添加玩家进入CPCOS："+pc1.GetRealName(),"StartPatch");
            if(!Main.ClonePlayerControlsOnStart.Contains(pc1)) Main.ClonePlayerControlsOnStart.Add(pc1);
            
            Info("成员检验"+Main.ClonePlayerControlsOnStart[c].GetRealName(),"StartPatch");
            
            //结算格式："\n" +$"{Utils.Utils.ColorString(pc1.Data.Color,pc1.GetRealName())}" +" - "+ GetPlayer.GetColorRole(pc1);
            
            
            if (pc1.Data.Role.IsImpostor)
            {
                GetPlayer.numImpostors++;
            }
            else
            {
                GetPlayer.numCrewmates++;
            }
            
            //Info(s,"StartPatch");
            c++;
        }
        Main.isFirstSendEnd = true;
        Info("设置isFirstSendEnd为"+Main.isFirstSendEnd.ToString(),"StartPatch");
    }
    [HarmonyPatch(nameof(IntroCutscene.CoBegin)), HarmonyPostfix]
    public static void Postfix()
    {
        
    }
}

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
class EndGamePatch
{
    public static Dictionary<byte, string> SummaryText = new();
    public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
    {
        Logger.Info("== 游戏结束 ==","EndGamePatch");
        Logger.Info("结束原因：" + endGameResult.GameOverReason.ToString(), "EndGamePatch");
        SummaryText = new();
        foreach (var id in ModPlayerData.AllPlayerDataForMod.Keys)
            SummaryText[id] = Utils.Utils.SummaryTexts(id);
        Main.isFirstSendEnd = true;
    }
}
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CoSetRole))]
class CoSetRolePatch
{
    public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] RoleTypes roleTypes)
    {
        __instance.SetRole(roleTypes);
    }
}
[HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
class SetEverythingUpPatch
{
    private static TextMeshPro roleSummary;
    public static string s = "";
    public static void Postfix(EndGameManager __instance)
    {
        var Pos = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
        var RoleSummaryObject = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
        RoleSummaryObject.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f, Pos.y - 0.1f, -15f);
        RoleSummaryObject.transform.localScale = new Vector3(1f, 1f, 1f);
        
        var RoleSummary = RoleSummaryObject.GetComponent<TextMeshPro>();
        RoleSummary.alignment = TextAlignmentOptions.TopLeft;
        RoleSummary.color = Color.white;
        RoleSummary.outlineWidth *= 1.2f;
        RoleSummary.fontSizeMin = RoleSummary.fontSizeMax = RoleSummary.fontSize = 1.25f;
        
        foreach (var kvp in ModPlayerData.AllPlayerDataForMod)
        {
            var id = kvp.Key;
            var data = kvp.Value;
            s += $"\n" + EndGamePatch.SummaryText[id];
        }
        
        var RoleSummaryRectTransform = RoleSummary.GetComponent<RectTransform>();
        RoleSummaryRectTransform.anchoredPosition = new Vector2(Pos.x + 3.5f, Pos.y - 0.1f);
        RoleSummary.text = GetString("EndMessage");
        RoleSummary.text += s;
    }
}
