using System;
using UnityEngine;

public class CommonScriptFuntions
{
    public static Texture2D RotateImage(Texture2D originTexture, int angle)
    {
        if (originTexture != null)
        {

            RenderTexture tmp = RenderTexture.GetTemporary(originTexture.width, originTexture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit(originTexture, tmp);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;
            Texture2D myTexture2D = new Texture2D(originTexture.width, originTexture.height);
            // Copy the pixels from the RenderTexture to the new Texture
            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();
            RenderTexture.active = previous;
            // Release the temporary RenderTexture
            RenderTexture.ReleaseTemporary(tmp);

            Texture2D result;
            result = new Texture2D(myTexture2D.width, myTexture2D.height);
            Color32[] pix1 = result.GetPixels32();
            Color32[] pix2 = myTexture2D.GetPixels32();
            int W = myTexture2D.width;
            int H = myTexture2D.height;
            int x = 0;
            int y = 0;
            Color32[] pix3 = RotateSquare(pix2, (Math.PI / 180 * angle), myTexture2D);
            for (int j = 0; j < H; j++)
            {
                for (int i = 0; i < W; i++)
                {
                    //pix1[result.width/2 - originTexture.width/2 + x + i + result.width*(result.height/2-originTexture.height/2+j+y)] = pix2[i + j*originTexture.width];
                    pix1[result.width / 2 - W / 2 + x + i + result.width * (result.height / 2 - H / 2 + j + y)] = pix3[i + j * W];
                }
            }
            result.SetPixels32(pix1);
            result.Apply();
            return result;
        }
        else
            return null;
    }

    private static Color32[] RotateSquare(Color32[] arr, double phi, Texture2D originTexture)
    {
        int x;
        int y;
        int i;
        int j;
        double sn = Math.Sin(phi);
        double cs = Math.Cos(phi);
        Color32[] arr2 = originTexture.GetPixels32();
        int W = originTexture.width;
        int H = originTexture.height;
        int xc = W / 2;
        int yc = H / 2;
        for (j = 0; j < H; j++)
        {
            for (i = 0; i < W; i++)
            {
                arr2[j * W + i] = new Color32(0, 0, 0, 0);
                x = (int)(cs * (i - xc) + sn * (j - yc) + xc);
                y = (int)(-sn * (i - xc) + cs * (j - yc) + yc);
                if ((x > -1) && (x < W) && (y > -1) && (y < H))
                {
                    arr2[j * W + i] = arr[y * W + x];
                }
            }
        }
        return arr2;
    }

    public static void RotateGameObject(GameObject _you, float degree)
    {
        _you.transform.rotation = Quaternion.Euler(new Vector3(0, 0, degree));
    }
}

