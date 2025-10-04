namespace YuEzTools;

public class SendInGamePatch
{
    public static void SendInGame(string text)
    {
        if (DestroyableSingleton<HudManager>._instance) 
            HudManager.Instance.Notifier.AddDisconnectMessage(text);
    }
}