using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace YuAntiCheat;

public class SendInGamePatch
{
    public static void SendInGame(string text, bool isAlways = false)
    {
        if (DestroyableSingleton<HudManager>._instance) DestroyableSingleton<HudManager>.Instance.Notifier.AddItem(text);
    }
}