namespace Fabric.Internal.Editor.Images
{
    using UnityEditor;
    using UnityEngine;

    public class Loader
    {
        private static readonly string root = "Assets/Fabric/Editor/GUI/Images/";

        public static Texture2D Load(string resource)
        {
            return AssetDatabase.LoadAssetAtPath(root + resource, typeof(Texture2D)) as Texture2D;
        }
    }
}
