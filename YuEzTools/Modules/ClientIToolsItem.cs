using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YuEzTools.Modules;

//来源：https://github.com/tukasa0001/TownOfHost/pull/1265
public class ClientToolsItem
{
    // 核心配置：每页最大按钮数（可修改）
    private const int MAX_BUTTONS_PER_PAGE = 18;
    
    // 单个按钮数据
    public bool Config;
    public ToggleButtonBehaviour ToggleButton;
    
    // 全局UI相关
    public static SpriteRenderer CustomBackground;
    private static int totalOptions = 0; // 总按钮数量
    private static int currentPage = 0; // 当前页码（从0开始）
    private static List<ClientToolsItem> allButtons = new(); // 存储所有按钮实例，用于分页控制
    private static PassiveButton prevPageBtn; // 上一页按钮
    private static PassiveButton nextPageBtn; // 下一页按钮


    private ClientToolsItem(
        string name,
        bool config,
        OptionsMenuBehaviour optionsMenuBehaviour,
        Action additionalOnClickAction = null)
    {
        try
        {
            Config = config;
            var mouseMoveToggle = optionsMenuBehaviour.DisableMouseMovement;

            // 1. 首次创建按钮时，初始化背景+换页控件
            if (CustomBackground == null)
            {
                InitBackgroundAndPageControls(optionsMenuBehaviour, mouseMoveToggle);
            }

            // 2. 创建当前按钮（位置计算不变，先正常生成）
            ToggleButton = Object.Instantiate(mouseMoveToggle, CustomBackground.transform);
            int globalIndex = allButtons.Count; // 当前按钮在全局列表的索引（还没加进去，所以用allButtons.Count）
            int pageInnerIndex = globalIndex % MAX_BUTTONS_PER_PAGE; // 关键：当前按钮在“所属页”内的索引（0~17）
            ToggleButton.transform.localPosition = new Vector3(
                pageInnerIndex % 2 == 0 ? -1.3f : 1.3f, // 左右分栏：页内索引0/2/4...左，1/3/5...右
                2.2f - (0.5f * (pageInnerIndex / 2)),   // Y坐标：按页内索引算，每页按钮都从2.2开始往下排
                -6f);
            ToggleButton.name = name;
            ToggleButton.Text.text = name;
            var passiveButton = ToggleButton.GetComponent<PassiveButton>();
            passiveButton.OnClick = new();
            passiveButton.OnClick.AddListener(new Action(() =>
            {
                Config = !Config; // 修正原逻辑：原代码修改的是局部变量，这里改为修改实例的Config
                UpdateToggle();
                additionalOnClickAction?.Invoke();
            }));

            // 3. 加入全局按钮列表，更新总数量
            allButtons.Add(this);
            totalOptions = allButtons.Count;
            UpdateToggle(); // 初始化按钮颜色
            UpdatePageButtonsVisibility(); // 生成按钮后，更新上/下一页按钮的显示状态
            ShowCurrentPageButtons(); // 生成后默认显示当前页按钮

        }
        finally 
        { 
            // 原numOptions改为totalOptions，无需单独维护
        }
    }

    // 初始化背景和分页控件（上一页/下一页按钮）
    private void InitBackgroundAndPageControls(OptionsMenuBehaviour optionsMenuBehaviour, ToggleButtonBehaviour mouseMoveToggle)
    {
        // 背景初始化（保持原有逻辑）
        CustomBackground = Object.Instantiate(optionsMenuBehaviour.Background, optionsMenuBehaviour.transform);
        CustomBackground.name = "ToolsCustomBackground";
        CustomBackground.transform.localScale = new(0.9f, 0.9f, 1f);
        CustomBackground.transform.localPosition += Vector3.back * 8;
        CustomBackground.gameObject.SetActive(false);

        // 关闭按钮（保持原有逻辑）
        var closeButton = Object.Instantiate(mouseMoveToggle, CustomBackground.transform);
        closeButton.transform.localPosition = new(1.3f, -2.3f, -6f);
        closeButton.name = "Back";
        closeButton.Text.text = GetString("Options.Back");
        closeButton.Background.color = Palette.DisabledGrey;
        var closePassiveButton = closeButton.GetComponent<PassiveButton>();
        closePassiveButton.OnClick = new();
        closePassiveButton.OnClick.AddListener(new Action(() =>
        {
            CustomBackground.gameObject.SetActive(false);
        }));

        // 新增：上一页按钮（左下角）
        prevPageBtn = CreatePageButton(
            CustomBackground.transform,
            mouseMoveToggle,
            new(-1.3f, -2.3f, -6f), // 与关闭按钮对称
            "PrevPageBtn",
            "←", // 建议在语言文件中加"上一页"的翻译
            onClick: () => 
            { 
                if (currentPage > 0) 
                { 
                    currentPage--; 
                    ShowCurrentPageButtons(); 
                } 
            }
        );

        // 新增：下一页按钮（下中间）
        nextPageBtn = CreatePageButton(
            CustomBackground.transform,
            mouseMoveToggle,
            new(-1.3f, -2.3f, -6f), // 在上一页和关闭按钮中间
            "NextPageBtn",
            "→", // 建议在语言文件中加"下一页"的翻译
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

        // 原逻辑：生成"YuETToolsOptions"入口按钮（保持不变）
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
            ShowCurrentPageButtons(); // 打开面板时默认显示当前页
        }));
    }

    // 工具方法：创建分页按钮（上一页/下一页通用）
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
        
        // 1. 先设置按钮缩放（比如缩为1:1的0.6倍，按需调整）
        float scaleRatio = 0.6f; // 缩放比例，0.6即原尺寸的60%
        btn.transform.localScale = new Vector3(scaleRatio, scaleRatio, 1f);
        
        // 2. 让文字大小随按钮缩放同步缩小（关键！）
        if (btn.Text != null)
        {
            // 原文字大小 * 缩放比例 = 适配后的文字大小
            btn.Text.fontSize = btn.Text.fontSize * scaleRatio;
            // 可选：让文字居中（避免偏移）
            btn.Text.alignment = TMPro.TextAlignmentOptions.Center;
        }
        
        var passiveBtn = btn.GetComponent<PassiveButton>();
        passiveBtn.OnClick = new();
        passiveBtn.OnClick.AddListener(new Action(onClick));
        
        // 在设置文字大小后添加
        btn.Text.rectTransform.anchorMin = new Vector2(0.5f, 0.5f); // 锚点居中
        btn.Text.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        btn.Text.rectTransform.pivot = new Vector2(0.5f, 0.5f); // 中心点居中
        btn.Text.rectTransform.localPosition = Vector3.zero; // 文字在按钮内居中
        
        return passiveBtn;
    }

    // 核心方法：显示当前页的按钮，隐藏其他页
    private void ShowCurrentPageButtons()
    {
        // 先清理列表中已失效的按钮（ToggleButton 为 null 或已销毁）
        allButtons.RemoveAll(btn => btn?.ToggleButton == null || btn.ToggleButton.gameObject == null);

        if (allButtons.Count == 0) return;
        int startIndex = currentPage * MAX_BUTTONS_PER_PAGE;
        int endIndex = Math.Min(startIndex + MAX_BUTTONS_PER_PAGE, allButtons.Count);

        for (int i = 0; i < allButtons.Count; i++)
        {
            bool isInCurrentPage = i >= startIndex && i < endIndex;
            var button = allButtons[i];
        
            // 跳过空按钮或已销毁的按钮
            if (button == null || button.ToggleButton == null || button.ToggleButton.gameObject == null)
                continue;

            if (isInCurrentPage)
            {
                int pageInnerIndex = i - startIndex;
                // 确保 transform 可用（再次空检查）
                if (button.ToggleButton.transform != null)
                {
                    button.ToggleButton.transform.localPosition = new Vector3(
                        pageInnerIndex % 2 == 0 ? -1.3f : 1.3f,
                        2.2f - (0.5f * (pageInnerIndex / 2)),
                        -6f);
                }
            }

            // 安全设置激活状态
            button.ToggleButton.gameObject.SetActive(isInCurrentPage);
        }

        UpdatePageButtonsVisibility();
    }

    // 控制上一页/下一页按钮是否显示
    private void UpdatePageButtonsVisibility()
    {
        if (prevPageBtn == null || nextPageBtn == null) return;
        
        int maxPage = GetMaxPage();
        prevPageBtn.gameObject.SetActive(currentPage > 0); // 不是第一页，显示"上一页"
        nextPageBtn.gameObject.SetActive(currentPage < maxPage); // 不是最后一页，显示"下一页"
    }

    // 计算最大页码（总页数-1，因为页码从0开始）
    private int GetMaxPage()
    {
        if (allButtons.Count == 0) return 0;
        return (allButtons.Count - 1) / MAX_BUTTONS_PER_PAGE; // 整数除法，比如19个按钮：(19-1)/18=1（0和1两页）
    }

    // 原有方法：更新按钮颜色（修正原逻辑的Config判断）
    public void UpdateToggle()
    {
        if (ToggleButton == null) return;
        // 原逻辑"Config != null"多余（因为Config是bool值，不可能为null），直接判断Config即可
        var color = Config ? Main.ModColor32 : new Color32(77, 77, 77, byte.MaxValue);
        ToggleButton.Background.color = color;
        ToggleButton.Rollover?.ChangeOutColor(color);
    }
    
    // public static void RefreshToggle(bool btnvalue, string btnstr)
    // {
    //     foreach (var button in allButtons)
    //     {
    //         Info(btnstr, "btn");
    //         if (button == null ||  button.ToggleButton.name != GetString("MenuUI."+btnstr)) continue;
    //         // 原逻辑"Config != null"多余（因为Config是bool值，不可能为null），直接判断Config即可
    //         var color = btnvalue ? Main.ModColor32 : new Color32(77, 77, 77, byte.MaxValue);
    //         button.ToggleButton.Background.color = color;
    //         button.ToggleButton.Rollover?.ChangeOutColor(color);
    //         button.Config = btnvalue; // 直接设置目标按钮的实例Config
    //         
    //         // Config = !Config; // 修正原逻辑：原代码修改的是局部变量，这里改为修改实例的Config
    //     }
    
    // }
    //
    // // 修正：更新目标按钮的实例Config，而非静态Config
    // public static void RefreshToggle(bool btnValue, string btnName)
    // {
    //     foreach (var button in allButtons)
    //     {
            // Info(btnstr, "btn");
    
    //         // 跳过空实例，或名称不匹配的按钮
    //         if (button == null || button.ToggleButton.name != btnName) continue;
    //
    //         // 只更新目标按钮的状态
    //         button.Config = btnValue; // 直接设置目标按钮的实例Config
    //         // 更新按钮颜色（复用UpdateToggle方法，避免重复代码）
    //         button.UpdateToggle(); 
    //         break; // 找到后退出循环即可，无需return（避免提前终止循环）
    //     }
    // }

    // 静态创建方法（保持原有调用方式不变）
    public static ClientToolsItem Create(
        string name,
        bool config,
        OptionsMenuBehaviour optionsMenuBehaviour,
        Action additionalOnClickAction = null)
    {
        return new(name, config, optionsMenuBehaviour, additionalOnClickAction);
    }

    // 可选：重置方法（切换场景时清理数据，避免重复生成）
    public static void Reset()
    {
        CustomBackground = null;
        totalOptions = 0;
        currentPage = 0;
        allButtons.Clear();
        prevPageBtn = null;
        nextPageBtn = null;
    }
}