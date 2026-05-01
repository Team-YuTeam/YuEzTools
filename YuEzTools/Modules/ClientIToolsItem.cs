using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YuEzTools.Helpers;
using YuEzTools.Utils;
using Object = UnityEngine.Object;

namespace YuEzTools.Modules;

public class ClientToolsItem
{
    private const int MAX_BUTTONS_PER_PAGE = 18;
    
    private ToggleHelper toggleHelper;
    public ToggleButtonBehaviour ToggleButton;
    
    public bool Config => toggleHelper.GetState();
    
    public static SpriteRenderer CustomBackground;
    private static int currentPage = 0;
    private static List<ClientToolsItem> allButtons = new();
    private static PassiveButton prevPageBtn;
    private static PassiveButton nextPageBtn;
    private static TextMeshPro pageText;

    private ClientToolsItem(
        string name,
        ToggleHelper toggleHelper,
        OptionsMenuBehaviour optionsMenuBehaviour)
    {
        try
        {
            this.toggleHelper = toggleHelper;
            var mouseMoveToggle = optionsMenuBehaviour.DisableMouseMovement;

            if (CustomBackground == null)
            {
                InitBackgroundAndPageControls(optionsMenuBehaviour, mouseMoveToggle);
            }

            ToggleButton = Object.Instantiate(mouseMoveToggle, CustomBackground.transform);
            int globalIndex = allButtons.Count;
            int pageInnerIndex = globalIndex % MAX_BUTTONS_PER_PAGE;
            ToggleButton.transform.localPosition = new Vector3(
                pageInnerIndex % 2 == 0 ? -1.3f : 1.3f,
                2.2f - (0.5f * (pageInnerIndex / 2)),
                -6f);
            ToggleButton.name = name;
            ToggleButton.Text.text = name;
            var passiveButton = ToggleButton.GetComponent<PassiveButton>();
            passiveButton.OnClick = new();
            passiveButton.OnClick.AddListener(new Action(() =>
            {
                toggleHelper.SetState(!toggleHelper.GetState());
                UpdateToggle();
                toggleHelper.AdditionalAction?.Invoke();
            }));

            allButtons.Add(this);
            UpdateToggle();
            UpdatePageTextAndButtons();
            ShowCurrentPageButtons();
        }
        finally
        {
        }
    }

    private static PassiveButton CloseButton;

    private void InitBackgroundAndPageControls(OptionsMenuBehaviour optionsMenuBehaviour, ToggleButtonBehaviour mouseMoveToggle)
    {
        CustomBackground = Object.Instantiate(optionsMenuBehaviour.Background, optionsMenuBehaviour.transform);
        CustomBackground.name = "ToolsCustomBackground";
        CustomBackground.transform.localScale = new(0.9f, 0.9f, 1f);
        CustomBackground.transform.localPosition += Vector3.back * 8;
        CustomBackground.gameObject.SetActive(false);

        // var closeButton = GameObject.Instantiate(mouseMoveToggle, CustomBackground.transform);
        // closeButton.gameObject.name = "CloseButton";
        // closeButton.transform.Find("Text_TMP").GetComponent<TextMeshPro>().text = "";
        // var closeTranslator = closeButton.transform.Find("Text_TMP").GetComponent<TextTranslatorTMP>();
        // if (closeTranslator != null) Object.Destroy(closeTranslator);
        // closeButton.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Utils.Utils.LoadSprite("YuEzTools.Resources.Close.png", 100f);
        // closeButton.transform.localPosition = new Vector3(1.3f, -2.8f, -6f);
        // closeButton.transform.localScale = new Vector3(0.3f, 1.2f, 0);
        // var closePassiveButton = closeButton.GetComponent<PassiveButton>();
        // closePassiveButton.OnClick = new Button.ButtonClickedEvent();
        // closePassiveButton.OnClick.AddListener((Action)(() => CustomBackground.gameObject.SetActive(false)));
        
        var template = AccountManager.Instance.transform.Find("PremissionRequestWindow");
        
        if(template != null)
        {
            CloseButton = GameObject.Instantiate(template.transform.Find("SubmitButton").GetComponent<PassiveButton>(),CustomBackground.transform);
            CloseButton.gameObject.name = "CloseButton";
            CloseButton.transform.Find("Text_TMP").GetComponent<TextMeshPro>().text = "";
            var closeTranslator = CloseButton.transform.Find("Text_TMP").GetComponent<TextTranslatorTMP>();
            if (closeTranslator != null) GameObject.Destroy(closeTranslator);
            CloseButton.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = LoadSprite("YuEzTools.Resources.Close.png", 100f);
            CloseButton.transform.localPosition = new Vector3(-2.7f,2.8f,-10f);
            CloseButton.transform.localScale = new Vector3(0.3f, 1.2f, 0);
            CloseButton.OnClick = new Button.ButtonClickedEvent();
            CloseButton.OnClick.AddListener((Action)(() => CustomBackground.gameObject.SetActive(false)));
        }
        else Error("CloseButton加载失败","ClientToolsItem");
        
        prevPageBtn = CreatePageButton(
            CustomBackground.transform,
            mouseMoveToggle,
            new(-1.3f, -2.3f, -6f),
            "PrevPageBtn",
            "←",
            onClick: () =>
            {
                if (currentPage > 0)
                {
                    currentPage--;
                    ShowCurrentPageButtons();
                }
            }
        );

        nextPageBtn = CreatePageButton(
            CustomBackground.transform,
            mouseMoveToggle,
            new(1.3f, -2.3f, -6f),
            "NextPageBtn",
            "→",
            onClick: () =>
            {
                int maxPage = GetMaxPage();
                if (currentPage < maxPage)
                {
                    currentPage++;
                    ShowCurrentPageButtons();
                }
            }
        );

        var pageTextObj = Object.Instantiate(mouseMoveToggle, CustomBackground.transform);
        pageTextObj.name = "PageText";
        pageTextObj.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        pageTextObj.transform.localPosition = new(0f, -2.3f, -6f);
        pageTextObj.Text.alignment = TextAlignmentOptions.Center;
        pageTextObj.Text.fontSize = pageTextObj.Text.fontSize * 0.6f;
        var pagePassive = pageTextObj.GetComponent<PassiveButton>();
        if (pagePassive != null) Object.Destroy(pagePassive);
        // pageTextObj.Background.color = Palette.DisabledGrey;
        pageText = pageTextObj.Text;
        Object.Destroy(pageTextObj.Background.gameObject);

        UiElement[] selectableButtons = optionsMenuBehaviour.ControllerSelectable.ToArray();
        PassiveButton leaveButton = null;
        foreach (var button in selectableButtons)
        {
            if (button == null) continue;
            if (button.name == "LeaveGameButton")
                leaveButton = button.GetComponent<PassiveButton>();
        }
        var generalTab = mouseMoveToggle.transform.parent.parent.parent;
        var modOptionsButton = Object.Instantiate(mouseMoveToggle, generalTab);
        modOptionsButton.transform.localPosition = leaveButton != null ?
            new Vector3(1.35f, -1.82f, leaveButton.transform.localPosition.z) :
            new(1.35f, -2.4f, 1f);
        modOptionsButton.name = "YuETToolsOptions";
        modOptionsButton.Text.text = GetString("YuETToolsOptions");
        modOptionsButton.Background.color = Main.ModColor32;
        var modOptionsPassiveButton = modOptionsButton.GetComponent<PassiveButton>();
        modOptionsPassiveButton.OnClick = new();
        modOptionsPassiveButton.OnClick.AddListener(new Action(() =>
        {
            CustomBackground.gameObject.SetActive(true);
            ShowCurrentPageButtons();
        }));
    }

    private PassiveButton CreatePageButton(
        Transform parent,
        ToggleButtonBehaviour prefab,
        Vector3 localPos,
        string name,
        string text,
        Action onClick)
    {
        var btn = Object.Instantiate(prefab, parent);
        btn.name = name;
        btn.transform.localPosition = localPos;
        btn.Text.text = text;
        btn.Background.color = Main.ModColor32;
        
        float scaleRatio = 0.6f;
        btn.transform.localScale = new Vector3(scaleRatio, scaleRatio, 1f);
        
        if (btn.Text != null)
        {
            btn.Text.fontSize = btn.Text.fontSize * scaleRatio;
            btn.Text.alignment = TextAlignmentOptions.Center;
        }
        
        var passiveBtn = btn.GetComponent<PassiveButton>();
        passiveBtn.OnClick = new();
        passiveBtn.OnClick.AddListener(new Action(onClick));
        
        btn.Text.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        btn.Text.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        btn.Text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        btn.Text.rectTransform.localPosition = Vector3.zero;
        
        return passiveBtn;
    }

    private void ShowCurrentPageButtons()
    {
        allButtons.RemoveAll(btn => btn?.ToggleButton == null || btn.ToggleButton.gameObject == null);

        if (allButtons.Count == 0) return;
        int startIndex = currentPage * MAX_BUTTONS_PER_PAGE;
        int endIndex = Math.Min(startIndex + MAX_BUTTONS_PER_PAGE, allButtons.Count);

        for (int i = 0; i < allButtons.Count; i++)
        {
            bool isInCurrentPage = i >= startIndex && i < endIndex;
            var button = allButtons[i];
        
            if (button == null || button.ToggleButton == null || button.ToggleButton.gameObject == null)
                continue;

            if (isInCurrentPage)
            {
                int pageInnerIndex = i - startIndex;
                if (button.ToggleButton.transform != null)
                {
                    button.ToggleButton.transform.localPosition = new Vector3(
                        pageInnerIndex % 2 == 0 ? -1.3f : 1.3f,
                        2.2f - (0.5f * (pageInnerIndex / 2)),
                        -6f);
                }
            }

            button.ToggleButton.gameObject.SetActive(isInCurrentPage);
        }

        UpdatePageTextAndButtons();
    }

    private static void UpdatePageTextAndButtons()
    {
        int maxPage = GetMaxPage();
        int totalPages = maxPage + 1;
        
        if (prevPageBtn != null)
            prevPageBtn.gameObject.SetActive(currentPage > 0);
        if (nextPageBtn != null)
            nextPageBtn.gameObject.SetActive(currentPage < maxPage);
        if (pageText != null)
            pageText.text = (currentPage + 1) + "/" + totalPages;
    }

    private static int GetMaxPage()
    {
        if (allButtons.Count == 0) return 0;
        return (allButtons.Count - 1) / MAX_BUTTONS_PER_PAGE;
    }

    public void UpdateToggle()
    {
        if (ToggleButton == null) return;
        var color = Config ? Main.ModColor32 : new Color32(77, 77, 77, byte.MaxValue);
        ToggleButton.Background.color = color;
        ToggleButton.Rollover?.ChangeOutColor(color);
    }

    public static ClientToolsItem Create(
        string name,
        ToggleHelper toggleHelper,
        OptionsMenuBehaviour optionsMenuBehaviour)
    {
        return new(name, toggleHelper, optionsMenuBehaviour);
    }

    public static void Reset()
    {
        CustomBackground = null;
        currentPage = 0;
        allButtons.Clear();
        prevPageBtn = null;
        nextPageBtn = null;
        pageText = null;
    }

    public static void RefreshAll()
    {
        for (var i = allButtons.Count - 1; i >= 0; i--)
        {
            var item = allButtons[i];
            if (item == null || item.ToggleButton == null || item.ToggleButton.gameObject == null)
            {
                allButtons.RemoveAt(i);
                continue;
            }
            item.UpdateToggle();
        }
        UpdatePageTextAndButtons();
    }
}
