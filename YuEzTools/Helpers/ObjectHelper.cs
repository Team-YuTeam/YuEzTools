using TMPro;
using UnityEngine;

namespace YuEzTools.Helpers;

public static class ObjectHelper
{
    /// <summary>
    /// オブジェクトの<see cref="TextTranslatorTMP"/>コンポーネントを破棄します
    /// </summary>
    public static void DestroyTranslator(this GameObject obj)
    {
        if (obj == null) return;
        obj.ForEachChild((Il2CppSystem.Action<GameObject>)DestroyTranslator);
        TextTranslatorTMP[] translator = obj.GetComponentsInChildren<TextTranslatorTMP>(true);
        translator?.Do(Object.Destroy);
    }
    /// <summary>
    /// オブジェクトの<see cref="TextTranslatorTMP"/>コンポーネントを破棄します
    /// </summary>
    public static void DestroyTranslatorL(this MonoBehaviour obj) => obj?.gameObject?.DestroyTranslator();
    
    /// <summary>
    /// 加载一个文字TMP，来源FS
    /// </summary>
    public static TextMeshPro InstantiateTextComponent(TextMeshPro template, Vector3 position, Transform parent = null)
    {
        var text = Object.Instantiate(template, parent);
        text.transform.localPosition = position;
        text.fontStyle = FontStyles.Bold;
        text.text = string.Empty;
        return text;
    }
}