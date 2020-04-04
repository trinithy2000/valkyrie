namespace Fabric.Internal.Crashlytics.Editor
{
    using Fabric.Internal.Editor.Model;
    using UnityEditor;
    using UnityEngine;

    public static class CrashlyticsMenuItems
    {
        private const string Name = Controller.Controller.Name;

        private static bool Enabled()
        {
            Settings.InstalledKit installed = Settings.Instance.InstalledKits.Find(k => k.Name.Equals(Name));
            return installed != null && installed.Installed && installed.Enabled;
        }

        private static bool Disabled()
        {
            Settings.InstalledKit installed = Settings.Instance.InstalledKits.Find(k => k.Name.Equals(Name));
            return installed != null && installed.Installed && !installed.Enabled;
        }

        [MenuItem("Fabric/Crashlytics/Enable Crashlytics", false, 1)]
        public static void EnableCrashlytics()
        {
            CrashlyticsSetup.EnableCrashlytics(true);
        }

        [MenuItem("Fabric/Crashlytics/Enable Crashlytics", true)]
        public static bool ValidateEnableCrashlytics()
        {
            return Disabled();
        }

        [MenuItem("Fabric/Crashlytics/Disable Crashlytics", false, 1)]
        public static void DisableCrashlytics()
        {
            CrashlyticsSetup.DisableCrashlytics();
        }

        [MenuItem("Fabric/Crashlytics/Disable Crashlytics", true)]
        public static bool ValidateDisableCrashlytics()
        {
            return Enabled();
        }

        [MenuItem("Fabric/Crashlytics/Documentation", false, 1)]
        public static void OpenCrashlyticsDocs()
        {
            Application.OpenURL("https://docs.fabric.io/unity/crashlytics/index.html");
        }

    }

}
