using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class Meta
{
    public SourceSize size;
}

[Serializable]
public class Frame
{
    public int x;
    public int y;
    public int w;
    public int h;
}

[Serializable]
public class SpriteSourceSize
{
    public int x;
    public int y;
    public int w;
    public int h;
}

[Serializable]
public class SourceSize
{
    public int w;
    public int h;
}

[Serializable]
public class SpriteData
{
    public string filename;
    public Frame frame;
    public bool rotated;
    public bool trimmed;
    public SpriteSourceSize spriteSourceSize;
    public SourceSize sourceSize;
}

[Serializable]
public class SpriteDataRoot
{
    public List<SpriteData> frames = new List<SpriteData>();
    public Meta meta;
}

public class TextureSprite
{
    private int MAX_TEXTURE_SIZE = 4096;
    private Dictionary<string, Sprite> _spriteDict;
    private List<Sprite> _spriteList;
    private Texture2D _texture2D;

    public TextureSprite()
    {
        _spriteDict = new Dictionary<string, Sprite>();
        _spriteList = new List<Sprite>();
    }

    public void Load(string fileName, string path)
    {
        string texPath = string.Format("{0}\\{1}.png", path, fileName + Global.TEX_NAME);
        string textPath = string.Format("{0}\\{1}.json", path, fileName + Global.TEXT_NAME);
        string paramText = File.ReadAllText(textPath);
        _texture2D = FileUtil.LoadTextureByIO(texPath);

        SpriteDataRoot data = JsonConvert.DeserializeObject<SpriteDataRoot>(paramText);
        foreach (var value in data.frames)
        {
            Frame frame = value.frame;
            int x = frame.x;
            //Texturepacker坐标系是左上角
            int y = data.meta.size.h - frame.y - frame.h;
            int blockWidth = frame.w;
            int blockHeight = frame.h;

            Rect rect = new Rect(x, y, blockWidth, blockHeight);
            Vector2 pivot = new Vector2(0.5f, 0.5f - (value.spriteSourceSize.h - 256) / 2.0f / (float)(value.spriteSourceSize.h + value.spriteSourceSize.y));
            Sprite newSprite = Sprite.Create(_texture2D, rect, pivot);
            _spriteDict[value.filename] = newSprite;
            _spriteList.Add(newSprite);
        }
    }

    public int FrameCount()
    {
        return _spriteList.Count;
    }

    public Sprite GetSprite(int index)
    {
        if (_spriteList != null && _spriteList.Count > index)
            return _spriteList[index];
        return null;
    }

    public Sprite GetSprite(string name)
    {
        Sprite sprite;
        _spriteDict.TryGetValue(name, out sprite);
        return sprite;
    }

    public void Dispose()
    {
        if (_spriteDict != null)
        {
            _spriteDict.Clear();
            _spriteDict = null;
        }

        for (int i = 0; i < _spriteList.Count; i++)
        {
            GameObject.Destroy(_spriteList[i]);
        }
        _spriteList = null;

        if (_texture2D != null)
        {
            GameObject.Destroy(_texture2D);
            _texture2D = null;
        }
    }
}
