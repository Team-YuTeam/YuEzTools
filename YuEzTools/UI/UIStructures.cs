using System;

namespace YuEzTools.UI;

public struct SubmenuInfo(string name, bool isExpanded, List<ToggleInfo> toggles)
{
    public string name = name;
    public bool isExpanded = isExpanded;
    public List<ToggleInfo> toggles = toggles;
}

public struct GroupInfo(string name, bool isExpanded, List<ToggleInfo> toggles, List<SubmenuInfo> submenus)
{
    public string name = name;
    public bool isExpanded = isExpanded;
    public List<ToggleInfo> toggles = toggles;
    public List<SubmenuInfo> submenus = submenus;
}

public struct ToggleInfo(string label, Func<bool> getState, Action<bool> setState)
{
    public string label = label;
    public Func<bool> getState = getState;
    public Action<bool> setState = setState;
}