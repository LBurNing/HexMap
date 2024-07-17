using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static HexGrid;

[Serializable]
public class Area
{
    public string areaName;
    public int areaType;
    public AreaAttr areaAttr;
    public List<GridPos> gridPos;
}



[Serializable]
public class AreaAttr
{
    //传送坐标
    public int x;
    public int y;

    //进入区域音乐id
    public string musicName;

    //进入区域音效Id
    public string soundEffectName;

    //剧情id
    public string juQingId;

    //要替换的资源名称
    public string resName;
    
    //可通行
    public int walkable = 1;
    //障碍
    public int obstacle = 1;
    //特殊通行
    public int specialPassage = 1;
}

[Serializable]
public class AreaTypeRoot
{
    public List<AreaType> areaTypes = new List<AreaType>();
}

[Serializable]
public class AreaType
{
    public int areaType;
    public string areaName;
    public string areaEName;
}

public class AreaManager
{
    private static AreaManager _instance;
    private AreaTypeRoot areaTypeRoot;

    public AreaType selectAreaType { get; set; }

    public bool brushAreaing { get; set; }

    private Dictionary<int, Area> _areaDic;

    //正在画的区域
    public Area drawAreaing { get; set; }

    public int areaId { get; set; }

    public List<Color> areaColors = new List<Color>()
    {
        new Color(Color.green.r, Color.green.g, Color.green.b, 0.5f),    //绿色
        new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.5f),    //黄色
        new Color(Color.magenta.r, Color.magenta.g, Color.magenta.b, 0.5f),    //品红
        new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.5f),    //青色
        new Color(0.3481923f, 0.05739204f, 0.5754717f, 0.5f),    //紫色
    };

    public static AreaManager instance
    {
        get
        {
            if (_instance == null)
                _instance = new AreaManager();

            return _instance;
        }
    }

    public Dictionary<int, Area> areaDic
    {
        get { return _areaDic; }
        set
        {
            if (_areaDic == null)
                _areaDic = new Dictionary<int, Area>();

            _areaDic = value;
        }
    }
    public AreaManager()
    {
        areaTypeRoot = new AreaTypeRoot();
        ReadAreaTypeJson();
    }

    public Color GetAreaColor(int areaType)
    {
        Color color = new Color(0, 0, 255, 0.3f);
        if (areaType <= areaColors.Count)
        {
            color = areaColors[areaType];
        }

        return color;
    }

    public void ReadAreaTypeJson()
    {
        if (!File.Exists(FileUtil.areaJsonPath))
        {
            return;
        }

        StreamReader streamReader = new StreamReader(FileUtil.areaJsonPath);
        if (streamReader == null)
            return;

        string json = streamReader.ReadToEnd();
        if (json.Length <= 0)
            return;

        areaTypeRoot = JsonConvert.DeserializeObject<AreaTypeRoot>(json);
        if (areaTypeRoot != null && areaTypeRoot.areaTypes != null)
            selectAreaType = areaTypeRoot.areaTypes[0];
    }

    private void SaveAreaTypeJson()
    {
        if (string.IsNullOrEmpty(FileUtil.areaJsonPath))
            return;

        StreamWriter streamWriter = new StreamWriter(FileUtil.areaJsonPath);
        if (streamWriter == null)
            return;

        string json = JsonConvert.SerializeObject(areaTypeRoot);
        streamWriter.WriteLine(json);

        streamWriter.Close();
        streamWriter.Dispose();
    }

    public void SaveAreaType(AreaType areaType)
    {
        if (areaType == null)
            return;


        if (areaTypeRoot == null)
            areaTypeRoot = new AreaTypeRoot();

        if (areaTypeRoot.areaTypes == null)
            areaTypeRoot.areaTypes = new List<AreaType>();

        areaTypeRoot.areaTypes.Add(areaType);
        SaveAreaTypeJson();
    }

    public void RemoveAreaType(AreaType areaType)
    {
        if (areaType == null)
            return;


        if (areaTypeRoot == null)
            areaTypeRoot = new AreaTypeRoot();

        if (areaTypeRoot.areaTypes == null)
            areaTypeRoot.areaTypes = new List<AreaType>();

        areaTypeRoot.areaTypes.Remove(areaType);
        SaveAreaTypeJson();
    }

    public void Reset()
    {
        drawAreaing = null;
    }

    public List<AreaType> GetAreaTypeList()
    {
        if (areaTypeRoot == null)
            return null;

        return areaTypeRoot.areaTypes;
    }
    public AreaType GetAreaType(int areaType)
    {
        foreach (AreaType value in areaTypeRoot.areaTypes)
        {
            if (value.areaType == areaType)
                return value;
        }

        return null;
    }

    public Area GetArea()
    {
        return _areaDic[areaId];
    }

    public void CreateArea()
    {
        if (selectAreaType == null)
            return;

        drawAreaing = new Area();
        drawAreaing.areaName = selectAreaType.areaName;
        drawAreaing.areaType = selectAreaType.areaType;
    }


    public void DrawArea(int key, bool isDraw = true)
    {
        if (!isDraw)
        {
            MeshCombineManager.Instance.DrawImage(drawAreaing.gridPos, selectAreaType.areaEName,0);
        }
        else
        {
            if (_areaDic[key].gridPos.Count > 0)
            {
                string areaName = string.Format("{0}_{1}", selectAreaType.areaEName, selectAreaType.areaType);
                MeshCombineManager.Instance.DrawImage(_areaDic[key].gridPos, areaName ,0);
            }
        }

    }

    public void SetAreaDic(AreaAttr areaAttr)
    {
        if (drawAreaing == null)
            return;
        drawAreaing.areaAttr = areaAttr;
        _areaDic[areaId] = drawAreaing;
    }

    public void SetAreaPos(int x,int y)
    {
        if(areaId == 0)
        {
            areaId = 10000 + _areaDic.Count;
        }

        if (drawAreaing.gridPos == null)
            drawAreaing.gridPos = new List<GridPos>();

        GridPos areaPos = new GridPos(x,y);
        if (!drawAreaing.gridPos.Exists(t=>t.x == x && t.y == y))
        {
            drawAreaing.gridPos.Add(areaPos);
        }
        _areaDic[areaId] = drawAreaing;
        DrawArea(areaId);
    }

    public void RemoveAreaPos(int x,int y)
    {
        var index = drawAreaing.gridPos.FindIndex(t => t.x == x && t.y == y);
        if (index == -1)
            return;

        drawAreaing.gridPos.RemoveAt(index);
        _areaDic[areaId] = drawAreaing;
        DrawArea(areaId);
    }
}
