namespace Fabric.Internal.Editor.Postbuild
{
    using Fabric.Internal.Editor.Model;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEngine;

    public class FabricBuildEventAndroid : MonoBehaviour
    {

        [PostProcessBuild(100)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {

            if (buildTarget == BuildTarget.Android)
            {
                SendBuildEvent();
            }

        }

        public static void SendBuildEvent()
        {
            Settings settings = Settings.Instance;

            Utils.Log("Sending build information");

            if (string.IsNullOrEmpty(settings.Organization.ApiKey))
            {
                Utils.Error("API key not found");
                return;
            }

#if UNITY_5_6_OR_NEWER
            string bundleId = PlayerSettings.applicationIdentifier;
#else
			var bundleId = PlayerSettings.applicationIdentifier;
#endif
            WWWForm form = new WWWForm();
            form.AddField("app_name", bundleId);
            form.AddField("app_identifier", bundleId);
            form.AddField("base_identifier", bundleId);
            form.AddField("platform_id", "android");

            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "X-CRASHLYTICS-DEVELOPER-TOKEN", "771b48927ee581a1f2ba1bf60629f8eb34a5b63f" },
                { "X-CRASHLYTICS-API-KEY", settings.Organization.ApiKey },
                { "X-CRASHLYTICS-API-CLIENT-ID", "io.fabric.tools.unity" },
                { "X-CRASHLYTICS-API-CLIENT-DISPLAY-VERSION", "1.0.0" }
            };

            string url = "https://api.crashlytics.com/spi/v1/platforms/android/apps/" + bundleId + "/built";
            byte[] rawData = form.data;
            WWW www = new WWW(url, rawData, headers);

            bool timeout = false;
            DateTime t0 = DateTime.UtcNow;
            while (!www.isDone)
            {
                DateTime t1 = DateTime.UtcNow;
                int delta = (int)(t1 - t0).TotalSeconds;
                if (delta > 5)
                {
                    timeout = true;
                    break;
                }
            };

            if (timeout)
            {
                Utils.Warn("Timed out waiting for build event response. If this is a production build, you may want to build again to ensure it has been properly sent.");
            }
            else if (string.IsNullOrEmpty(www.error))
            {
                Utils.Log("Build information sent");
            }
            else
            {
                Utils.Warn("Could not send build event. Error: " + www.error);
            }
        }

        private static string SerializeToJSON(Dictionary<string, string> dict)
        {
            string json = "{";
            foreach (KeyValuePair<string, string> line in dict)
            {
                json += string.Format("\"{0}\":\"{1}\",", line.Key, line.Value);
            }
            json += "}";
            return json;
        }
    }

}
