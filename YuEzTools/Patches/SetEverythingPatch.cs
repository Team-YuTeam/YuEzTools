using System;
using AmongUs.GameOptions;
using TMPro;
using YuEzTools.Modules;
using UnityEngine;
using YuEzTools.Attributes;
using YuEzTools.Utils;
using YuEzTools.UI;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGame))]
internal class CoStartGamePatch
{
    public static void Postfix()
    {
        GameModuleInitializerAttribute.InitializeAll();
        // if (AmongUsClient.Instance.AmHost && Main.HasHacker)
        // {
        //     Logger.Info("Host Try end game with room " +
        //                 GameStartManager.Instance.GameRoomNameCode.text,"StartPatch");
        //     try
        //     {
        //         GameManager.Instance.RpcEndGame(GameOverReason.ImpostorDisconnect, false);
        //     }
        //     catch (Exception e)
        //     {
        //         Logger.Error(e.ToString(), "StartPatch");
        //     }
        //     Main.HasHacker = false;
        // }
        // Main.HasHacker = false;
    }
}

[HarmonyPatch(typeof(IntroCutscene))]
class StartPatch
{
    [HarmonyPatch(nameof(IntroCutscene.CoBegin)), HarmonyPrefix]
    public static void Prefix()
    {
        GetPlayer.numImpostors = 0;
        GetPlayer.numCrewmates = 0;
        int c = 0;
        Info("== 游戏开始 ==", "StartPatch");
        foreach (var pc1 in Main.AllPlayerControls)
        {
            //Logger.Info("添加玩家进入CPCOS："+pc1.GetRealName(),"StartPatch");
            if (!Main.ClonePlayerControlsOnStart.Contains(pc1)) Main.ClonePlayerControlsOnStart.Add(pc1);

            Info("成员检验" + Main.ClonePlayerControlsOnStart[c].GetRealName(), "StartPatch");

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
        Info("设置isFirstSendEnd为" + Main.isFirstSendEnd.ToString(), "StartPatch");
    }
}

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
class EndGamePatch
{
    public static Dictionary<byte, string> SummaryText = new();
    public static string WinReason = "";
    public static string WinTeam = "";
    public static void Postfix([HarmonyArgument(0)] ref EndGameResult endGameResult)
    {
        Info("== 游戏结束 ==", "EndGamePatch");
        Info("结束原因：" + endGameResult.GameOverReason.ToString(), "EndGamePatch");
        SummaryText = [];
        foreach (var id in ModPlayerData.AllPlayerDataForMod.Keys)
            SummaryText[id] = Utils.Utils.SummaryTexts(id);
        WinReason = endGameResult.GameOverReason.ToString();
        WinTeam = endGameResult.GameOverReason.GetWinTeam();
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

// Thanks Nebula on the Ship
public static class DetailDialog
{
    static EndGameManager endGameManager;
    static GameObject dialog;
    static TMP_Text saveText;
    static TMP_Text[] text;
    static PassiveButton button;
    // static PassiveButton saveButton;
    static SpriteRenderer renderer;

    static public void Initialize(EndGameManager endGameManager, ControllerDisconnectHandler handler, TMP_Text textTemplate, string[] detail)
    {
        DetailDialog.endGameManager = endGameManager;

        handler.enabled = false;
        handler.name = "DetailDialog";
        handler.gameObject.SetActive(false);
        handler.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        dialog = handler.gameObject;
        renderer = dialog.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        button = dialog.transform.GetChild(2).gameObject.GetComponent<PassiveButton>();
        // saveButton = dialog.transform.GetChild(2).gameObject.GetComponent<PassiveButton>();
        saveText = dialog.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();

        renderer.transform.localScale = new Vector3(1.6f, 0.85f, 1.0f);

        button.transform.localPosition = new Vector3(0f, -1.95f, 0f);
        button.transform.GetChild(1).GetComponent<TextMeshPro>().text = GetString("game.endScreen.close");
        button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        button.OnClick.AddListener((Action)Close);

        saveText.transform.localPosition = new Vector3(3.45f, -2.3f, 5f);
        saveText.alignment = TextAlignmentOptions.TopLeft;
        saveText.color = Color.white;
        saveText.fontSizeMin = 1.25f;
        saveText.fontSizeMax = 1.25f;
        saveText.fontSize = 1.25f;
        saveText.text = "";

        text = new TMP_Text[detail.Length];
        float width = 0.0f;
        for (int i = 0; i < detail.Length; i++)
        {
            text[i] = UnityEngine.Object.Instantiate(textTemplate);
            text[i].transform.SetParent(dialog.transform);
            text[i].transform.localScale = new Vector3(1f, 1f, 1f);
            text[i].transform.localPosition = new Vector3(width, 2.1f, 0f);
            text[i].alignment = TextAlignmentOptions.TopLeft;
            text[i].color = Color.white;
            text[i].fontSizeMin = 1.5f;
            text[i].fontSizeMax = 1.5f;
            text[i].fontSize = 1.5f;
            text[i].text = detail[i];

            text[i].gameObject.SetActive(true);

            RectTransform rectTransform = text[i].gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(0f, 0f);

            width += text[i].preferredWidth - 0.05f;
        }

        //中央に移動させる
        for (int i = 0; i < detail.Length; i++)
        {
            text[i].transform.localPosition -= new Vector3(width / 2.0f, 0f, 0f);
        }

        renderer.gameObject.SetActive(true);
        button.gameObject.SetActive(true);
        // saveButton.gameObject.SetActive(true);
        saveText.gameObject.SetActive(true);
    }

    static public void Open()
    {
        dialog.SetActive(true);
        dialog.transform.localScale = new Vector3(0.0f, 0.0f, 1.0f);
        endGameManager.StartCoroutine(Effects.Lerp(0.12f, new Action<float>((p) =>
        {
            dialog.transform.localScale = new Vector3(p, p, 1.0f);
        })));
    }

    static public void Close()
    {
        endGameManager.StartCoroutine(Effects.Lerp(0.12f, new Action<float>((p) =>
        {
            dialog.transform.localScale = new Vector3(1.0f - p, 1.0f - p, 1.0f);
            if (p == 1f) dialog.SetActive(false);
        })));
    }
}

[HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
class SetEverythingUpPatch
{
    public static string s = "";
    public static void Postfix(EndGameManager __instance)
    {
        s = "";
        var BackgroundLayer = GameObject.Find("PoolablePlayer(Clone)");
        __instance.WinText.text = Toggles.WinTextSize ?
            $"<size=50%>{GetString(EndGamePatch.WinTeam)}\n<size=30%>{GetString(EndGamePatch.WinReason)}</size>" :
            $"<size=50%>{GetString(EndGamePatch.WinReason)}\n<size=30%>{GetString(EndGamePatch.WinTeam)}</size>";
        if (EndGamePatch.WinTeam == "NobodyWin")
        {
            Info("进入NobodyWin", "SetEverythingUpPatch");
            BackgroundLayer.SetActive(false);
        }
        var ModDisplay = new GameObject("ModDisplay");
        var position = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height));
        TMP_Text[] roleSummaryText = new TMP_Text[5];
        for (int i = 0; i < roleSummaryText.Length; i++)
        {
            GameObject obj = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
            obj.transform.SetParent(ModDisplay.transform);

            RectTransform roleSummaryTextMeshRectTransform = obj.GetComponent<RectTransform>();
            roleSummaryTextMeshRectTransform.pivot = new Vector2(0f, 1f);
            roleSummaryTextMeshRectTransform.anchoredPosition = new Vector3(position.x, position.y - 0.1f, -14f);
            obj.transform.localScale = new Vector3(1f, 1f, 1f);

            roleSummaryText[i] = obj.GetComponent<TMP_Text>();
            roleSummaryText[i].alignment = TextAlignmentOptions.TopLeft;
            roleSummaryText[i].color = Color.white;
            roleSummaryText[i].fontSizeMin = 1.25f;
            roleSummaryText[i].fontSizeMax = 1.25f;
            roleSummaryText[i].fontSize = 1.25f;
        }

        foreach (var kvp in ModPlayerData.AllPlayerDataForMod)
        {
            var id = kvp.Key;
            var data = kvp.Value;
            s += $"\n" + EndGamePatch.SummaryText[id];
        }

        Info(s, "SetEverythingUpPatch");
        //唤出结算按钮
        var detailButton = UnityEngine.Object.Instantiate(__instance.Navigation.ContinueButton.transform.GetChild(0));
        detailButton.transform.SetParent(ModDisplay.transform);
        detailButton.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        detailButton.localPosition = roleSummaryText[0].transform.localPosition + new Vector3(1.0f, -roleSummaryText[0].preferredHeight - 0.5f);
        PassiveButton detailPassiveButton = detailButton.GetComponent<PassiveButton>();
        detailPassiveButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        detailPassiveButton.OnClick.AddListener((UnityEngine.Events.UnityAction)(() => DetailDialog.Open()));
        TMP_Text detailButtonText = detailButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        detailButtonText.text = GetString("game.endScreen.detail");
        detailButtonText.gameObject.GetComponent<TextTranslatorTMP>().enabled = false;

        //结算界面
        var detailDialog = UnityEngine.Object.Instantiate(UnityEngine.Object.FindObjectOfType<ControllerDisconnectHandler>(), null);
        DetailDialog.Initialize(__instance, detailDialog, __instance.WinText, [GetString("EndMessage")+ s]);
        Info(s, "EndSummary");
    }
}