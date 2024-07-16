using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using YuAntiCheat.Get;
using static YuAntiCheat.Logger;

namespace YuAntiCheat.Patches;

[HarmonyPatch(typeof(IntroCutscene))]//    [HarmonyPatch(nameof(IntroCutscene.CoBegin)), HarmonyPrefix]
class StartPatch
{    
    public static string s = "结算：";
    [HarmonyPatch(nameof(IntroCutscene.CoBegin)), HarmonyPrefix]
    public static void Prefix()
    {
        int c = 0;
        Logger.Info("== 游戏开始 ==","StartPatch");
        foreach (var pc1 in Main.AllPlayerControls)
        {
            Logger.Info("添加玩家进入CPCOS："+pc1.GetRealName(),"StartPatch");
            if(!Main.ClonePlayerControlsOnStart.Contains(pc1)) Main.ClonePlayerControlsOnStart.AddItem(pc1);
            if(Main.ClonePlayerControlsOnStart.Count() == 0)
                Info("错误，CPCOS列表空！","CPCOS in StartPatch");
            else if(Main.ClonePlayerControlsOnStart == null)
                Info("错误，CPCOS列表null！","CPCOS in StartPatch");
            else Logger.Info("成员检验"+Main.ClonePlayerControlsOnStart[c].GetRealName(),"StartPatch");
            s += "\n" + pc1.GetRealName() +" - "+ pc1.Data.Role.NiceName;
            Info(s,"StartPatch");
            c++;
        }
        Main.isFirstSendEnd = true;
        Info("设置isFirstSendEnd为"+Main.isFirstSendEnd.ToString(),"EndGamePatch");
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
// [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
// class SetEverythingUpPatch
// {
//     private static TextMeshPro roleSummary;
//     public static string s = "结算：";
//     public static void Postfix(EndGameManager __instance)
//     {
//         
//         StringBuilder sb = new($"结算：");
//         try
//         {
//             foreach (var role in Main.ClonePlayerControlsOnStart)
//             {
//                 //sb.Append($"\n　 ").Append(role.Data.Role.name);
//                 s += "\n" + role.Data.Role.name;
//             }
//         }
//         catch{Error("错误：无法导入结算职业结果","ENDPATCH");}
//
//         Info(s,"ENDPATCH");
//     }
// }
