using Fabric.Crashlytics;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static void Enable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    public static void Disable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    private static void HandleLog(string logString, string stackTrace, LogType type)
    {

        // only capture log from main thread, otherwise crashes
        if (Application.platform == RuntimePlatform.Android && Game.Get().mainThread.Equals(System.Threading.Thread.CurrentThread))
        {
            Crashlytics.Log(logString);
        }
    }

    public static void Crash()
    {
        Crashlytics.Crash();
    }
}
