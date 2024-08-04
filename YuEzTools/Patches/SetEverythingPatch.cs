using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using YuEzTools.Modules;
using YuEzTools.Patches;
using YuEzTools.UI;
using UnityEngine;
using static YuEzTools.Translator;
using YuEzTools.Templates;
using YuEzTools.Get;
using static YuEzTools.Logger;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(IntroCutscene))]//    [HarmonyPatch(nameof(IntroCutscene.CoBegin)), HarmonyPrefix]
class StartPatch
{    
    public static string s = GetString("EndMessage");
    public static string sc = GetString("EndMessageC");
    private static string r, b, g;
    [HarmonyPatch(nameof(IntroCutscene.CoBegin)), HarmonyPrefix]
    public static void Prefix()
    {
        GetPlayer.numImpostors = 0;
        GetPlayer.numCrewmates = 0;
        s = GetString("EndMessage");
        sc = GetString("EndMessageC");
        int c = 0;
        Logger.Info("== 游戏开始 ==","StartPatch");
        foreach (var pc1 in Main.AllPlayerControls)
        {
            //Logger.Info("添加玩家进入CPCOS："+pc1.GetRealName(),"StartPatch");
            if(!Main.ClonePlayerControlsOnStart.Contains(pc1)) Main.ClonePlayerControlsOnStart.AddItem(pc1);
            if(Main.ClonePlayerControlsOnStart.Count() == 0)
                Info("错误，CPCOS列表空！","CPCOS in StartPatch");
            else if(Main.ClonePlayerControlsOnStart == null)
                Info("错误，CPCOS列表null！","CPCOS in StartPatch");
            else Logger.Info("成员检验"+Main.ClonePlayerControlsOnStart[c].GetRealName(),"StartPatch");
            r = Convert.ToString((int)pc1.Data.Color.r,16);
            b = Convert.ToString((int)pc1.Data.Color.b,16);
            g = Convert.ToString((int)pc1.Data.Color.g,16);
            
            s += "\n" + pc1.GetRealName() +" - "+ pc1.Data.Role.NiceName;
            sc += "\n" +$"{pc1.GetRealName()}{pc1.Data.ColorName}" +" - "+ GetPlayer.GetColorRole(pc1);
            
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
}

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
class EndGamePatch
{
    public static Dictionary<byte, string> SummaryText = new();
    
    public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
    {
        Logger.Info("== 游戏结束 ==","EndGamePatch");
        Logger.Info("结束原因：" + endGameResult.GameOverReason.ToString(), "EndGamePatch");
        Main.isFirstSendEnd = true;
        Info("设置isFirstSendEnd为"+Main.isFirstSendEnd.ToString(),"EndGamePatch");
    }
}
[HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
class SetEverythingUpPatch
{
    private static TextMeshPro roleSummary;
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

        var RoleSummaryRectTransform = RoleSummary.GetComponent<RectTransform>();
        RoleSummaryRectTransform.anchoredPosition = new Vector2(Pos.x + 3.5f, Pos.y - 0.1f);
        RoleSummary.text = StartPatch.sc;
        
        Info(StartPatch.s,"ENDPATCH");
    }
}
