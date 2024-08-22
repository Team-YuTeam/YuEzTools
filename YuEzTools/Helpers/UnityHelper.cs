using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Cpp2IL.Core.Extensions;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using UnityEngine.UI;

namespace YuEzTools;

public static class UnityHelper
{

    public static GameObject CreateObject(string objName, Transform? parent, Vector3 localPosition,int? layer = null)
    {
        var obj = new GameObject(objName);
        obj.transform.SetParent(parent);
        obj.transform.localPosition = localPosition;
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        if (layer.HasValue) obj.layer = layer.Value;
        else if (parent != null) obj.layer = parent.gameObject.layer;
        return obj;
    }

    public static T CreateObject<T>(string objName, Transform? parent, Vector3 localPosition,int? layer = null) where T : Component
    {
        return CreateObject(objName, parent, localPosition, layer).AddComponent<T>();
    }

    //SortingGroupOrderを10に固定します。
    public static SpriteRenderer CreateSpriteRenderer(string objName, Transform? parent, Vector3 localPosition, int? layer = null)
    {
        var renderer = CreateObject<SpriteRenderer>(objName, parent, localPosition, layer);
        renderer.sortingGroupOrder = 10;
        return renderer;
    }

    public static (MeshRenderer renderer, MeshFilter filter) CreateMeshRenderer(string objName, Transform? parent, Vector3 localPosition, int? layer,Color? color = null)
    {
        var meshFilter = UnityHelper.CreateObject<MeshFilter>("mesh", parent, localPosition, layer);
        var meshRenderer = meshFilter.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find(color.HasValue ? "Unlit/Color" : "Unlit/Texture"));
        if(color.HasValue) meshRenderer.sharedMaterial.color = color.Value;
        meshFilter.mesh = new Mesh();

        return (meshRenderer, meshFilter);
    }

    public static Camera CreateRenderingCamera(string objName, Transform? parent, Vector3 localPosition, float halfYSize, int layerMask = 31511) {
        var camera = UnityHelper.CreateObject<Camera>(objName, parent, localPosition);
        camera.backgroundColor = Color.black;
        camera.allowHDR = false;
        camera.allowMSAA = false;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.depth = 5;
        camera.nearClipPlane = -1000f;
        camera.orthographic = true;
        camera.orthographicSize = halfYSize;
        camera.cullingMask = layerMask;
        return camera;
    }

    public static RenderTexture SetCameraRenderTexture(this Camera camera, int textureX, int textureY)
    {
        if (camera.targetTexture) GameObject.Destroy(camera.targetTexture);
        camera.targetTexture = new RenderTexture(textureX, textureY, 32, RenderTextureFormat.ARGB32);

        return camera.targetTexture;
    }

    public static MeshFilter CreateRectMesh(this MeshFilter filter, Vector2 size, Vector3? center = null)
    {
        center ??= Vector3.zero;

        var mesh = filter.mesh;

        float x = size.x * 0.5f;
        float y = size.y * 0.5f;
        mesh.SetVertices((Vector3[])[
            new Vector3(-x , -y) + center.Value,
            new Vector3(x, -y) + center.Value,
            new Vector3(-x, y) + center.Value,
            new Vector3(x, y) + center.Value]);
        mesh.SetTriangles((int[])[0, 2, 1, 2, 3, 1], 0);
        mesh.SetUVs(0, (Vector2[])[new(0, 0), new(1, 0), new(0, 1), new(1, 1)]);
        var color = new Color32(255, 255, 255, 255);
        mesh.SetColors((Color32[])[color, color, color, color]);

        return filter;
    }

    public static LineRenderer SetUpLineRenderer(string objName,Transform? parent,Vector3 localPosition,int? layer = null,float width = 0.2f)
    {
        var line = UnityHelper.CreateObject<LineRenderer>(objName, parent, localPosition, layer);
        line.material.shader = Shader.Find("Sprites/Default");
        line.SetColors(Color.clear, Color.clear);
        line.positionCount = 2;
        line.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
        line.useWorldSpace = false;
        line.SetWidth(width, width);
        return line;
    }

    public static T? FindAsset<T>(string name) where T : Il2CppObjectBase
    {
        foreach (var asset in UnityEngine.Object.FindObjectsOfTypeIncludingAssets(Il2CppType.Of<T>()))
        {
            if (asset.name == name) return asset.Cast<T>();
        }
        return null;
    }

    public static T MarkDontUnload<T>(this T obj) where T : UnityEngine.Object
    {
        GameObject.DontDestroyOnLoad(obj);
        obj.hideFlags |= HideFlags.DontUnloadUnusedAsset | HideFlags.HideAndDontSave;

        return obj;
    }

    public static Camera? FindCamera(int cameraLayer) => Camera.allCameras.FirstOrDefault(c => (c.cullingMask & (1 << cameraLayer)) != 0);

    public static Vector3 ScreenToWorldPoint(Vector3 screenPos, int cameraLayer)
    {
        return FindCamera(cameraLayer)?.ScreenToWorldPoint(screenPos) ?? Vector3.zero;
    }

    public static Vector3 WorldToScreenPoint(Vector3 worldPos, int cameraLayer)
    {
        return FindCamera(cameraLayer)?.WorldToScreenPoint(worldPos) ?? Vector3.zero;
    }


    public static PassiveButton SetUpButton(this GameObject gameObject, bool withSound = false, SpriteRenderer? buttonRenderer = null, Color? defaultColor = null, Color? selectedColor = null)
        => SetUpButton(gameObject, withSound, buttonRenderer != null ? [buttonRenderer] : [], defaultColor, selectedColor);

    public static PassiveButton SetUpButton(this GameObject gameObject, bool withSound, SpriteRenderer[] buttonRenderers, Color? defaultColor = null, Color? selectedColor = null) {
        var button = gameObject.AddComponent<PassiveButton>();
        button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        button.OnMouseOut = new UnityEngine.Events.UnityEvent();
        button.OnMouseOver = new UnityEngine.Events.UnityEvent();

        if (withSound)
        {
            button.OnClick.AddListener(VanillaAsset.PlaySelectSE);
            button.OnMouseOver.AddListener(VanillaAsset.PlayHoverSE);
        }
        if (buttonRenderers.Length > 0)
        {
            button.OnMouseOut.AddListener(() => { foreach (var r in buttonRenderers) r.color = defaultColor ?? Color.white; });
            button.OnMouseOver.AddListener(() => { foreach (var r in buttonRenderers) r.color = selectedColor ?? Color.green; });
        }

        if (buttonRenderers.Length > 0) foreach(var r in buttonRenderers)r.color = defaultColor ?? Color.white;
        
        return button;
    }

    static public void AddListener(this UnityEngine.UI.Button.ButtonClickedEvent onClick, Action action) => onClick.AddListener((UnityEngine.Events.UnityAction)action);
    static public void AddListener(this UnityEngine.Events.UnityEvent unityEvent, Action action) => unityEvent.AddListener((UnityEngine.Events.UnityAction)action);

    public static void SetModText(this TextTranslatorTMP text,string translationKey)
    {
        text.TargetText = (StringNames)short.MaxValue;
        text.defaultStr = translationKey;
    }

    static public void DoTransitionFade(this TransitionFade transitionFade, GameObject? transitionFrom, GameObject? transitionTo, Action onTransition, Action callback)
    {
        if (transitionTo) transitionTo!.SetActive(false);

        IEnumerator Coroutine()
        {
            yield return Effects.ColorFade(transitionFade.overlay, Color.clear, Color.black, 0.1f);
            if (transitionFrom && transitionFrom!.gameObject) transitionFrom.gameObject.SetActive(false);
            if (transitionTo && transitionTo!.gameObject) if (transitionTo != null) transitionTo.gameObject.SetActive(true);
            onTransition.Invoke();
            yield return null;
            yield return Effects.ColorFade(transitionFade.overlay, Color.black, Color.clear, 0.1f);
            callback.Invoke();
            yield break;
        }

        transitionFade.StartCoroutine(Coroutine().WrapToIl2Cpp());
    }

    public static Vector3 AsVector3(this Vector2 vec,float z)
    {
        Vector3 result = vec;
        result.z = z;
        return result;
    }
    

    /// <summary>
    /// 自分自身と、すべての子に対して手続きを実行します。
    /// </summary>
    /// <param name="obj"></param>
    public static void DoForAllChildren(GameObject obj, Action<GameObject> procedure, bool doSelf = true)
    {
        if(doSelf) procedure.Invoke(obj);

        void _sub__DoForAllChildren(GameObject parent)
        {
            for(int i = 0;i < parent.transform.childCount; i++)
            {
                var child = parent.transform.GetChild(i).gameObject;
                procedure.Invoke(child);
                _sub__DoForAllChildren(child);
            }
        }

        _sub__DoForAllChildren(obj);
    }
}

public interface ITextureLoader
{
    Texture2D GetTexture();
}

public interface IDividedSpriteLoader
{
    Sprite GetSprite(int index);
    Image AsLoader(int index) => new WrapSpriteLoader(() => GetSprite(index));
    int Length { get; }
}

public static class GraphicsHelper
{
    public static Texture2D LoadTextureFromResources(string path)
    {
        Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
        Assembly assembly = Assembly.GetExecutingAssembly();
        Stream? stream = assembly.GetManifestResourceStream(path);
        if (stream == null) return null!;
        var byteTexture = new byte[stream.Length];
        stream.Read(byteTexture, 0, (int)stream.Length);
        LoadImage(texture, byteTexture, false);
        return texture;
    }

    public static Texture2D LoadTextureFromStream(Stream stream) => LoadTextureFromByteArray(stream.ReadBytes());

    public static Texture2D LoadTextureFromByteArray(byte[] data)
    {
        Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
        LoadImage(texture, data, false);
        return texture;
    }

    public static Texture2D LoadTextureFromDisk(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                byte[] byteTexture = File.ReadAllBytes(path);
                LoadImage(texture, byteTexture, false);
                return texture;
            }
        }
        catch
        {
            //System.Console.WriteLine("Error loading texture from disk: " + path);
        }
        return null!;
    }

    public static Texture2D LoadTextureFromZip(ZipArchive? zip, string path)
    {
        if (zip == null) return null!;
        try
        {
            var entry = zip.GetEntry(path);
            if (entry != null)
            {
                Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                Stream stream = entry.Open();
                byte[] byteTexture = new byte[entry.Length];
                stream.Read(byteTexture, 0, byteTexture.Length);
                stream.Close();
                LoadImage(texture, byteTexture, false);
                return texture;
            }
        }
        catch
        {
            System.Console.WriteLine("Error loading texture from disk: " + path);
        }
        return null!;
    }

    public static Sprite ToSprite(this Texture2D texture, float pixelsPerUnit) => ToSprite(texture, new Rect(0, 0, texture.width, texture.height),pixelsPerUnit);

    public static Sprite ToSprite(this Texture2D texture, Rect rect, float pixelsPerUnit)
    {
        return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }

    public static Sprite ToSprite(this Texture2D texture, Rect rect, Vector2 pivot,float pixelsPerUnit)
    {
        return Sprite.Create(texture, rect, pivot, pixelsPerUnit);
    }

    public static Sprite ToExpandableSprite(this Texture2D texture, float pixelsPerUnit,int x,int y)
    {
        return Sprite.CreateSprite(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit, 0, SpriteMeshType.FullRect, new Vector4(x, y, x, y), false);
    }

    internal delegate bool d_LoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
    internal static d_LoadImage iCall_LoadImage = null!;
    public static bool LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
    {
        if (iCall_LoadImage == null)
            iCall_LoadImage = IL2CPP.ResolveICall<d_LoadImage>("UnityEngine.ImageConversion::LoadImage");
        var il2cppArray = (Il2CppStructArray<byte>)data;
        return iCall_LoadImage.Invoke(tex.Pointer, il2cppArray.Pointer, markNonReadable);
    }
}

public class ResourceTextureLoader : ITextureLoader
{
    string address;
    Texture2D? texture = null;

    public ResourceTextureLoader(string address)
    {
        this.address = address;
    }

    public Texture2D GetTexture()
    {
        if (!texture) texture = GraphicsHelper.LoadTextureFromResources(address);
        return texture!;
    }
}

public class DiskTextureLoader : ITextureLoader
{
    string address;
    Texture2D texture = null!;
    bool isUnloadAsset = false;

    public DiskTextureLoader MarkAsUnloadAsset() { isUnloadAsset = true; return this; }
    public DiskTextureLoader(string address)
    {
        this.address = address;
    }

    public Texture2D GetTexture()
    {
        if (!texture)
        {
            texture = GraphicsHelper.LoadTextureFromDisk(address);
            if (isUnloadAsset) texture.hideFlags |= HideFlags.DontUnloadUnusedAsset | HideFlags.HideAndDontSave;
        }
        return texture;
    }
}

public class ZipTextureLoader : ITextureLoader
{
    ZipArchive archive;
    string address;
    Texture2D texture = null!;

    public ZipTextureLoader(ZipArchive zip,string address)
    {
        this.archive = zip;
        this.address = address;
    }

    public Texture2D GetTexture()
    {
        if (!texture)
        {
            texture = GraphicsHelper.LoadTextureFromZip(archive, address);
            if(texture!=null) texture.hideFlags |= HideFlags.DontUnloadUnusedAsset | HideFlags.HideAndDontSave;
        }
        return texture!;
    }
}

public class StreamTextureLoader : ITextureLoader
{
    Texture2D texture;

    public StreamTextureLoader(Stream stream)
    {
        texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
        GraphicsHelper.LoadImage(texture, stream.ReadBytes(), false);
        texture.hideFlags |= HideFlags.DontUnloadUnusedAsset | HideFlags.HideAndDontSave;
    }

    public Texture2D GetTexture() => texture;
}

public class UnloadTextureLoader : ITextureLoader
{
    Texture2D texture = null!;
    
    public UnloadTextureLoader(byte[] byteTexture, bool isFake = false)
    {
        texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
        GraphicsHelper.LoadImage(texture, byteTexture, false);
        if (!isFake) texture.hideFlags |= HideFlags.DontUnloadUnusedAsset | HideFlags.HideAndDontSave;
    }

    public class AsyncLoader
    {
        public UnloadTextureLoader? Result { get; private set; } = null;
        Func<Stream?> stream;
        bool IsFake;

        public AsyncLoader(Func<Stream?> stream, bool isFake = false)
        {
            this.stream = stream;
            this.IsFake = isFake;
        }

        private async Task<byte[]> ReadStreamAsync(Action<Exception>? exceptionHandler = null)
        {
            try
            {
                var myStream = stream.Invoke();
                if (myStream == null) return new byte[0];

                List<byte> bytes = new();

                MemoryStream dest = new();
                await myStream.CopyToAsync(dest);
                return dest.ToArray();
            }
            catch(Exception ex)
            {
                exceptionHandler?.Invoke(ex);

            }
            return new byte[0];
        }

        public IEnumerator LoadAsync(Action<Exception>? exceptionHandler = null)
        {
            if (stream == null) yield break;

            
            var task = ReadStreamAsync(exceptionHandler);
            while (!task.IsCompleted) yield return new WaitForSeconds(0.15f);
            
            Result = new UnloadTextureLoader(task.Result,IsFake);
        }
    }

    public Texture2D GetTexture() => texture;
    
}

public class SpriteLoader : Image
{
    Sprite sprite = null!;
    float pixelsPerUnit;
    ITextureLoader textureLoader;
    public SpriteLoader(ITextureLoader textureLoader, float pixelsPerUnit)
    {
        this.textureLoader=textureLoader;
        this.pixelsPerUnit = pixelsPerUnit;
    }

    public Sprite GetSprite()
    {
        if (!sprite) sprite = textureLoader.GetTexture().ToSprite(pixelsPerUnit);
        sprite.hideFlags = textureLoader.GetTexture().hideFlags;
        return sprite;
    }

    static public SpriteLoader FromResource(string address, float pixelsPerUnit) => new SpriteLoader(new ResourceTextureLoader(address), pixelsPerUnit);
}

public class ResourceExpandableSpriteLoader : Image
{
    Sprite sprite = null!;
    string address;
    float pixelsPerUnit;
    //端のピクセル数
    int x, y;
    public ResourceExpandableSpriteLoader(string address, float pixelsPerUnit,int x,int y)
    {
        this.address = address;
        this.pixelsPerUnit = pixelsPerUnit;
        this.x = x;
        this.y = y;
    }

    public Sprite GetSprite()
    {
        if (!sprite)
            sprite = GraphicsHelper.LoadTextureFromResources(address).ToExpandableSprite(pixelsPerUnit, x, y);
        return sprite;
    }
}


public class DividedSpriteLoader : Image, IDividedSpriteLoader
{
    float pixelsPerUnit;
    Sprite[] sprites;
    ITextureLoader texture;
    Tuple<int, int>? division, size;
    public Vector2 Pivot = new Vector2(0.5f, 0.5f);

    public DividedSpriteLoader(ITextureLoader textureLoader, float pixelsPerUnit, int x, int y, bool isSize = false)
    {
        this.pixelsPerUnit = pixelsPerUnit;
        if (isSize)
        {
            this.size = new(x, y);
            this.division = null;
        }
        else
        {
            this.division = new(x, y);
            this.size = null;
        }
        sprites = null!;
        texture = textureLoader;
    }

    public Sprite GetSprite(int index)
    {
        if (size == null || division == null || sprites == null)
        {
            var texture2D = texture.GetTexture();
            if (size == null)
                size = new(texture2D.width / division!.Item1, texture2D.height / division!.Item2);
            else if (division == null)
                division = new(texture2D.width / size!.Item1, texture2D.height / size!.Item2);
            sprites = new Sprite[division!.Item1 * division!.Item2];
        }

        if (!sprites[index])
        {
            var texture2D = texture.GetTexture();
            int _x = index % division!.Item1;
            int _y = index / division!.Item1;
            sprites[index] = texture2D.ToSprite(new Rect(_x * size.Item1, (division.Item2 - _y - 1) * size.Item2, size.Item1, size.Item2), Pivot, pixelsPerUnit);
        }
        return sprites[index];
    }

    public Sprite GetSprite() => GetSprite(0);

    public Image AsLoader(int index) => new WrapSpriteLoader(() => GetSprite(index));

    public int Length {
        get {
            if (division == null) GetSprite(0);
            return division!.Item1 * division!.Item2;
        }
    }

    static public DividedSpriteLoader FromResource(string address, float pixelsPerUnit, int x, int y, bool isSize = false)
         => new DividedSpriteLoader(new ResourceTextureLoader(address), pixelsPerUnit, x, y, isSize);
    static public DividedSpriteLoader FromDisk(string address, float pixelsPerUnit, int x, int y, bool isSize = false)
         => new DividedSpriteLoader(new DiskTextureLoader(address), pixelsPerUnit, x, y, isSize);
}

public class XOnlyDividedSpriteLoader : Image, IDividedSpriteLoader
{
    float pixelsPerUnit;
    Sprite[] sprites;
    ITextureLoader texture;
    int? division, size;
    public Vector2 Pivot = new Vector2(0.5f, 0.5f);

    public XOnlyDividedSpriteLoader(ITextureLoader textureLoader, float pixelsPerUnit, int x, bool isSize = false)
    {
        this.pixelsPerUnit = pixelsPerUnit;
        if (isSize)
        {
            this.size = x;
            this.division = null;
        }
        else
        {
            this.division = x;
            this.size = null;
        }
        sprites = null!;
        texture = textureLoader;
    }

    public Sprite GetSprite(int index)
    {
        if (!size.HasValue || !division.HasValue || sprites == null)
        {
            var texture2D = texture.GetTexture();
            if (size == null)
                size = texture2D.width / division;
            else if (division == null)
                division = texture2D.width / size!;
            sprites = new Sprite[division!.Value];
        }

        if (!sprites[index])
        {
            var texture2D = texture.GetTexture();
            sprites[index] = texture2D.ToSprite(new Rect(index * size!.Value, 0, size!.Value, texture2D.height), Pivot, pixelsPerUnit);
        }
        return sprites[index];
    }

    public Sprite GetSprite() => GetSprite(0);

    public int Length
    {
        get
        {
            if (!division.HasValue) GetSprite(0);
            return division!.Value;
        }
    }

    public Image WrapLoader(int index) => new WrapSpriteLoader(() => GetSprite(index));

    static public XOnlyDividedSpriteLoader FromResource(string address, float pixelsPerUnit, int x, bool isSize = false)
         => new XOnlyDividedSpriteLoader(new ResourceTextureLoader(address), pixelsPerUnit, x, isSize);
    static public XOnlyDividedSpriteLoader FromDisk(string address, float pixelsPerUnit, int x, bool isSize = false)
         => new XOnlyDividedSpriteLoader(new DiskTextureLoader(address), pixelsPerUnit, x, isSize);
}

public class WrapSpriteLoader : Image
{
    Func<Sprite> supplier;

    public WrapSpriteLoader(Func<Sprite> supplier)
    {
        this.supplier = supplier;
    }

    public Sprite GetSprite() => supplier.Invoke();
}