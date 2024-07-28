using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace YuEzTools;

public class SendInGamePatch
{
    public static void SendInGame(string text)
    {
        if (DestroyableSingleton<HudManager>._instance) 
            HudManager.Instance.Notifier.AddDisconnectMessage(text);
    }
}