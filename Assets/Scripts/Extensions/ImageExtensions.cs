using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.U2D;

public static class ImageExtensions
{
    public static void LoadSprite(this Image image, string resPath)
    {
        if (image == null) return;

        Sprite sp = Resources.Load(string.Format("Textures/{0}", resPath), typeof(Sprite)) as Sprite;
        if (sp != null)
        {
            image.sprite = sp;
        }
    }

    public static void LoadSprite(this Image image, string atlasName, string spriteName)
    {
        if (image == null) return;

        SpriteAtlas atlas = Resources.Load(string.Format("SpriteAtlas/{0}", atlasName), typeof(SpriteAtlas)) as SpriteAtlas;
        if (atlas != null)
        {
            Sprite sprite = atlas.GetSprite(spriteName);

            if (sprite != null)
            {
                image.sprite = sprite;
            }
        }
    }
}
