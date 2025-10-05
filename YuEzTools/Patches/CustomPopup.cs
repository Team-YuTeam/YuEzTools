using System;
using TMPro;
using UnityEngine;
using YuEzTools.Helpers;
using Object = UnityEngine.Object;

namespace YuEzTools.Patches;

#nullable enable
public static class CustomPopup
{
    public static GameObject? Fill;
    public static GameObject? InfoScreen;

    public static TextMeshPro? TitleTMP;
    public static TextMeshPro? InfoTMP;

    public static PassiveButton? ActionButtonPrefab;
    public static List<PassiveButton>? ActionButtons;

    private static bool busy = false;

    /// <summary>
    /// 显示一个全屏信息显示界面
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="info">内容</param>
    /// <param name="buttons">按钮（文字，点击事件）</param>
    public static void Show(string title, string info, List<(string, Action)>? buttons)
    {
        if (busy || Fill == null || InfoScreen == null || ActionButtonPrefab == null || TitleTMP == null || InfoTMP == null)
            Init();

        if (Fill == null || InfoScreen == null || ActionButtonPrefab == null || TitleTMP == null || InfoTMP == null)
        {
            busy = false;
            return;
        }

        busy = true;

        TitleTMP.text = title;
        InfoTMP.text = info;

        if (ActionButtons != null)
        {
            foreach (var button in ActionButtons)
            {
                if (button != null && button.gameObject != null)
                    Object.Destroy(button.gameObject);
            }
        }
        ActionButtons = [];

        if (buttons != null)
        {
            foreach (var buttonInfo in buttons.Where(b => !string.IsNullOrWhiteSpace(b.Item1)))
            {
                var (text, action) = buttonInfo;
                var button = Object.Instantiate(ActionButtonPrefab, InfoScreen.transform);
                if (button == null) continue;

                var tmpTransform = button.transform.Find("Text_TMP");
                if (tmpTransform != null)
                {
                    var tmp = tmpTransform.GetComponent<TextMeshPro>();
                    tmp?.text = text;
                }

                button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                button.OnClick.AddListener((Action)(() =>
                {
                    InfoScreen?.SetActive(false);
                    Fill?.SetActive(false);
                }));

                if (action != null)
                {
                    button.OnClick.AddListener(action);
                }

                button.transform.SetLocalX(0);
                button.gameObject.SetActive(true);
                ActionButtons.Add(button);
            }
        }

        if (ActionButtons != null && ActionButtons.Count > 1)
        {
            var buttonCollider = ActionButtonPrefab.gameObject.GetComponent<BoxCollider2D>();
            if (buttonCollider != null)
            {
                float widthSum = ActionButtons.Count * buttonCollider.size.x;
                widthSum += (ActionButtons.Count - 1) * 0.1f;
                float start = -Math.Abs(widthSum / 2);
                float each = widthSum / ActionButtons.Count;
                for (int index = 0; index < ActionButtons.Count; index++)
                {
                    var button = ActionButtons[index];
                    button?.transform.SetLocalX(start + each * (index + 0.5f));
                }
            }
        }

        Fill.SetActive(true);
        InfoScreen.SetActive(true);
        busy = false;
    }

    private static (string title, string info, List<(string, Action)>? buttons)? waitToShow = null;
    public static void ShowLater(string title, string info, List<(string, Action)>? buttons) => waitToShow = (title, info, buttons);

    private static string waitToUpdateText = string.Empty;
    public static void UpdateTextLater(string info) => waitToUpdateText = info;

    public static void Update()
    {
        if (waitToShow != null)
        {
            Show(waitToShow.Value.title, waitToShow.Value.info, waitToShow.Value.buttons);
            waitToShow = null;
        }
        if (!string.IsNullOrEmpty(waitToUpdateText) && InfoTMP != null)
        {
            InfoTMP.text = waitToUpdateText;
            waitToUpdateText = string.Empty;
        }
    }

    public static void Init()
    {
        if (AccountManager.Instance == null) return;

        var DOBScreen = AccountManager.Instance.transform.Find("DOBEnterScreen");
        if (DOBScreen == null) return;

        var fillTransform = DOBScreen.Find("Fill");
        if (fillTransform != null)
        {
            Fill = Object.Instantiate(fillTransform.gameObject);
            Fill.name = "YuET Info Popup Fill";
            Fill.transform.SetLocalZ(-100f);
            Fill.SetActive(false);
        }

        var infoPageTransform = DOBScreen.Find("InfoPage");
        if (infoPageTransform != null)
        {
            InfoScreen = Object.Instantiate(infoPageTransform.gameObject);
            InfoScreen.name = "YuET Info Popup Page";
            InfoScreen.transform.SetLocalZ(-110f);
            InfoScreen.SetActive(false);
        }

        if (InfoScreen != null)
        {
            var titleTransform = InfoScreen.transform.Find("Title Text");
            if (titleTransform != null)
            {
                TitleTMP = titleTransform.GetComponent<TextMeshPro>();
                if (TitleTMP != null)
                {
                    TitleTMP.transform.localPosition = new Vector3(0f, 2.3f, 3f);
                    TitleTMP.DestroyTranslatorL();
                    TitleTMP.text = "";
                }
            }

            var infoTextTransform = InfoScreen.transform.Find("InfoText_TMP");
            if (infoTextTransform != null)
            {
                InfoTMP = infoTextTransform.GetComponent<TextMeshPro>();
                if (InfoTMP != null)
                {
                    var rectTransform = InfoTMP.GetComponent<RectTransform>();
                    rectTransform?.sizeDelta = new Vector2(7f, 1.3f);

                    InfoTMP.transform.localScale = Vector3.one;
                    InfoTMP.DestroyTranslatorL();
                    InfoTMP.text = "";
                }
            }

            var backButtonTransform = InfoScreen.transform.Find("BackButton");
            if (backButtonTransform != null)
            {
                ActionButtonPrefab = backButtonTransform.GetComponent<PassiveButton>();
                if (ActionButtonPrefab != null)
                {
                    ActionButtonPrefab.gameObject.name = "ActionButtonPrefab";
                    ActionButtonPrefab.transform.localScale = new Vector3(0.66f, 0.66f, 0.66f);
                    ActionButtonPrefab.transform.localPosition = new Vector3(0f, -1.35f, 3f);
                    ActionButtonPrefab.gameObject.SetActive(false);

                    var textTransform = ActionButtonPrefab.transform.Find("Text_TMP");
                    if (textTransform != null)
                    {
                        var textTMP = textTransform.GetComponent<TextMeshPro>();
                        textTMP?.DestroyTranslatorL();
                    }
                }
            }
        }
    }
}