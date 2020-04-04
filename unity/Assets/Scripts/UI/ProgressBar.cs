using UnityEngine;
using UnityEngine.Networking;

// This class is used to make a sprite change size over time
// It can be attached to a unity gameobject
public class ProgressBar : MonoBehaviour
{
    private RectTransform rect;
    private float xEdge = 0;
    private float size = 0;
    private UnityWebRequest download;

    /// <summary>
    /// Set the WWW object to monitor</summary>
    /// <param name="d">WWW object</param>
    public void SetDownload(UnityWebRequest d)
    {
        download = d;
    }

    /// <summary>
    /// Called at init by unity.</summary>
    private void Start()
    {
        // Get the image attached to this game object
        rect = gameObject.GetComponent<RectTransform>();
        size = rect.sizeDelta.x;
        xEdge = rect.anchoredPosition.x - (size / 2);
    }

    /// <summary>
    /// Called once per frame by Unity.</summary>
    /// <param name="height">Vertical size.</param>
    private void Update()
    {
        float fill = 0;
        if (download != null && download.error == null)
        {
            fill = download.downloadProgress * size;
        }
        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, xEdge, fill);
    }
}
