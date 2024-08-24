using UnityEngine;

namespace YuEzTools;

public class LayerExpansion
{
    static int? uiLayer = null;
    static public int GetUILayer()
    {
        if (uiLayer == null) uiLayer = LayerMask.NameToLayer("UI");
        return uiLayer.Value;
    }
}