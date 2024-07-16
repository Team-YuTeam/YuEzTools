using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace YuAntiCheat.Utils;

public class Utils
{
    public static string getColoredPingText(int ping){

        if (ping <= 100){ // Green for ping < 100

            return $"<color=#00ff00ff>\nPing: {ping} ms</color>";

        } else if (ping < 400){ // Yellow for 100 < ping < 400

            return $"<color=#ffff00ff>\nPing: {ping} ms</color>";

        } else{ // Red for ping > 400

            return $"<color=#ff0000ff>\nPing: {ping} ms</color>";
        }
    }
    public static string ColorString(Color32 color, string str) => $"<color=#{color.r:x2}{color.g:x2}{color.b:x2}{color.a:x2}>{str}</color>";
    public static string GetString(StringNames stringName)
        => DestroyableSingleton<TranslationController>.Instance.GetString(stringName, new Il2CppReferenceArray<Il2CppSystem.Object>(0));
    public static string getColoredFPSText(float fps)
    {
        string a = !Toggles.ShowPing ? "\n" : "  ";
        if (fps >= 100){ // Green for fps > 100

            return a + $"<color=#00ff00ff>FPS: {fps}</color>";

        } else if (fps < 100 & fps > 50){ // Yellow for 100 > fps > 50

            return a + $"<color=#ffff00ff>FPS: {fps}</color>";

        } else{ // Red for fps < 50

            return a + $"<color=#ff0000ff>FPS: {fps}</color>";
        }
    }
    public static KeyCode stringToKeycode(string keyCodeStr){

        if(!string.IsNullOrEmpty(keyCodeStr)){ // Empty strings are automatically invalid

            try{
                
                // Case-insensitive parse of UnityEngine.KeyCode to check if string is validssss
                KeyCode keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyCodeStr, true);
                
                return keyCode;

            }catch{}
        
        }

        return KeyCode.Delete; // If string is invalid, return Delete as the default key
    }
}