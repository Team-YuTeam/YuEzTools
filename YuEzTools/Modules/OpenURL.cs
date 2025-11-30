using System.Runtime.InteropServices;
using UnityEngine;

namespace YuEzTools.Modules;

public class OpenURL
{
    public static void OpenUrl(string url)
    {
        #if Windows
        Application.OpenURL(url);
#elif Android
OpenURLAndroid(url);
#endif
    }
#if Android
    // from fs
    private static void OpenURLAndroid(string url)
    {
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // 创建 Intent
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject =
                new AndroidJavaObject("android.content.Intent", "android.intent.action.VIEW");

            // 创建 URI
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", url);

            // 设置 Intent 的数据
            intentObject.Call<AndroidJavaObject>("setData", uriObject);

            // 设置标志确保在新任务中打开
            int FLAG_ACTIVITY_NEW_TASK = 0x10000000;
            intentObject.Call<AndroidJavaObject>("setFlags", FLAG_ACTIVITY_NEW_TASK);

            // 启动 Activity
            currentActivity.Call("startActivity", intentObject);

            // 释放资源（虽然不是必须的，但推荐）
            unityPlayer.Dispose();
            currentActivity.Dispose();
            intentClass.Dispose();
            intentObject.Dispose();
            uriClass.Dispose();
            uriObject.Dispose();
        }
        catch
        {
            // 降级到Application.OpenURL
            Application.OpenURL(url);
        }
    }
    #endif
}