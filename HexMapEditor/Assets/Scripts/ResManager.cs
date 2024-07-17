using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;

public enum LayerType
{
    //µØÍ¼
    [Description("µØ±í")]
    map = 1,

    //µ×°å
    [Description("µ××ù")]
    floor = 2,

    //¹ÖÎï
    [Description("µ¥Î»")]
    unit = 3,

    //ÃÔÎí
    [Description("ÃÔÎí")]
    fog = 4,
}

public class TextureData : IEquatable<TextureData>
{
    public string _fileName;
    public string _resName;
    private TextureSprite _textureSprite;

    public TextureData(TextureSprite textureSprite)
    {
        _textureSprite = textureSprite;
    }

    public Sprite GetTexture2D(int index)
    {
        return _textureSprite.GetSprite(index);
    }

    public int FrameCount()
    {
        return _textureSprite.FrameCount();
    }

    public bool Equals(TextureData other)
    {
        return other._fileName.Equals(_fileName) && other._resName.Equals(_resName);
    }
}

public class ResManager
{
    public static ResManager _instance;
    private Dictionary<string, List<TextureData>> _ress;

    public static ResManager instance
    {
        get
        {
            if (_instance == null)
                _instance = new ResManager();

            return _instance;
        }
    }

    public ResManager()
    {
        _ress = new Dictionary<string, List<TextureData>>();
    }

    public void UpdateRes()
    {
        LoadRes();
    }

    public void LoadRes()
    {
        foreach(var resType in Enum.GetNames(typeof(LayerType)))
        {
            List<TextureData> texDatas;
            if(!_ress.TryGetValue(resType, out texDatas))
            {
                texDatas = new List<TextureData>();
                _ress.Add(resType, texDatas);
            }

            string resPath = Path.GetFullPath(FileUtil.resPath + "/" + resType);
            if (!Directory.Exists(resPath))
                continue;

            List<string> directories = FileUtil.GetDirectories(resPath);
            foreach (string dir in directories)
            {
                List<string> resDirectories = FileUtil.GetDirectories(dir);
                foreach (string resDir in resDirectories)
                {
                    string fileName = Path.GetFileNameWithoutExtension(resDir);
                    TextureSprite textureSprite = new TextureSprite();
                    textureSprite.Load(fileName, resDir);

                    TextureData texData = new TextureData(textureSprite);
                    texData._resName = fileName;
                    texData._fileName = Path.GetFileNameWithoutExtension(dir);

                    if (!texDatas.Contains(texData))
                        texDatas.Add(texData);
                }
            }
        }
    }

    public TextureData GetTextureData(string resType, string fileName, string resName)
    {
        var texDatas = GetTextureData(resType);
        if (texDatas == null)
            return null;

        foreach (var val in texDatas)
        {
            if (val._fileName.Equals(fileName) && val._resName.Equals(resName))
                return val;
        }

        return null;
    }

    public List<TextureData> GetTextureData(string resType)
    {
        List<TextureData> texDatas;
        if (!_ress.TryGetValue(resType, out texDatas))
            return null;

        return texDatas;
    }

    public Dictionary<string, List<TextureData>> GetRess()
    {
        return _ress;
    }

    public int GetResLength()
    {
        return _ress.Count;
    }

    public static string GetDescription(string val)
    {
        var field = Enum.Parse(typeof(LayerType), val).GetType().GetField(val);
        var customAttribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
        return customAttribute == null ? val : ((DescriptionAttribute)customAttribute).Description;
    }

    public void Dispose()
    {
    }
}
