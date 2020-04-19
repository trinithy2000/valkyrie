﻿using UnityEngine;

// I couldn't work out unity scaling so I wrote my own.
// The screen is alway 30 'units' high.  At 4:3 it is 40 across, at 16:9 it is 53.33.
// I have not designed for 5:4 (37.5 units)
// 1 unit is enough for 'small' text with a border.  1.5 is medium text, 3 is big text
// Everything is floats so partial units are fine.

// This is a utily class to help with screen placement and scaling
public class UIScaler
{

    public int widthPx;
    public int heightPx;
    // Everything is based around the screen being 30 units high
    public static float rowsOfUnits = 30;

    // Initialise
    // We don't handle resizing after this
    public UIScaler(Canvas c)
    {
        // The canvas is positioned so that 0 is in the centre, so double
        // The units of the UI canvas are in pixels
        widthPx = Mathf.RoundToInt(c.transform.position.x * 2);
        heightPx = Mathf.RoundToInt(c.transform.position.y * 2);
    }

    // Number of pixels (canvas units) per scale unit
    public static float GetPixelsPerUnit()
    {
        Game game = Game.Get();
        return game.uiScaler.heightPx / rowsOfUnits;
    }

    // Convert a scale unit vector to canvas units
    public static Vector2 Location(float x, float y)
    {
        return new Vector2(x * GetPixelsPerUnit(), y * GetPixelsPerUnit());
    }

    // Get the width of the screen in scale units
    public static float GetWidthUnits()
    {
        Game game = Game.Get();
        return game.uiScaler.widthPx / GetPixelsPerUnit();
    }

    // Get the hieght of the screen in scale units (should always be 30)
    public static float GetHeightUnits()
    {
        Game game = Game.Get();
        return game.uiScaler.heightPx / GetPixelsPerUnit();
    }

    // Get the right most position of the screen in scale units, with an offset
    public static float GetRight(float offset = 0)
    {
        return GetWidthUnits() + offset;
    }

    public static float GetLeft(float offset = 0)
    {
        return 0 + offset;
    }
    // Get the lowest position of the screen in scale units, with an offset
    public static float GetBottom(float offset = 0)
    {
        return GetHeightUnits() + offset;
    }

    public static float GetTop(float offset = 0)
    {
        return 0 + offset;
    }

    // Get the vertical mid point of the screen in scale units, with an offset
    public static float GetVCenter(float offset = 0)
    {
        return (rowsOfUnits / 2) + offset;
    }

    // Get the horizontal mid point of the screen in scale units, with an offset
    public static float GetHCenter(float offset = 0)
    {
        return (GetWidthUnits() / 2) + offset;
    }

    public static int GetSmallestFont(float scale = 1)
    {
        return Mathf.RoundToInt(GetPixelsPerUnit() * 0.7f * scale);
    }

    // small (standard) font size
    public static int GetSmallFont(float scale = 1)
    {
        return Mathf.RoundToInt(GetPixelsPerUnit() * 0.8f * scale);
    }

    public static int GetSemiSmallFont(float scale = 1)
    {
        return Mathf.RoundToInt(GetPixelsPerUnit() * 0.92f * scale);
    }
    // medium font size
    public static int GetMediumFont(float scale = 1)
    {
        return Mathf.RoundToInt(GetPixelsPerUnit() * 1.2f * scale);
    }

    // large font size
    public static int GetLargeFont(float scale = 1)
    {
        return Mathf.RoundToInt(GetPixelsPerUnit() * 2.3f * scale);
    }

    public static int GetBigFont(float scale = 1)
    {
        return Mathf.RoundToInt(GetPixelsPerUnit() * 1.9f * scale);
    }

    public static float GetRelWidth(float width)
    {
        return UIScaler.GetWidthUnits() / width;
    }

    public static float GetRelHeight(float height)
    {
        return UIScaler.GetHeightUnits() / height;
    }
}
