using System.Collections;
using Il2CppInterop.Runtime;
using TMPro;
using Twitch;
using UnityEngine;

namespace YuEzTools;

public class VanillaAsset
{
    static public Sprite PopUpBackSprite { get; private set; } = null!;
    static public Sprite FullScreenSprite { get; private set; } = null!;
    static public Sprite TextButtonSprite { get; private set; } = null!;
    static public Sprite CloseButtonSprite { get; private set; } = null!;
    static public TMPro.TextMeshPro StandardTextPrefab { get; private set; } = null!;
    static public AudioClip HoverClip { get; private set; } = null!;
    static public AudioClip SelectClip { get; private set; } = null!;
    static public Material StandardMaskedFontMaterial { get {
            if (standardMaskedFontMaterial == null) standardMaskedFontMaterial = UnityHelper.FindAsset<Material>("LiberationSans SDF - BlackOutlineMasked")!;
            return standardMaskedFontMaterial!;
        }
    }
    static public Material OblongMaskedFontMaterial { get { 
            if(oblongMaskedFontMaterial == null) oblongMaskedFontMaterial = UnityHelper.FindAsset<Material>("Brook Atlas Material Masked");
            return oblongMaskedFontMaterial!;
        } }
    
    static private Material? standardMaskedFontMaterial = null;
    static private Material? oblongMaskedFontMaterial = null;

    static private TMP_FontAsset? versionFont = null;
    static public TMP_FontAsset VersionFont
    {
        get
        {
            if (versionFont == null) versionFont = UnityHelper.FindAsset<TMP_FontAsset>("Barlow-Medium SDF");
            return versionFont!;
        }
    }

    static private TMP_FontAsset? preSpawnFont = null;
    static public TMP_FontAsset PreSpawnFont { get
        {
            if(preSpawnFont==null) preSpawnFont = UnityHelper.FindAsset<TMP_FontAsset>("DIN_Pro_Bold_700 SDF")!;
            return preSpawnFont;
        }
    }

    static private TMP_FontAsset? brookFont = null;
    static public TMP_FontAsset BrookFont
    {
        get
        {
            if (brookFont == null) brookFont = UnityHelper.FindAsset<TMP_FontAsset>("Brook SDF")!;
            return brookFont;
        }
    }

    static public PlayerCustomizationMenu PlayerOptionsMenuPrefab { get; private set; } = null!;

    static public ShipStatus[] MapAsset = new ShipStatus[6];
    static public Vector2 GetMapCenter(byte mapId) => MapAsset[mapId].MapPrefab.transform.GetChild(5).localPosition;
    static public float GetMapScale(byte mapId) => VanillaAsset.MapAsset[mapId].MapScale;
    static public Vector2 ConvertToMinimapPos(Vector2 pos,Vector2 center, float scale)=> (pos / scale) + center;
    static public Vector2 ConvertToMinimapPos(Vector2 pos, byte mapId) => ConvertToMinimapPos(pos, GetMapCenter(mapId), GetMapScale(mapId));
    static public Vector2 ConvertFromMinimapPosToWorld(Vector2 minimapPos, Vector2 center, float scale) => (minimapPos - center) * scale;
    static public Vector2 ConvertFromMinimapPosToWorld(Vector2 minimapPos, byte mapId) => ConvertFromMinimapPosToWorld(minimapPos, GetMapCenter(mapId), GetMapScale(mapId));
    

    public static void PlaySelectSE() => SoundManager.Instance.PlaySound(SelectClip, false, 0.8f);
    public static void PlayHoverSE() => SoundManager.Instance.PlaySound(HoverClip, false, 0.8f);
}
